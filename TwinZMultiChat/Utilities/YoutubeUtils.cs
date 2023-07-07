using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TwitchLib.Api.Core.Exceptions;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using TwitchLib.Api.Helix;
using TwitchLib.Communication.Interfaces;
using System.Reflection;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using Google.Apis.Upload;
using TwitchLib.Client.Models;

namespace TwinZMultiChat.Utilities
{
    public class MyYoutubeAPI
    {
        public bool ChatListener = true;
        private static YouTubeService? _youtubeService;
        private static string applicationName = "";
        private static string streamMsgIntro = "";
        private static bool isChatMessageListenerActive = false;
        private static Task? _chatMessageListenerTask;
        private static LiveBroadcast? curBroadcast;
        private static TwinZMultiChat.MainForm? UiForm;

        // Define the event
        public event EventHandler<ChatMessageEventArgs>? ChatMessageReceived;

        public MyYoutubeAPI(TwinZMultiChat.MainForm sender, string ApplicationName, string StreamMsgIntro)
        {
            applicationName = ApplicationName;
            UiForm = sender;
            streamMsgIntro = StreamMsgIntro;
        }
        public async Task ConnectAsync()
        {
            try
            {
                if (!File.Exists("client_secret.json"))
                {
                    MessageBox.Show("Error: client_secret.json NOT found. Please put the file downloaded from console.google.com in the application directory.", "Warning!");
                }
                UserCredential credential;
                using (FileStream stream = new("client_secret.json", FileMode.Open, FileAccess.Read)!)
                {
                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        new[] { YouTubeService.Scope.Youtube },
                        "user",
                        CancellationToken.None,
                        new FileDataStore(Assembly.GetExecutingAssembly().GetName().Name)
                    );
                }

                BaseClientService.Initializer initializer = new()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
                };

                _youtubeService = new YouTubeService(initializer)!;
                curBroadcast = GetLiveBroadcast(_youtubeService!)!;

                _chatMessageListenerTask = Task.Run(StartChatMessageListener);
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log or display an error message)
                await UiForm!.WriteToLog($"An error occurred while connecting: {ex.Message}");
            }
        }

        public Task DisconnectAsync()
        {
            if (_youtubeService != null)
            {
                isChatMessageListenerActive = false;
                // Wait for the chat message listener task to complete (if it's still running)
                if (_chatMessageListenerTask != null)
                {
                    _chatMessageListenerTask.GetAwaiter().GetResult();
                    _chatMessageListenerTask = null;
                }
                _youtubeService!.Dispose();
            }
            return Task.CompletedTask;
        }

        // Method to raise the ChatMessageReceived event
        protected virtual void OnChatMessageReceived(ChatMessageEventArgs e)
        {
            ChatMessageReceived?.Invoke(this, e);
        }

        private async Task StartChatMessageListener()
        {
            string liveStreamId = await GetActiveLiveStreamId()!;
            if (string.IsNullOrEmpty(liveStreamId))
            {
                await UiForm!.WriteToLog("No active live streams found.\n");
                return;
            }

            string liveChatId = await GetLiveChatIdForCurrentStream(liveStreamId);
            if (string.IsNullOrEmpty(liveChatId))
            {
                await UiForm!.WriteToLog("No live chat ID found.\n");
                return;
            }

            HashSet<string> processedMessageIds = new (); // Track processed message IDs
            isChatMessageListenerActive = true;
            LiveChatMessageListResponse? liveChatMessages = null;
            IEnumerable<LiveChatMessage?> newMessages;
            while (isChatMessageListenerActive)
            {
                if (liveChatMessages == null || !string.IsNullOrEmpty(liveChatMessages.NextPageToken))
                {
                    liveChatMessages = await GetLiveChatMessages(liveChatId, liveChatMessages?.NextPageToken!);
                    if (liveChatMessages == null)
                    {
                        // Error occurred while fetching messages, break the loop
                        break;
                    }
                }

                newMessages = liveChatMessages.Items;
                foreach (LiveChatMessage? message in newMessages)
                {
                    if (message != null && !processedMessageIds.Contains(message.Id) && !message.Snippet.DisplayMessage.StartsWith(streamMsgIntro))
                    {
                        // Create an event argument object with the message data
                        ChatMessageEventArgs eventArgs = new (message.AuthorDetails.DisplayName, message.Snippet.DisplayMessage);
                        processedMessageIds.Add(message.Id); // Add message ID to processed collection
                                                             // Raise the event with the message data
                        OnChatMessageReceived(eventArgs);
                    }
                }

                await Task.Delay(10 * 1000);
            }
        }

        public async Task<SearchListResponse> SearchVideos(string query, int maxResults = 20)
        {
            SearchResource.ListRequest searchListRequest = _youtubeService!.Search.List("snippet")!;
            searchListRequest.Q = query;
            searchListRequest.MaxResults = maxResults;
            return await searchListRequest.ExecuteAsync()!;
        }

        public async Task<ChannelListResponse> GetChannel(string channelId)
        {
            ChannelsResource.ListRequest channelListRequest = _youtubeService!.Channels.List("snippet,contentDetails,statistics")!;
            channelListRequest.Id = channelId;
            return await channelListRequest.ExecuteAsync()!;
        }

        public async Task<VideoListResponse> GetVideo(string videoId)
        {
            VideosResource.ListRequest videoListRequest = _youtubeService!.Videos.List("snippet,contentDetails,statistics")!;
            videoListRequest.Id = videoId;
            return await videoListRequest.ExecuteAsync()!;
        }

        public async Task<PlaylistListResponse> GetPlaylist(string playlistId)
        {
            PlaylistsResource.ListRequest playlistListRequest = _youtubeService!.Playlists.List("snippet,contentDetails")!;
            playlistListRequest.Id = playlistId;
            return await playlistListRequest.ExecuteAsync()!;
        }

        public async Task<CommentThreadListResponse> GetVideoComments(string videoId, int maxResults = 20)
        {
            CommentThreadsResource.ListRequest commentThreadListRequest = _youtubeService!.CommentThreads.List("snippet")!;
            commentThreadListRequest.VideoId = videoId;
            commentThreadListRequest.MaxResults = maxResults;
            return await commentThreadListRequest.ExecuteAsync()!;
        }

        public async Task<PlaylistItemListResponse> GetPlaylistItems(string playlistId, int maxResults = 20)
        {
            PlaylistItemsResource.ListRequest playlistItemListRequest = _youtubeService!.PlaylistItems.List("snippet")!;
            playlistItemListRequest.PlaylistId = playlistId;
            playlistItemListRequest.MaxResults = maxResults;
            return await playlistItemListRequest.ExecuteAsync()!;
        }

        public async Task<SubscriptionListResponse> GetChannelSubscriptions(string channelId, int maxResults = 20)
        {
            SubscriptionsResource.ListRequest subscriptionListRequest = _youtubeService!.Subscriptions.List("snippet")!;
            subscriptionListRequest.ChannelId = channelId;
            subscriptionListRequest.MaxResults = maxResults;
            return await subscriptionListRequest.ExecuteAsync()!;
        }
           
        public async Task<ActivityListResponse> GetChannelActivities(string channelId, int maxResults = 20)
        {
            ActivitiesResource.ListRequest activityListRequest = _youtubeService!.Activities.List("snippet,contentDetails")!;
            activityListRequest.ChannelId = channelId;
            activityListRequest.MaxResults = maxResults;
            return await activityListRequest.ExecuteAsync()!;
        }

        public async Task<LiveBroadcastListResponse> GetLiveBroadcasts(LiveBroadcastsResource.ListRequest.BroadcastStatusEnum broadcastStatus, int maxResults = 20)
        {
            LiveBroadcastsResource.ListRequest liveBroadcastListRequest = _youtubeService!.LiveBroadcasts.List("snippet,contentDetails,status")!;
            liveBroadcastListRequest.BroadcastStatus = broadcastStatus;
            liveBroadcastListRequest.MaxResults = maxResults;
            return await liveBroadcastListRequest.ExecuteAsync()!;
        }

        public async Task<LiveStreamListResponse> GetLiveStreams(string part, int maxResults = 20)
        {
            LiveStreamsResource.ListRequest liveStreamListRequest = _youtubeService!.LiveStreams.List(part)!;
            liveStreamListRequest.MaxResults = maxResults;
            return await liveStreamListRequest.ExecuteAsync()!;
        }

        public async Task<LiveChatMessageListResponse> GetLiveChatMessages(string liveChatId, string pageToken = "", int maxResults = 6)
        {
                LiveChatMessagesResource.ListRequest liveChatMessagesListRequest = _youtubeService!.LiveChatMessages.List(liveChatId, "snippet,authorDetails")!;
                liveChatMessagesListRequest.PageToken = pageToken;
                liveChatMessagesListRequest.MaxResults = maxResults;
                return await liveChatMessagesListRequest.ExecuteAsync();
        }


        public async Task<string> GetActiveLiveStreamId()
        {
            LiveBroadcastsResource.ListRequest liveBroadcastsListRequest = _youtubeService!.LiveBroadcasts.List("id")!;
            liveBroadcastsListRequest.BroadcastStatus = LiveBroadcastsResource.ListRequest.BroadcastStatusEnum.Active;

            LiveBroadcastListResponse liveBroadcastsListResponse = await liveBroadcastsListRequest.ExecuteAsync()!;

            if (liveBroadcastsListResponse.Items.Count > 0)
            {
                string liveStreamId = liveBroadcastsListResponse.Items[0].Id!;
                return liveStreamId;
            }

            return string.Empty; // No active live streams found
        }

        public async Task<string> GetLiveChatIdForCurrentStream(string liveStreamId)
        {
            LiveBroadcastsResource.ListRequest liveBroadcastsListRequest = _youtubeService!.LiveBroadcasts.List("snippet")!;
            liveBroadcastsListRequest.Id = liveStreamId;

            LiveBroadcastListResponse liveBroadcastsListResponse = await liveBroadcastsListRequest.ExecuteAsync()!;

            if (liveBroadcastsListResponse.Items.Count > 0)
            {
                string liveChatId = liveBroadcastsListResponse.Items[0].Snippet.LiveChatId!;
                return liveChatId;
            }

            return string.Empty; // No live chat ID found
        }

        public async Task SendLiveChatMessage(string messageText)
        {
            string liveStreamId = await GetActiveLiveStreamId()!;
            if (string.IsNullOrEmpty(liveStreamId))
            {
                await UiForm!.WriteToLog("No active live streams found.\n");
                return;
            }

            string liveChatId = await GetLiveChatIdForCurrentStream(liveStreamId);
            if (string.IsNullOrEmpty(liveChatId))
            {
                await UiForm!.WriteToLog("No live chat ID found.\n");
                return;
            }

            LiveChatMessage liveChatMessage = new ()
            {
                Snippet = new LiveChatMessageSnippet
                {
                    LiveChatId = liveChatId,
                    Type = "textMessageEvent",
                    TextMessageDetails = new LiveChatTextMessageDetails
                    {
                        MessageText = messageText
                    }
                }
            };           

            LiveChatMessagesResource.InsertRequest insertRequest = _youtubeService!.LiveChatMessages.Insert(liveChatMessage, "snippet")!;
            LiveChatMessage insertedMessage = await insertRequest.ExecuteAsync()!;
        }

        static LiveBroadcast GetLiveBroadcast(YouTubeService youtubeService)
        {
            LiveBroadcastsResource.ListRequest request = youtubeService.LiveBroadcasts.List("snippet")!;
            request.BroadcastStatus = LiveBroadcastsResource.ListRequest.BroadcastStatusEnum.Active;

            LiveBroadcastListResponse? response = request.Execute();
            return response.Items.FirstOrDefault()!;
        }

        public async Task DeleteLiveChatMessage(string liveChatMessageId)
        {
            await _youtubeService!.LiveChatMessages.Delete(liveChatMessageId).ExecuteAsync();
        }

        public async Task<LiveChatModeratorListResponse> GetLiveChatModerators(string liveChatId, string? pageToken = null, int maxResults = 100)
        {
            LiveChatModeratorsResource.ListRequest liveChatModeratorsListRequest = _youtubeService!.LiveChatModerators.List(liveChatId, string.Empty)!;
            liveChatModeratorsListRequest.PageToken = pageToken;
            liveChatModeratorsListRequest.MaxResults = maxResults;
            return await liveChatModeratorsListRequest.ExecuteAsync()!;
        }

        public async Task<LiveChatModerator> InsertLiveChatModerator(string liveChatId, string channelId)
        {
            LiveChatModerator liveChatModerator = new()
            {
                Snippet = new LiveChatModeratorSnippet
                {
                    LiveChatId = liveChatId,
                    ModeratorDetails = new ChannelProfileDetails
                    {
                        ChannelId = channelId
                    }
                }
            };

            LiveChatModeratorsResource.InsertRequest insertRequest = _youtubeService!.LiveChatModerators.Insert(liveChatModerator, "snippet")!;
            return await insertRequest.ExecuteAsync();
        }

        public async Task DeleteLiveChatModerator(string liveChatModeratorId)
        {
            await _youtubeService!.LiveChatModerators.Delete(liveChatModeratorId).ExecuteAsync()!;
        }

        public async Task<string> CheckLiveStatus(string userId)
        {
            try
            {
                LiveBroadcastListResponse liveBroadcasts = await GetLiveBroadcasts(LiveBroadcastsResource.ListRequest.BroadcastStatusEnum.Active, 1)!;
                if (liveBroadcasts.Items.Count == 0)
                {
                    return string.Empty;
                }

                LiveBroadcast liveBroadcast = liveBroadcasts.Items[0]!;
                ChannelListResponse channel = await GetChannel(liveBroadcast.Snippet.ChannelId)!;
                if (channel.Items[0].Id != userId)
                {
                    return string.Empty;
                }

                string link = $"https://www.youtube.com/watch?v={liveBroadcast.Id}";
                return $"{userId} is live: {link}";
            }
            catch (Exception e)
            {
                await UiForm!.WriteToLog($"{e.Message}\n");
                return $"An error occurred while checking the live status of {userId}: {e.Message}";
            }
        }


        // Custom type classes


        public class ChatMessageEventArgs : EventArgs
        {
            public string Username { get; }
            public string Message { get; }

            public ChatMessageEventArgs(string username, string message)
            {
                Username = username;
                Message = message;
            }
        }

    }
}