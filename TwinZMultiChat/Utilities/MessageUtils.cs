using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace TwinZMultiChat.Utilities
{
    internal class MessageUtils
    {
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0044 // Remove unused private members
        private static string DiscordBotToken = "";
        private static ulong DiscordChannelID = 0;
        private static string YouTubeOauthToken = "";
        private static string YouTubeclientID = "";
        private static string YouTubeClientSecret = "";
        private static string TwitchOauthToken = "";
        private static string TwitchClientID = "";
        private static string TwitchClientSecret = "";

        private TwitchLib.Api.Helix.Helix? _helix;
        private TwitchClient? _client;
        public event EventHandler<OnMessageReceivedArgs>? MessageReceived;

        readonly static string BOTUSERNAME = "SyncBot";

#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore IDE0044 // Remove unused private members

        public class MessageSyncService
        {
            private readonly TwitchClient twitchClient;
            private readonly DiscordSocketClient discordClient;
            private readonly YouTubeService youtubeService;
            private readonly List<string> syncedMessages;

            public MessageSyncService(string twitchUsername, string twitchAccessToken, string discordToken, string youtubeApiKey)
            {
                syncedMessages = new List<string>();

                // Initialize Twitch client
                twitchClient = new TwitchClient();
                twitchClient.Initialize(new ConnectionCredentials(twitchUsername, twitchAccessToken));

                // Initialize Discord client
                discordClient = new DiscordSocketClient();
                discordClient.Log += LogAsync;
                discordClient.Ready += () => Task.Run(() => discordClient.SetGameAsync("Syncing Messages"));

                // Initialize YouTube service
                youtubeService = new YouTubeService(new BaseClientService.Initializer
                {
                    ApiKey = youtubeApiKey,
                    ApplicationName = "MessageSync"
                });
            }

            public async Task StartAsync()
            {
                // Connect to Twitch
                twitchClient.OnConnected += TwitchConnected;
                twitchClient.OnMessageReceived += TwitchMessageReceived;
                twitchClient.Connect();

                // Connect to Discord
                await discordClient.LoginAsync(TokenType.Bot, DiscordBotToken);
                await discordClient.StartAsync();
                discordClient.MessageReceived += DiscordMessageReceived;

                // Connect to YouTube
                await SyncYouTubeChat();
            }

            public async Task StopAsync()
            {
                // Stop syncing messages
                twitchClient.Disconnect();
                await discordClient.StopAsync();
                await discordClient.LogoutAsync();
                twitchClient.OnConnected -= TwitchConnected;
                twitchClient.OnMessageReceived -= TwitchMessageReceived;
                discordClient.MessageReceived -= DiscordMessageReceived;
            }

            private async Task SyncYouTubeChat()
            {
                var liveStreams = await GetLiveYouTubeStreams();
                foreach (var stream in liveStreams)
                {
                    var chatId = await GetYouTubeLiveChatId(stream.Id);
                    if (!string.IsNullOrEmpty(chatId))
                    {
                        var chatSocket = youtubeService.LiveChatMessages.List(chatId, "snippet");
                        var chatPoll = chatSocket.ExecuteAsStream();
                        var chatStreamReader = new System.IO.StreamReader(chatPoll);
                        while (!chatStreamReader.EndOfStream)
                        {
                            var line = chatStreamReader.ReadLine();
                            // Parse and process the YouTube chat message here
                            // You can extract the username and message content from the line
                            // and sync it to other platforms using the SyncMessageToTwitch and SyncMessageToDiscord methods
                        }
                    }
                }
                await Task.Delay(0); // Fake Await
            }

            private async Task<IEnumerable<Google.Apis.YouTube.v3.Data.LiveBroadcast>> GetLiveYouTubeStreams()
            {
                var liveBroadcastsRequest = youtubeService.LiveBroadcasts.List("snippet");
                var liveBroadcastsResponse = await liveBroadcastsRequest.ExecuteAsync();
                await Task.Delay(0); // Fake Await
                return liveBroadcastsResponse.Items;
            }

            private async Task<string?> GetYouTubeLiveChatId(string broadcastId)
            {
                var liveStreamsRequest = youtubeService.LiveBroadcasts.List("snippet");
                liveStreamsRequest.Id = broadcastId;
                var liveStreamsResponse = await liveStreamsRequest.ExecuteAsync();
                var liveStream = liveStreamsResponse.Items.FirstOrDefault();
                await Task.Delay(0); // Fake Await
                return liveStream?.Snippet.LiveChatId;
            }

            private async void TwitchConnected(object? sender, OnConnectedArgs e)
            {
                twitchClient.JoinChannel(BOTUSERNAME);
                // $"Connected to Twitch as {e.BotUsername}";
                await Task.Delay(0); // Fake Await
            }

            private async void TwitchMessageReceived(object? sender, OnMessageReceivedArgs e)
            {
                if (IsMessageSynced(e.ChatMessage.Message))
                    return;

                if (ModerationCheck(e.ChatMessage.Message))
                    return;

                SyncMessageToDiscord(e.ChatMessage.Username, e.ChatMessage.Message);
                SyncMessageToYouTube(e.ChatMessage.Username, e.ChatMessage.Message);
                syncedMessages.Add(e.ChatMessage.Message);
                await Task.Delay(0); // Fake Await
            }

            private async Task DiscordMessageReceived(SocketMessage message)
            {
                if (message.Author.IsBot)
                    return;

                if (IsMessageSynced(message.Content))
                    return;

                if (ModerationCheck(message.Content))
                    return;

                SyncMessageToTwitch(message.Author.Username, message.Content);
                SyncMessageToYouTube(message.Author.Username, message.Content);
                syncedMessages.Add(message.Content);
                await Task.Delay(0); // Fake Await
            }

            private void SyncMessageToTwitch(string username, string message)
            {
                twitchClient.SendMessage(twitchClient.JoinedChannels[0], $"{username}: {message}");
            }

            private void SyncMessageToDiscord(string username, string message)
            {
                // Replace the following lines with your own Discord message syncing logic
                var channelId = DiscordChannelID;
                var channel = discordClient.GetChannel(channelId) as SocketTextChannel;
                channel?.SendMessageAsync($"{username}: {message}");
            }

            private void SyncMessageToYouTube(string username, string message)
            {
                // Replace the following lines with your own YouTube message syncing logic
                var videoId = YouTubeclientID;
                var commentThread = new Google.Apis.YouTube.v3.Data.CommentThread
                {
                    Snippet = new Google.Apis.YouTube.v3.Data.CommentThreadSnippet
                    {
                        VideoId = videoId,
                        TopLevelComment = new Google.Apis.YouTube.v3.Data.Comment
                        {
                            Snippet = new Google.Apis.YouTube.v3.Data.CommentSnippet
                            {
                                TextOriginal = $"{username}: {message}"
                            }
                        }
                    }
                };
                youtubeService.CommentThreads.Insert(commentThread, "snippet").Execute();
            }

            private bool IsMessageSynced(string message)
            {
                return syncedMessages.Contains(message);
            }

            private bool ModerationCheck(string message)
            {
                // Replace this with your own moderation check logic
                // Return true if the message should be ignored due to moderation concerns
                // For example, you can check for profanity or other inappropriate content
                return false;
            }

            private Task LogAsync(LogMessage logMessage)
            {
                // Fix Console.WriteLine(logMessage.ToString());
                return Task.CompletedTask;
            }
        }
        /*
        public static async Task Start()
        {
            var messageSyncService = new MessageSyncService(TWITCHUSERNAME, TWITCHTOKEN, DISCORDTOKEN, YOUTUBEVIDEOAPIKEY);
            await messageSyncService.StartAsync();
        } */
    }
}