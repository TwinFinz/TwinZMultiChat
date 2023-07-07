using Discord.WebSocket;
using Discord;
using System.Diagnostics;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Net;
using System.Text.RegularExpressions;
using TwinZMultiChat.Utilities;
using System.Threading.Tasks;
using System.CodeDom;
using System.Diagnostics.Contracts;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using Discord.Net.Queue;
using Discord.Net.Rest;
using Microsoft.VisualBasic.Logging;
using Google.Apis.Auth.OAuth2;
using System.Net.Sockets;
using Discord.Rest;

namespace TwinZMultiChat.Utilities
{
#pragma warning disable CA1822 // If i wanted it static i would have initiated as static
    public class MyDiscordAPI
    {
        private static readonly object _LogFileLock = new();
        private static string discordBotToken = "";
        private static TwinZMultiChat.MainForm? UiForm;
        private static DiscordSocketClient? _client;
        private static Dictionary<string, string> _streamers = new();
        private readonly Dictionary<string, bool> _streamsStatus = new();
        private List<ulong> _streamChannels = new();
        private List<ulong> _modChannels = new();
        private List<ulong> _ignoreChannels = new();
        //private List<ulong> _serverChannels = new(); // For Detecting server channels
        private HashSet<string> _bannedWords = new();
        private Dictionary<ulong, int> _userStrikes = new();
        private readonly Dictionary<ulong, List<IInviteMetadata>> invite_uses_before = new();
        private readonly Dictionary<ulong, List<IInviteMetadata>> invite_uses_after = new();
        private readonly Dictionary<string, List<ulong>> counter = new();
        private ulong WelcomeChannel = 1062978795168075839;
        private static ulong channelID = ulong.MinValue; 
        private readonly string wordListName = "wordbanlist.txt";
        private static readonly string StreamIntro = "Stream live: ";
        private static bool Waiting = false;

        public event EventHandler<SocketMessage>? ChatMessageReceived;
        public event EventHandler<SocketMessage>? ChatMessageUpdated;
        public readonly string StreamMsgIntro = "TMC: ";

        public MyDiscordAPI(TwinZMultiChat.MainForm sender, string DiscordBotToken, ulong DiscordChannelID) // Restore Data
        {
            UiForm = sender;
            discordBotToken = DiscordBotToken;
            channelID = DiscordChannelID;
        }

        public async Task ConnectAsync()
        {
            DiscordSocketConfig socketConfig = new()
            {
                GatewayIntents = GatewayIntents.MessageContent | GatewayIntents.Guilds | GatewayIntents.GuildBans | GatewayIntents.GuildEmojis | GatewayIntents.GuildIntegrations | GatewayIntents.GuildMembers | GatewayIntents.GuildMessages | GatewayIntents.GuildMessageReactions | GatewayIntents.GuildMessageTyping | GatewayIntents.GuildWebhooks | GatewayIntents.DirectMessages | GatewayIntents.DirectMessageReactions | GatewayIntents.DirectMessageTyping
            };
            _client = new DiscordSocketClient(socketConfig);
            await _client!.LoginAsync(TokenType.Bot, discordBotToken);
            await _client!.StartAsync();

            _client!.Connected += OnConnected;
            _client!.Disconnected += OnDisconnected;
            _client.Log += WriteLog; // Optional: Handle logging events
        }

        public async Task DisconnectAsync()
        {
            Waiting = false;
            await DeregisterEvents();
            if (_client != null)
            {
                _client!.Connected -= OnConnected;
                _client!.Disconnected -= OnDisconnected;
                _client.Log -= WriteLog;
                await _client!.StopAsync();
            }
        }

        #region Events/Handlers
        private async Task RegisterEvents()
        {
            await Task.Delay(3);
            _client!.MessageReceived += HandleMessageReceived;
            _client!.MessageUpdated += HandleMessageUpdate;
            _client!.UserJoined += HandleUserJoined;
            _client!.GuildScheduledEventCompleted += GuildEventCompleted;
        }

        private async Task DeregisterEvents()
        {
            await Task.Delay(3);
            _client!.MessageReceived -= HandleMessageReceived;
            _client!.MessageUpdated -= HandleMessageUpdate;
            _client!.UserJoined -= HandleUserJoined;
            _client!.GuildScheduledEventCompleted -= GuildEventCompleted;
        }

        private Task HandleMessageUpdate(Cacheable<IMessage, ulong> Cache, SocketMessage message, ISocketMessageChannel channel)
        {
            Task.Run(() => ChatMessageUpdated?.Invoke(this, message));
            return Task.CompletedTask;
        }

        private Task HandleMessageReceived(SocketMessage message)
        {
            Task.Run(async () =>
            {
                await ManageCommandFunctions(message, message.Channel);
            });
            return Task.CompletedTask;
        }

        private async Task HandleUserJoined(SocketUser user)
        {
            SocketGuild guild = _client!.GetGuild(WelcomeChannel)!;
            if (guild != null)
            {
                invite_uses_after[guild.Id] = new List<IInviteMetadata>();
                var invites = guild.GetInvitesAsync().Result;
                invite_uses_after[guild.Id].AddRange(invites);

                for (int i = 0; i < invite_uses_after[guild.Id].Count; i++)
                {
                    if (!counter.ContainsKey(invite_uses_after[guild.Id][i].Code))
                    {
                        counter[invite_uses_after[guild.Id][i].Code] = new List<ulong>();
                    }

                    counter[invite_uses_after[guild.Id][i].Code].Add(user.Id);
                    if (invite_uses_before[guild.Id][i].Uses != invite_uses_after[guild.Id][i].Uses)
                    {
                        if (counter[invite_uses_after[guild.Id][i].Code].Count >= 5)
                        {
                            await invite_uses_after[guild.Id][i].DeleteAsync();
                            List<ulong> bannedUsers = counter[invite_uses_after[guild.Id][i].Code]!;
                            foreach (ulong bannedUser in bannedUsers)
                            {
                                SocketGuildUser userToBan = guild.GetUser(bannedUser)!;
                                await guild.AddBanAsync(userToBan, 7, "Flow protector is ON and is protecting you from token spams");
                                await guild.RemoveBanAsync(userToBan);
                            }
                        }
                    }
                }

                invite_uses_before[guild.Id].Clear();
                invite_uses_before[guild.Id].AddRange(invite_uses_after[guild.Id]);
            }
        }

        private async Task OnConnected()
        {
            // Join a specific guild or channel
            _client!.GetChannel(channelID);

            // Register additional event handlers
            await RegisterEvents();
            await _client.SetStatusAsync(UserStatus.Invisible);
            await SendMessageAsync(channelID, $"Activated.");
        }

        private async Task OnDisconnected(Exception e)
        {
            await UiForm!.WriteToLog(e.Message);
        }

        private Task GuildEventCompleted(SocketGuildEvent arg)
        {
            return Task.CompletedTask;
        }

        #endregion Events/Handlers

        private async Task Wait()
        {
            Waiting = true;
            while (Waiting)
            {
                await Task.Delay(10);
            }
        }

        public async Task<ulong> ConvertChannelStringToIdAsync(string channelString)
        {
            await Task.Delay(0);
            if (channelString == null) { }


            return (ulong)0;
        }

        public async Task<ulong> ConvertUserStringToIdAsync(string userString)
        {
            await Task.Delay(0);
            SocketUser? user = _client!.GetUser(userString, "");
            return user?.Id ?? 0;
        }

        public async Task SendMessageAsync(ulong channelID, string message)
        {
            if ((IMessageChannel)await _client!.GetChannelAsync(channelID) is IMessageChannel channel)
            {
                await channel.SendMessageAsync(message);
            }
        }

        public async Task DeleteMessageAsync(ulong channelID, ulong messageId) // Fix this
        {
            if (_client!.GetChannel(channelID) is IMessageChannel channel)
            {
                IMessage? message = await channel.GetMessageAsync(messageId);
                await message.DeleteAsync();
            }
        }

        public async Task BanUserAsync(ulong userId, int daysOfMessagesToDelete = 0, string reason = "")
        {
            SocketGuild guild = _client!.Guilds.FirstOrDefault()!;
            if (guild != null)
            {
                SocketGuildUser user = guild.GetUser(userId)!;
                await guild.AddBanAsync(user, daysOfMessagesToDelete, reason);
            }
        }

        public async Task KickUserAsync(ulong userId, string reason = "")
        {
            SocketGuild? guild = _client!.Guilds.FirstOrDefault();
            if (guild != null)
            {
                SocketGuildUser? user = guild.GetUser(userId);
                await user.KickAsync(reason);
            }
        }

        public async Task LeaveGuildAsync(ulong guildId)
        {
            SocketGuild? guild = _client!.GetGuild(guildId);
            await guild.LeaveAsync();
        }

        public async Task AddRoleAsync(ulong guildId, string roleName)
        {
            SocketGuild? guild = _client!.GetGuild(guildId);
            await guild.CreateRoleAsync(roleName);
        }

        public async Task DeleteRoleAsync(ulong guildId, ulong roleId)
        {
            SocketGuild guild = _client!.GetGuild(guildId)!;
            SocketRole role = guild.GetRole(roleId)!;
            await role.DeleteAsync();
        }

        public async Task AddMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
        {
            SocketGuild guild = _client!.GetGuild(guildId)!;
            SocketGuildUser user = guild.GetUser(userId)!;
            SocketRole role = guild.GetRole(roleId)!;
            await user.AddRoleAsync(role);
        }

        public async Task RemoveMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
        {
            SocketGuild guild = _client!.GetGuild(guildId);
            SocketGuildUser user = guild.GetUser(userId);
            SocketRole role = guild.GetRole(roleId);
            await user.RemoveRoleAsync(role);
        }

        public async Task DeleteInviteAsync(string inviteCode)
        {
            RestInviteMetadata invite = await _client!.GetInviteAsync(inviteCode)!;
            await invite.DeleteAsync();
        }

        public async Task BotDelayDelete(SocketMessage curMsg)
        {
            if (curMsg.Content.StartsWith(StreamIntro) || curMsg.Content.StartsWith(StreamMsgIntro))
            {
                return;
            }
            else
            {
                await Task.Delay(8000);
                try
                {
                    await curMsg.DeleteAsync();
                }
                catch (Exception e)
                {
                    await UiForm!.WriteToLog(e.Message);
                }
            }
            return;
        }

        public async Task EmbedImageMsg(SocketMessage curMsg, string msg, string imageUrl)
        {
            // Create a new embed object
            EmbedBuilder embed = new ();
            // Add an image to the embed
            embed.WithImageUrl(imageUrl);
            // Send the embed to the channel
            await curMsg.Channel.SendMessageAsync(msg, false, embed.Build());
        }

        public async Task<string> GetDiscordUsernameColor(ulong userId)
        {
            SocketUser user = _client!.GetUser(userId)!;
            SocketGuild guild = _client!.Guilds.FirstOrDefault(g => g.Users.Any(u => u.Id == user.Id))!;

            SocketGuildUser guildUser = guild!.GetUser(user.Id)!;
            SocketRole role = guildUser.Roles.OrderByDescending(r => r.Position).FirstOrDefault()!;
            await Task.Delay(0); // fake delay
            return role!.Color.ToString();
        }

        static async Task<string> GetCatUrl()
        {
            //API that returns random image of a cat
            string url = "https://api.thecatapi.com/v1/images/search";
            HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(url);
            string jsonString = await response.Content.ReadAsStringAsync()!;
            dynamic jsonObject = JsonConvert.DeserializeObject(jsonString)!;
            string retStr = jsonObject[0].url ?? "Url Failed";
            //await UiForm!.WriteToLog($"Cat url: {retStr}");
            return retStr;
        }

        // Save object to file
        public void WriteToXmlFile<T>(string filePath, object obj)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            if (obj == null)
            {
                //throw new ArgumentNullException("obj can't be null");
                return;
            }
            if (string.IsNullOrEmpty(filePath))
            {
                //throw new ArgumentNullException("filePath can't be null or empty");
                return;
            }

            TextWriter? stream = null;
            JsonSerializer serializer = new();
            try
            {
                stream = File.CreateText(filePath);
                serializer.Serialize(stream, obj);

                return;
            }
            catch (Exception)
            {

            }
            finally
            {
                stream?.Close();
            }
        }

        // Load object from file
        public T? ReadFromXmlFile<T>(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                //throw new ArgumentNullException("file null\notfound");
                return default;
            }

            JsonReader? stream = null;
            JsonSerializer serializer = new();
            try
            {
                stream = new JsonTextReader(new StreamReader(filePath));

                return (T?)serializer.Deserialize<T>(stream);
            }
            catch (Exception e)
            {
                Task.Run(async() => await UiForm!.WriteToLog(e.Message));
                return default;
            }
            finally
            {
                stream?.Close();
            }
        }

        public async Task ManageCommandFunctions(SocketMessage curMsg, ISocketMessageChannel channel)
        {
            if (curMsg.Content == null)
            {
                return;
            }
            if (channel != curMsg.Channel)
            {
                channel = curMsg.Channel;
            }
            // Ignore messages from bots
            if (curMsg.Author.IsBot)
            {
                await BotDelayDelete(curMsg);
                return;
            }
            if (!_ignoreChannels.Contains(curMsg.Channel.Id))
            {
                bool usrHasRole = true;
                // Check user roles herestring[] usrRoles;
                if (usrHasRole)
                {
                    await Task.Run(() => ChatMessageReceived?.Invoke(this, curMsg));
                    // Check if the message starts with a command prefix
                    if (curMsg.Content.StartsWith("!"))
                    {
                        List<string> command = curMsg.Content.Split(' ').ToList();
                        string cmd = command.First();
                        List<string> args = command.Skip(1).ToList();
                        try
                        {
                            await curMsg.Channel.DeleteMessageAsync(curMsg);
                        }
                        catch (Exception e)
                        {
                            await UiForm!.WriteToLog(e.Message);
                        }
                        try
                        {
                            switch (cmd.ToLower())
                            {

                                case "!help":
                                    string helpString = "Available commands:\n";
                                    helpString += "!HereKitty\n";
                                    helpString += "!RegisterStream {platform} {username}\n";
                                    helpString += "!DeregisterStream {username}\n";
                                    helpString += "!StreamChannel {channelID}\n";
                                    helpString += "!RemoveStreamChannel {channelID}\n";
                                    helpString += "!AddBotModChannel {channelID}\n";
                                    helpString += "!CheckBotModChannel {channelID}\n";
                                    helpString += "!RemoveBotModChannel {channelID}\n";
                                    helpString += "!SetWelcomeChannel {channelID}\n";
                                    helpString += "!IgnoreChannel {channelID}\n";
                                    helpString += "!ListenChannel {channelID}\n";
                                    helpString += "!Clear {Num}\n";
                                    helpString += "!BanWord {word}\n";
                                    helpString += "!UnbanWord {word}\n";
                                    helpString += "!CheckStrikes {userid}\n";
                                    helpString += "!ClearAllData\n";
                                    await curMsg.Channel.SendMessageAsync(helpString);
                                    break;
                                case "!setwelcomechannel":
                                    await SetWelcomeChannel(curMsg, args);
                                    break;
                                case "!registerstream":
                                    await RegisterStream(curMsg, args);
                                    break;
                                case "!deregisterstream":
                                    await DeregisterStream(curMsg, args);
                                    break;
                                case "!streamchannel":
                                    await StreamChannel(curMsg, args);
                                    break;
                                case "!removestreamchannel":
                                    await RemoveStreamChannel(curMsg, args);
                                    break;
                                case "!addbotmodchannel":
                                    await AddBotModChannel(curMsg, args);
                                    break;
                                case "!checkbotmodchannel":
                                    await CheckBotModChannel(curMsg, args);
                                    break;
                                case "!removebotmodchannel":
                                    await RemoveBotModChannel(curMsg, args);
                                    break;
                                case "!ignorechannel":
                                    await IgnoreChannel(curMsg, args);
                                    break;
                                case "!listenchannel":
                                    await ListenChannel(curMsg, args);
                                    break;
                                case "!banword":
                                    await BanWord(curMsg, args);
                                    break;
                                case "!unbanword":
                                    await UnbanWord(curMsg, args);
                                    break;
                                case "!checkstrikes":
                                    await CheckStrikes(curMsg, args);
                                    break;
                                case "!clear":
                                    Clear(curMsg, args);
                                    break;
                                case "!nuke":
                                    await Nuke(curMsg);
                                    break;
                                case "!clearalldata":
                                    ClearAllData(curMsg);
                                    break;
                                case "!herekitty":
                                    await HereKitty(curMsg);
                                    break;
                                default:
                                    await curMsg.Channel.SendMessageAsync("Invalid command.");
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            await UiForm!.WriteToLog(e.Message);
                        }
                        GC.Collect();
                        return;
                    }
                }
                // Check if the message is in a channel that the bot is moderating
                if (_modChannels.Contains(curMsg.Channel.Id))
                {

                }
            }
            Task.WaitAll();
        }

        // Command Functions
        public void Backup()
        {
            WriteToXmlFile<Dictionary<string, string>>("streamers.dat", _streamers);
            WriteToXmlFile<List<ulong>>("streamChannels.dat", _streamChannels);
            WriteToXmlFile<List<ulong>>("ignorechannels.dat", _ignoreChannels);
            WriteToXmlFile<List<ulong>>("modChannels.dat", _modChannels);
            WriteToXmlFile<HashSet<string>>("bannedWords.dat", _bannedWords);
            WriteToXmlFile<Dictionary<ulong, int>>("userStrikes.dat", _userStrikes);
            WriteToXmlFile<ulong>("welcomeChannel.dat", WelcomeChannel);

            //await UiForm!.WriteToLog("Data backed up");
        }

        public void Restore()
        {
            if (File.Exists("streamers.dat"))
            {
                //await UiForm!.WriteToLog("Restoring streamers");
                Dictionary<string, string>? dat = ReadFromXmlFile<Dictionary<string, string>>("streamers.dat");
                if (dat != null)
                {
                    _streamers = dat;
                }
            }
            if (File.Exists("streamChannels.dat"))
            {
                //await UiForm!.WriteToLog("Restoring streamer channels");
                List<ulong>? dat = ReadFromXmlFile<List<ulong>>("streamChannels.dat");
                if (dat != null)
                {
                    _streamChannels = dat;
                }
            }
            if (File.Exists("modChannels.dat"))
            {
                //await UiForm!.WriteToLog("Restoring mod channels");
                List<ulong>? dat = ReadFromXmlFile<List<ulong>>("modChannels.dat");
                if (dat != null)
                {
                    _modChannels = dat;
                }
            }
            if (File.Exists("bannedWords.dat"))
            {
                //await UiForm!.WriteToLog("Restoring banned words");
                HashSet<string>? dat = ReadFromXmlFile<HashSet<string>>("bannedWords.dat");
                if (dat != null)
                {
                    _bannedWords = dat;
                }
            }
            if (File.Exists("userStrikes.dat"))
            {
                //await UiForm!.WriteToLog("Restoring user strikes");
                Dictionary<ulong, int>? dat = ReadFromXmlFile<Dictionary<ulong, int>>("userStrikes.dat");
                if (dat != null)
                {
                    _userStrikes = dat;
                }
            }
            if (File.Exists("welcomeChannel.dat"))
            {
                //await UiForm!.WriteToLog("Restoring welcome channel");
                ulong? dat = ReadFromXmlFile<ulong>("welcomeChannel.dat");
                if (dat != null)
                {
                    WelcomeChannel = (ulong)dat;
                }
            }
            if (File.Exists("ignorechannels.dat"))
            {
                //await UiForm!.WriteToLog("Restoring ignored channels");
                List<ulong>? dat = ReadFromXmlFile<List<ulong>>("ignorechannels.dat");
                if (dat != null)
                {
                    _ignoreChannels = dat;
                }
            }
            if (File.Exists(wordListName))
            {
                string wordsAdded = "Importing words from word list: ";
                //await UiForm!.WriteToLog("Importing banned words");
                StreamReader Reader = File.OpenText(wordListName);
                string listTxt = Reader.ReadToEnd();
                string[] wordList = listTxt.Split(',');
                foreach (string word in wordList)
                {
                    if (!_bannedWords.Contains(word.ToLower()))
                    {
                        _bannedWords.Add(word.ToLower());
                        wordsAdded += word.ToLower() + " ";
                    }
                }
                //await UiForm!.WriteToLog(wordsAdded);
            }

            //await UiForm!.WriteToLog("Data restored");
        }

        public async Task RegisterStream(SocketMessage curMsg, List<string> args)
        {
            if (args.Count != 2)
            {
                await curMsg.Channel.SendMessageAsync("Invalid format. Use !RegisterStream {platform} {username}");
                return;
            }
            _streamers.Add(args[1], args[0]);
            await curMsg.Channel.SendMessageAsync($"{args[1]} is now registered on {args[0]}.");
        }

        public async Task DeregisterStream(SocketMessage curMsg, List<string> args)
        {
            if (args.Count != 1)
            {
                await curMsg.Channel.SendMessageAsync("Invalid format. Use !DeregisterStream {username}");
                return;
            }
            if (_streamers.Remove(args[0]))
            {
                await curMsg.Channel.SendMessageAsync($"{args[0]} is now deregistered.");
            }
            else
            {
                await curMsg.Channel.SendMessageAsync($"{args[0]} is not registered.");
            }
        }

        public async Task StreamChannel(SocketMessage curMsg, List<string> args)
        {
            if (args.Count != 1)
            {
                await curMsg.Channel.SendMessageAsync("Invalid format. Use !StreamChannel {channelID}");
                return;
            }
            if (ulong.TryParse(args[0], out channelID))
            {
                _streamChannels.Add(channelID);
                await curMsg.Channel.SendMessageAsync($"Channel {channelID} has been added to the stream post list.");
            }
            else
            {
                await curMsg.Channel.SendMessageAsync("Invalid channel id.");
            }
        }

        public async Task RemoveStreamChannel(SocketMessage curMsg, List<string> args)
        {
            if (args.Count != 1)
            {
                await curMsg.Channel.SendMessageAsync("Invalid format. Use !RemoveStreamChannel {channelID}");
                return;
            }
            if (ulong.TryParse(args[0], out channelID))
            {
                if (_streamChannels.Remove(channelID))
                {
                    await curMsg.Channel.SendMessageAsync($"Channel {channelID} has been removed from the stream post list.");
                }
                else
                {
                    await curMsg.Channel.SendMessageAsync($"Channel {channelID} is not in the stream post list.");
                }
            }
            else
            {
                await curMsg.Channel.SendMessageAsync("Invalid channel id.");
            }
        }

        public async Task AddBotModChannel(SocketMessage curMsg, List<string> args)
        {
            if (args.Count != 1)
            {
                await curMsg.Channel.SendMessageAsync("Invalid format. Use !AddBotModChannel {channelID}");
                return;
            }
            if (ulong.TryParse(args[0], out channelID))
            {
                if (!_modChannels.Contains(channelID))
                {
                    _modChannels.Add(channelID);
                    await curMsg.Channel.SendMessageAsync($"Channel id {channelID} has been added to the bot moderation list.");
                }
                else
                {
                    await curMsg.Channel.SendMessageAsync($"Channel id {channelID} has already been added to the bot moderation list.");
                }
            }
            else
            {
                await curMsg.Channel.SendMessageAsync("Invalid channel id.");
            }
        }

        public async Task CheckBotModChannel(SocketMessage curMsg, List<string> args)
        {
            if (args.Count != 1)
            {
                await curMsg.Channel.SendMessageAsync("Invalid format. Use !AddBotModChannel {channelID}");
                return;
            }
            if (ulong.TryParse(args[0], out channelID))
            {
                if (!_modChannels.Contains(channelID))
                {
                    await curMsg.Channel.SendMessageAsync($"Channel id {channelID} is in the bot moderation list.");
                }
                else
                {
                    await curMsg.Channel.SendMessageAsync($"Channel id {channelID} is not in the bot moderation list.");
                }
            }
            else
            {
                await curMsg.Channel.SendMessageAsync("Invalid channel id.");
            }
        }

        public async Task RemoveBotModChannel(SocketMessage curMsg, List<string> args)
        {
            if (args.Count != 1)
            {
                await curMsg.Channel.SendMessageAsync("Invalid format. Use !RemoveBotModChannel {channelID}");
                return;
            }
            if (ulong.TryParse(args[0], out channelID))
            {
                if (_modChannels.Remove(channelID))
                {
                    await curMsg.Channel.SendMessageAsync($"Channel {channelID} has been removed from the bot moderation list.");
                }
                else
                {
                    await curMsg.Channel.SendMessageAsync($"Channel {channelID} is not in the bot moderation list.");
                }
            }
        }

        public async Task SetWelcomeChannel(SocketMessage curMsg, List<string> args)
        {
            if (ulong.TryParse(args[0], out channelID))
            {
                WelcomeChannel = channelID;
                await curMsg.Channel.SendMessageAsync($"Channel {channelID} has been set as the Welcome channel.");
            }

            else
            {
                await curMsg.Channel.SendMessageAsync("Invalid channel id.");
            }
        }

        public async Task IgnoreChannel(SocketMessage curMsg, List<string> args)
        {
            if (true)
            {
                if (args.Count != 1)
                {
                    await curMsg.Channel.SendMessageAsync("Invalid format. Use !ignorechannel {channelID}");
                    return;
                }
                if (ulong.TryParse(args[0], out channelID))
                {
                    _ignoreChannels.Add(channelID);
                    await curMsg.Channel.SendMessageAsync($"Channel id {channelID} has been added to the bot ignore list.");
                }
                else
                {
                    await curMsg.Channel.SendMessageAsync("Invalid channel id.");
                }
            }
        }

        public async Task ListenChannel(SocketMessage curMsg, List<string> args)
        {
            if (args.Count != 1)
            {
                await curMsg.Channel.SendMessageAsync("Invalid format. Use !listenchannel {channelID}");
                return;
            }
            if (ulong.TryParse(args[0], out channelID))
            {
                if (_ignoreChannels.Remove(channelID))
                {
                    await curMsg.Channel.SendMessageAsync($"Channel {channelID} has been removed from the bot moderation list.");
                }
                else
                {
                    await curMsg.Channel.SendMessageAsync($"Channel {channelID} is not in the bot moderation list.");
                }
            }
        }

        public async static void Clear(SocketMessage curMsg, List<string> args)
        {
            try
            {
                if (args.Count == 1)
                {
                    List<ulong> msgIdList = new();
                    if (int.TryParse(args[0], out int amount))
                    {
                        IEnumerable<IMessage> messages = curMsg.Channel.GetMessagesAsync(amount + 1).FlattenAsync().Result;
                        foreach (IMessage msg in messages)
                        {
                            if (!msg.IsPinned)
                            {
                                await curMsg.Channel.DeleteMessageAsync(msg.Id);
                                await Task.Delay(1000);
                            }
                        }

                        await curMsg.Channel.SendMessageAsync($"{curMsg.Author} has deleted {amount} messages");
                    }
                    else
                    {
                        await curMsg.Channel.SendMessageAsync("Usage: !Clear {num}.");
                    }
                }
                else
                {
                    await curMsg.Channel.SendMessageAsync("Usage: !Clear {num}.");
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("10008"))
                {
                    Clear(curMsg, args);
                }
                else
                {
                    await UiForm!.WriteToLog(e.Message);
                }
            }
        }

        private async Task Nuke(SocketMessage curMsg)
        {
            ISocketMessageChannel channel = curMsg.Channel;
            bool isChannelEmpty = false;
            IEnumerable<IMessage> messages = curMsg.Channel.GetMessagesAsync().FlattenAsync().Result;
            while (isChannelEmpty)
            {
                foreach (IMessage message in messages)
                {
                    await channel.DeleteMessageAsync(message);
                }
                messages = curMsg.Channel.GetMessagesAsync().FlattenAsync().Result;
                if (!messages.Any())
                {
                    isChannelEmpty = true;
                }
            }

            await curMsg.Channel.SendMessageAsync("Messages deleted");
        }

        public async Task HereKitty(SocketMessage curMsg)
        {
            string caturl = GetCatUrl().Result;
            await EmbedImageMsg(curMsg, "Here is a kitty.", caturl);

        }

        public async Task BanWord(SocketMessage curMsg, List<string> args)
        {
            if (args.Count < 1)
            {
                await curMsg.Channel.SendMessageAsync("Invalid format. Use !BanWord {word}");
                return;
            }
            else
            {
                foreach (string arg in args)
                {
                    if (!_bannedWords.Contains(arg))
                    {
                        _bannedWords.Add(arg);
                        await curMsg.Channel.SendMessageAsync($"The word '{arg}' has been added to the banned word list.");
                    }
                    else
                    {
                        await curMsg.Channel.SendMessageAsync($"The word '{arg}' has already been added to the banned word list.");
                    }
                }
            }
        }

        public async Task UnbanWord(SocketMessage curMsg, List<string> args)
        {
            if (args.Count < 1)
            {
                await curMsg.Channel.SendMessageAsync("Invalid format. Use !UnbanWord {word}");
                return;
            }
            else
            {
                foreach (string arg in args)
                {
                    if (_bannedWords.Remove(arg))
                    {
                        await curMsg.Channel.SendMessageAsync($"The word '{arg}' has been removed from the banned word list.");
                    }
                    else
                    {
                        await curMsg.Channel.SendMessageAsync($"The word '{arg}' is not in the banned word list.");
                    }
                }
            }
        }

        public async Task CheckStrikes(SocketMessage curMsg, List<string> args)
        {
            if (args.Count != 1)
            {
                await curMsg.Channel.SendMessageAsync("Invalid format. Use !CheckStrikes {userid}");
                return;
            }
            if (ulong.TryParse(args[0], out ulong userId))
            {
                if (_userStrikes.TryGetValue(userId, out int value))
                {
                    await curMsg.Channel.SendMessageAsync($"User {userId} has {value} strikes.");
                }
                else
                {
                    await curMsg.Channel.SendMessageAsync($"User {userId} has no strikes.");
                }
            }
            else
            {
                await curMsg.Channel.SendMessageAsync("Invalid user id.");
            }
        }

        public async void ClearAllData(SocketMessage curMsg) // Fix this
        {
            _streamers = new();
            _streamChannels = new();
            _modChannels = new();
            _bannedWords = new();
            _userStrikes = new();
            WelcomeChannel = new();

            await curMsg.Channel.SendMessageAsync("Data has been cleared.");
            //await UiForm!.WriteToLog("Data has been cleared.");
        }

        private Task WriteLog(LogMessage Log)
        {
            lock (_LogFileLock)
            {
                StreamWriter LogFile = File.AppendText("DiscordLog.txt");
                LogFile.WriteLine($"{Log.Source}: {Log.Message}");
                LogFile.Flush();
                LogFile.Close();
            }
            return Task.CompletedTask;
        }


        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

        // End Commands 

/* Examples 
                await channel.TriggerTypingAsync();
*/
}
}
