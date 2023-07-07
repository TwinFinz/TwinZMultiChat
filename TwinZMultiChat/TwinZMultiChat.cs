using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text.Json.Nodes;
using TwinZMultiChat.Utilities;
using Discord.WebSocket;
using Discord;
using TwitchLib.Client;
using TwitchLib.Api.Helix;
using TwitchLib.Client.Events;
using System.Net.Http.Headers;
using Newtonsoft.Json.Serialization;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.Client.Models;
using Google.Apis.Util;
using System.Threading.Tasks;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection;
using System.Security.AccessControl;

namespace TwinZMultiChat
{
    public partial class MainForm : Form
    {
        #region Variables
        private static string DiscordBotToken = "";
        private static ulong DiscordChannelID = 0;
        private static string YouTubeApplicationName = "";
        private static string TwitchClientID = "";
        private static string TwitchClientSecret = "";
        private static string TwitchUserName = "";
        private static bool EnableDiscord = false;
        private static bool EnableYouTube = false;
        private static bool EnableTwitch = false;
        private static bool EnableKick = false;
        private readonly static string DefaultColor = "rgb(100,255,255)";
        //private const string BotUsername = "TwinZMultiChat";

        private readonly static object chatOverlayFileLock = new();
        private readonly static List<OverlayMsg> chatMessages = new();

        private static MyDiscordAPI? discordBot;
        private static MyTwitchAPI? twitchBot;
        private static MyYoutubeAPI? youTubeBot;

        #endregion Variables
        
        #region Init
        public MainForm() // Fix Loading Saved Variables
        {
            InitializeComponent();
            LogBox.AppendText("Init Successful.\n");
            LoadSavedVariables();
            discordBot = new(this, DiscordBotToken, DiscordChannelID);
            twitchBot = new(this, TwitchClientID, TwitchClientSecret);
            youTubeBot = new(this, YouTubeApplicationName, discordBot!.StreamMsgIntro);
        }

        private async void LoadSavedVariables()
        {
            if (File.Exists("Config.xml"))
            {
                StreamReader CFG = File.OpenText("Config.xml");
                string cfgInput = CFG.ReadToEnd();
                string[] Configs = cfgInput.Split("|||");
                if (!string.IsNullOrWhiteSpace(Configs[0]))
                {
                    DiscordBotToken = Configs[0].Replace("\"", "");
                    DiscordBotTokenBox.Text = DiscordBotToken;
                }
                if (ulong.TryParse(Configs[1], out ulong j))
                {
                    DiscordChannelID = j;
                    DiscordChannelIDBox.Text = DiscordChannelID.ToString();
                }
                if (!string.IsNullOrWhiteSpace(Configs[2]))
                {
                    TwitchUserName = Configs[2].Replace("\"", "");
                    TwitchBotNameBox.Text = TwitchUserName;
                }
                if (!string.IsNullOrWhiteSpace(Configs[3]))
                {
                    TwitchClientID = Configs[3].Replace("\"", "");
                    TwitchClientIDBox.Text = TwitchClientID;
                }
                if (!string.IsNullOrWhiteSpace(Configs[4]))
                {
                    TwitchClientSecret = Configs[4].Replace("\"", "");
                    TwitchClientSecretBox.Text = TwitchClientSecret;
                }
                if (!string.IsNullOrWhiteSpace(Configs[5]))
                {
                    YouTubeApplicationName = Configs[5].Replace("\"", "");
                    YouTubeApplicationNameBox.Text = YouTubeApplicationName;
                }
                CFG.Close();
                await WriteToLog("Loaded Successfully.\n");
            }
            else
            {
                await WriteToLog("Config Not Found.\n");
            }
        }
        #endregion Init

        #region BtnClicks
        private async void SaveBtn_Click(object sender, EventArgs e)
        {
            string JsonOutput = "";
            JsonOutput += JsonConvert.SerializeObject(DiscordBotToken) + "|||";
            JsonOutput += JsonConvert.SerializeObject(DiscordChannelID) + "|||";
            JsonOutput += JsonConvert.SerializeObject(TwitchUserName) + "|||";
            JsonOutput += JsonConvert.SerializeObject(TwitchClientID) + "|||";
            JsonOutput += JsonConvert.SerializeObject(TwitchClientSecret) + "|||";
            JsonOutput += JsonConvert.SerializeObject(YouTubeApplicationName) + "|||";
            File.WriteAllText("Config.xml", JsonOutput);
            await WriteToLog("Save Successfully.\n\n");
            await Task.Delay(0); // Fake Await
        }

        private async void ResetBtn_Click(object sender, EventArgs e)
        {
            DiscordBotTokenBox.Text = "";
            DiscordChannelIDBox.Text = "";
            TwitchBotNameBox.Text = "";
            TwitchClientIDBox.Text = "";
            TwitchClientSecretBox.Text = "";
            YouTubeApplicationNameBox.Text = "";
            File.Delete("Config.xml\n");
            await WriteToLog("Reset Successfully.\n\n");

        }

        private async void StartBtn_Click(object sender, EventArgs e)
        {
            try
            {
                await StartAsync();
            }
            catch (Exception ex)
            {
                await WriteToLog(ex.Message);
            }
            await WriteToLog("Started Sync\n");
        }

        private async void StopBtn_Click(object sender, EventArgs e)
        {
            await Task.Delay(50);
            await StopAsync();
            await WriteToLog("Stopped Sync\n");
        }
        #endregion BtnClicks

        #region MessageUtils
        public async Task StartAsync()
        {
            if (EnableTwitch) // Connect Twitch
            {
                twitchBot = new(this, TwitchClientID, TwitchClientSecret);
                await WriteToLog("Connecting to Twitch.\n");
                await twitchBot!.ConnectAsync();
                twitchBot!.ChatMessageReceived += SyncMessageTwitch;
                await WriteToLog("Success.\n");
            }
            if (EnableDiscord) // Connect Discord
            {
                discordBot = new(this, DiscordBotToken, DiscordChannelID);
                await WriteToLog("Connecting to Discord.\n");
                await discordBot!.ConnectAsync();
                await WriteToLog("Success.\n");
                discordBot.ChatMessageReceived += SyncMessageDiscord;
            }
            
            if (EnableYouTube) // Connect YouTube
            {
                youTubeBot = new(this, YouTubeApplicationName, discordBot!.StreamMsgIntro);
                await WriteToLog("Connecting to YouTube.\n");
                await youTubeBot!.ConnectAsync();
                youTubeBot!.ChatMessageReceived += SyncMessageYouTube;
                await youTubeBot!.SendLiveChatMessage($"{YouTubeApplicationName}: Active");
                await WriteToLog("Success.\n");

            }
        }

        public async Task StopAsync()
        {
            if (EnableDiscord) // Connect Discord
            {
                await WriteToLog("Disconnecting From Discord.\n");
                discordBot!.ChatMessageReceived -= SyncMessageDiscord;
                await discordBot!.DisconnectAsync();
                await WriteToLog("Success.\n");
            }
            if (EnableYouTube) // Connect YouTube
            {
                await WriteToLog("Disconnecting From YouTube.\n");
                youTubeBot!.ChatMessageReceived -= SyncMessageYouTube;
                await youTubeBot!.DisconnectAsync();
                await WriteToLog("Success.\n");

            }
            if (EnableTwitch) // Connect Twitch
            {
                await WriteToLog("Disconnecting From Twitch.\n");
                twitchBot!.ChatMessageReceived -= SyncMessageTwitch;
                await twitchBot!.DisconnectAsync();
                await WriteToLog("Success.\n");
            }
        }

        public async void SyncMessageDiscord(object? sender, SocketMessage message)
        {
            await WriteToLog($"MessageReceived| Discord> {message.Author}: {message}\n");
            string Color = await discordBot!.GetDiscordUsernameColor(message.Author.Id) ?? DefaultColor;
            OverlayMsg msg = new()
            {
                Platform = "discord",
                User = message.Author.Username,
                UserColor = Color,
                Message = message.Content
            };
            AddMessageToChatOverlay(msg); 
            if (EnableYouTube)
            {
                await youTubeBot!.SendLiveChatMessage($"{discordBot!.StreamMsgIntro}{message.Author.Username}: {message.Content}");
            }
            if (EnableTwitch)
            {
                await twitchBot!.SendMessage($"{discordBot!.StreamMsgIntro}{message.Author.Username}: {message.Content}");
            }
            await Task.Delay(0); // FakeDelay
        }

        public async void SyncMessageYouTube(object? sender, MyYoutubeAPI.ChatMessageEventArgs YTMessage)
        {
            await WriteToLog($"MessageReceived| YouTube> {YTMessage.Username}: {YTMessage.Message}\n");
            OverlayMsg msg = new()
            {
                Platform = "youtube",
                User = YTMessage.Username,
                UserColor = DefaultColor,
                Message = YTMessage.Message
            };
            AddMessageToChatOverlay(msg);
            if (EnableDiscord)
            {
                await discordBot!.SendMessageAsync(DiscordChannelID, $"{discordBot!.StreamMsgIntro}{YTMessage.Username}: {YTMessage.Message}");
            }
            if (EnableTwitch)
            {
                await twitchBot!.SendMessage($"{discordBot!.StreamMsgIntro}{YTMessage.Username}: {YTMessage.Message}");
            }
            await Task.Delay(0); // FakeDelay
        }

        public async void SyncMessageTwitch(object? sender, OnMessageReceivedArgs TwitchMsg)
        {
            await WriteToLog($"MessageReceived| Twitch> {TwitchMsg.ChatMessage.Username}: {TwitchMsg.ChatMessage.Message}\n");
            OverlayMsg msg = new()
            {
                Platform = "twitch",
                User = TwitchMsg.ChatMessage.Username,
                UserColor = TwitchMsg.ChatMessage.ColorHex ?? string.Empty,
                Message = TwitchMsg.ChatMessage.Message
            };
            AddMessageToChatOverlay(msg);
            if (EnableDiscord)
            {
                await discordBot!.SendMessageAsync(DiscordChannelID, $"{discordBot!.StreamMsgIntro}{TwitchMsg.ChatMessage.Username}: {TwitchMsg.ChatMessage.Message}");
            }
            if (EnableYouTube)
            {
                await youTubeBot!.SendLiveChatMessage($"{discordBot!.StreamMsgIntro}{TwitchMsg.ChatMessage.Username}: {TwitchMsg.ChatMessage.Message}");
            }

            await Task.Delay(0); // FakeDelay
        }

        
        #endregion

        #region UiElementUpdates
        private void DiscordBotBoxChanged(object sender, EventArgs e)
        {
            DiscordBotToken = DiscordBotTokenBox.Text ?? "";

        }

        private void DiscordChannelIDBoxChanged(object sender, EventArgs e) // Fix Parse Error
        {
            try
            {
                DiscordChannelID = ulong.Parse(DiscordChannelIDBox.Text);
            }
            catch (Exception)
            {

            }

        }

        private void TwitchBotNameBoxChanged(object sender, EventArgs e)
        {
            TwitchUserName = TwitchBotNameBox.Text ?? "";
        }

        private void TwitchClientIDBoxChanged(object sender, EventArgs e)
        {
            TwitchClientID = TwitchClientIDBox.Text ?? "";

        }

        private void TwitchClientSecretBoxChanged(object sender, EventArgs e)
        {
            TwitchClientSecret = TwitchClientSecretBox.Text ?? "";
        }

        private void YouTubeApplicationNameBoxChanged(object sender, EventArgs e)
        {
            YouTubeApplicationName = YouTubeApplicationNameBox.Text ?? "";
        }

        internal async Task WriteToLog(string Message)
        {
            if (InvokeRequired)
            {
                InvokeUI(async () =>
                {
                    await WriteToLog(Message);
                });
            }
            else
            {
                LogBox.AppendText(Message);
                LogBox.ScrollToCaret();
            }
            await Task.Delay(0); // Fake Delay
        }

        internal void InvokeUI(Action a)
        {
            this.BeginInvoke(new MethodInvoker(a));
        }

        private void DiscordCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            EnableDiscord = DiscordCheckBox.Checked;
        }

        private void YouTubeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            EnableYouTube = YouTubeCheckBox.Checked;
        }

        private void TwitchCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            EnableTwitch = TwitchCheckBox.Checked;
        }
        private void KickCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            EnableKick = KickCheckBox.Checked;
        }

        private static Task<string> MessageBoxWithInput(string promptMessage, string title)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(promptMessage, title, "") ?? string.Empty;
            if (input == string.Empty)
            {
                throw new Exception("User Canceled Authorization\n");
            }

            return Task.FromResult(input);
        }

        #endregion UiElementUpdates

        #region OBS Overlay
        public static string GenerateChatOverlay(List<OverlayMsg> chatMessages, int refreshIntervalInSeconds)
        {
            StringBuilder sb = new();
            sb.AppendLine(@"<style>
                    body {
                        background-color: rgba(0, 0, 0, 0.85);
                        color: white;
                        font-family: Arial, sans-serif;
                        font-size: 18px;
                        width: 610px;
                        height: 410px;
                        opacity: 0.85;
                        position: fixed;
                        top: 0;
                        left: 0;
                        overflow-y: auto;
                        padding-right: 10px;
                    }

                    .chat-message {
                        margin-bottom: 10px;
                        background-color: #1a1a1a;
                        padding: 5px; /* Adjust the padding value to make the border smaller */
                        border-radius: 5px;
                        box-shadow: 0 2px 5px rgba(0, 0, 0, 0.5);
                    }

                    .chat-message-inner {
                        display: flex;
                        align-items: center;
                        gap: 10px;
                    }

                    .chat-message-avatar {
                        width: 45px;
                        height: 45px;
                        border-radius: 50%;
                        overflow: hidden;
                        flex-shrink: 0;
                    }

                    .chat-message-avatar img {
                        width: 100%;
                        height: 100%;
                        object-fit: cover;
                    }

                    .chat-message-content {
                        flex-grow: 1;
                        word-wrap: break-word;
                    }

                    .user-color {
                        color: inherit;
                    }
                </style>");

                        sb.AppendLine("<body>");
                        foreach (OverlayMsg messageEntry in chatMessages)
                        {
                            string platform = messageEntry.Platform;
                            string message = messageEntry.Message;
                            string user = messageEntry.User;
                            string userColor = messageEntry.UserColor;
                            string platformIconPath = GetPlatformIconPath(platform.ToLower());

                            sb.AppendLine($@"    <div class=""chat-message"">
                    <div class=""chat-message-inner"">
                        <div class=""chat-message-avatar"">
                            <img src=""{platformIconPath}"" alt=""{platform}"" />
                        </div>
                        <div class=""chat-message-content""><span class=""user-color"" style=""color: {userColor}"">{user}</span>: {message}</div>
                    </div>
                </div>");
                        }

                        sb.AppendLine("</body>");
                        sb.AppendLine($@"<script>
                setTimeout(function() {{
                    location.reload();
                }}, {refreshIntervalInSeconds * 1000});
            </script>");
            return sb.ToString();
        }

        private static string GetPlatformIconPath(string platform)
        {
            // Modify this method to return the file path or URL of the corresponding platform icon
            return platform switch
            {
                "twitch" => "twitch.png",
                "youtube" => "youtube.png",
                "discord" => "discord.png",
                _ => string.Empty,// Return an empty string or default icon path if platform is not recognized
            };
        }

        public static void AddMessageToChatOverlay(OverlayMsg message)
        {
            chatMessages.Add(message);
            if (chatMessages.Count > 6)
            {
                chatMessages.RemoveAt(0);
            }

            string overlayHtml = GenerateChatOverlay(chatMessages, 1);

            lock (chatOverlayFileLock)
            {
                File.WriteAllText("ChatOverlay.html", overlayHtml);
            }
        }

        public class OverlayMsg
        {
            [JsonProperty("platform")]
            public string Platform { get; set; } = string.Empty;
            [JsonProperty("user")]
            public string User { get; set; } = string.Empty;
            [JsonProperty("user-color")]
            public string UserColor { get; set; } = string.Empty; // Color format: Hexadecimal, RGB, RGBA, etc.
            [JsonProperty("message")]
            public string Message { get; set; } = string.Empty;
        }
    }

    #endregion
}
