using TwitchLib.Client;
using TwitchLib.Api.Helix;
using TwitchLib.Client.Events;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.Client.Models;
using Google.Apis.Util;
using TwitchLib.Communication.Events;
using TwitchLib.Api;
using TwitchLib.Api.Interfaces;
using TwitchLib.Api.Auth;
using TwitchLib.Communication.Models;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Extensions;
using TwitchLib.Communication.Clients;
using Google.Apis.Auth.OAuth2;
using System.Diagnostics;
using static System.Net.WebRequestMethods;
using TwitchLib.PubSub.Models.Responses.Messages;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;

namespace TwinZMultiChat.Utilities
{
    public class MyTwitchAPI
    {
#pragma warning disable IDE0044 // Ignore readonly warning
        private static string TwitchClientID = "";
        private static string TwitchClientSecret = "";
        private static string TwitchChannelID = "";
        private static TwitchLib.Api.Helix.Models.Users.GetUsers.User? TwitchUserID; 
        private static string TwitchRedirectUri = "http://localhost:8080/redirect/";
        private static bool active = false;
        private static TwinZMultiChat.MainForm? UiForm;
        private string RefreshToken = $"";
        private static List<string> Scopes = new() { "chat:read", "whispers:read", "whispers:edit", "chat:edit", "channel:moderate" };

        private TwitchLib.Api.Helix.Helix? _helix = new();
        private static TwitchClient? _client;
        private static TwitchAPI? _twitchApi;
#pragma warning restore IDE0044

        public event EventHandler<OnMessageReceivedArgs>? ChatMessageReceived;

        public MyTwitchAPI(TwinZMultiChat.MainForm sender, string twitchClientID, string twitchClientSecret)
        {
            TwitchClientID = twitchClientID;
            TwitchClientSecret = twitchClientSecret;
            UiForm = sender;

            TwitchClientSecret = twitchClientSecret;
            TwitchClientID = twitchClientID;
            _twitchApi = new TwitchAPI();
            _twitchApi!.Settings.ClientId = TwitchClientID;
            _twitchApi!.Settings.Secret = TwitchClientSecret;
        }

        private async void OnConnect(object? sender, OnConnectedArgs e)
        {
            await UiForm!.WriteToLog($"Twitch Connected: {e.BotUsername} {e.AutoJoinChannel}\n");
        }

        private async void OnDisconnect(object? sender, OnDisconnectedEventArgs e)
        {
            await UiForm!.WriteToLog($"Twitch Disconnected\n");

        }

        public async Task ConnectAsync()
        {
            try
            {
                string authorizationUrl = GetAuthorizationCodeUrl(TwitchClientID, TwitchRedirectUri, Scopes)!;
                if (_twitchApi!.Settings.AccessToken == null)
                {
                    LaunchUrl(authorizationUrl);
                    ValidateCreds();
                    _twitchApi = new TwitchLib.Api.TwitchAPI();
                    _twitchApi.Settings.ClientId = TwitchClientID;
                    WebServer server = new(TwitchRedirectUri);

                    Authorization auth = (await server.Listen())!;
                    if (auth == null)
                    {
                        throw new Exception("Authentication failed.\n");
                    }
                    AuthCodeResponse resp = await _twitchApi.Auth.GetAccessTokenFromCodeAsync(auth!.Code, TwitchClientSecret, TwitchRedirectUri)!;
                    _twitchApi.Settings.AccessToken = resp.AccessToken;
                    RefreshResponse refresh = await _twitchApi.Auth.RefreshAuthTokenAsync(resp.RefreshToken, TwitchClientSecret)!;
                    _twitchApi.Settings.AccessToken = refresh.AccessToken;
                    TwitchUserID = (await _twitchApi!.Helix.Users.GetUsersAsync()).Users[0];
                    TwitchChannelID = TwitchUserID.Id;
                    // print out all the data we've got
                    await UiForm!.WriteToLog($"Authorization success!\nUser: {TwitchUserID.DisplayName} (id: {TwitchUserID.Id})\nAccess token: {refresh.AccessToken}\nRefresh token: {refresh.RefreshToken}\nExpires in: {refresh.ExpiresIn}\nScopes: {string.Join(", ", refresh.Scopes)}\n");

                }
                if (_twitchApi!.Settings.AccessToken != null)
                {
                    ConnectionCredentials credentials = new(TwitchUserID!.Login, _twitchApi!.Settings.AccessToken);
                    ClientOptions clientOptions = new()
                    {
                        MessagesAllowedInPeriod = 750,
                        ThrottlingPeriod = TimeSpan.FromSeconds(30)
                    };
                    _client = new();
                    _client.Initialize(credentials, TwitchUserID!.Login);
                    _client.DisableAutoPong = false;
                    await RegisterEvents();
                    if (_client.Connect())
                    {
                        _client.JoinChannel(TwitchUserID!.Login);
                        await SendMessage("Active\n");
                    }
                    else
                    {
                        await UiForm!.WriteToLog("Client Failed to connect\n");
                        throw new Exception("Client Failed to connect");
                    }
                }
            }
            catch (Exception e)
            {
                await UiForm!.WriteToLog($"Error: {e.Message}");
                await Task.FromException(e);
            }
        }

        private static string GetAuthorizationCodeUrl(string clientId, string redirectUri, List<string> scopes)
        {
            string scopesStr = string.Join(' ', scopes);

            return "https://id.twitch.tv/oauth2/authorize?" +
                   $"client_id={clientId}&" +
                   $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                   "response_type=code&" +
                   $"scope={Uri.EscapeDataString(scopesStr)}";
        }

        private async static void ValidateCreds()
        {
            if (String.IsNullOrEmpty(TwitchClientID))
                throw new Exception("client id cannot be null or empty\n");
            if (String.IsNullOrEmpty(TwitchClientSecret))
                throw new Exception("client secret cannot be null or empty\n");
            if (String.IsNullOrEmpty(TwitchRedirectUri))
                throw new Exception("redirect uri cannot be null or empty\n");
            await UiForm!.WriteToLog($"Client ID: '{TwitchClientID}', \nClient Secret: '{TwitchClientSecret}'\nRedirect url: '{TwitchRedirectUri}'.\n");
        }

        private Task RegisterEvents()
        {
            _client!.OnMessageReceived += OnChatMessageReceived;
            _client!.OnDisconnected += OnDisconnect;
            _client!.OnConnected += OnConnect;
            return Task.CompletedTask;
        }

        private Task DeregisterEvents()
        {
            _client!.OnMessageReceived -= OnChatMessageReceived;
            _client!.OnDisconnected -= OnDisconnect;
            _client!.OnConnected -= OnConnect;
            return Task.CompletedTask;
        }

        // Method to raise the ChatMessageReceived event
        protected virtual void OnChatMessageReceived(object? sender, OnMessageReceivedArgs e)
        {
            ChatMessageReceived?.Invoke(this, e);
        }

        public async Task SendMessage(string message)
        {
            try
            {
                if (_client!.IsConnected)
                {
                    _client.SendMessage(TwitchUserID!.Login, message);
                }
                else
                {
                    throw new Exception("Twitch Client Not Connected.\n");
                }
                await Task.Delay(0); // Fake Delay
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public Task DisconnectAsync()
        {
            if (_client != null && _client!.IsConnected)
            {
                active = false;
                DeregisterEvents();
                _client!.Disconnect();
            }
            return Task.CompletedTask;
        }

        public async Task<TwitchLib.Api.Helix.Models.Users.GetUsers.GetUsersResponse?> GetUsers(List<string> userLogins)
        {
            try
            {
                return await _helix!.Users.GetUsersAsync(userLogins);
            }
            catch (Exception e)
            {
                await UiForm!.WriteToLog(e.Message);
                return null;
            }
        }

        public async Task<TwitchLib.Api.Helix.Models.Streams.GetStreams.GetStreamsResponse?> GetStream(List<string> userIds)
        {
            try
            {
                return await _helix!.Streams.GetStreamsAsync(userIds: userIds);
            }
            catch (Exception e)
            {
                await UiForm!.WriteToLog(e.Message);
                return null;
            }
        }

        public async Task<TwitchLib.Api.Helix.Models.Channels.GetChannelInformation.GetChannelInformationResponse?> GetChannels(string channelIds)
        {
            try
            {
                return await _helix!.Channels.GetChannelInformationAsync(channelIds);
            }
            catch (Exception e)
            {
                await UiForm!.WriteToLog(e.Message);
                return null;
            }
        }

        public async Task<TwitchLib.Api.Helix.Models.Search.SearchChannelsResponse?> SearchChannels(string query, int limit = 20)
        {
            try
            {
                return await _helix!.Search.SearchChannelsAsync(query, true, null, limit);
            }
            catch (Exception e)
            {
                await UiForm!.WriteToLog(e.Message);
                return null;
            }
        }

        public async Task<TwitchLib.Api.Helix.Models.Users.GetUserFollows.GetUsersFollowsResponse?> GetFollowers(string userId, int first = 20)
        {
            try
            {
                return await _helix!.Users.GetUsersFollowsAsync(toId: userId, first: first);
            }
            catch (Exception e)
            {
                await UiForm!.WriteToLog(e.Message);
                return null;
            }
        }

        public async Task<TwitchLib.Api.Helix.Models.Videos.GetVideos.GetVideosResponse?> GetVideos(string userId, int limit = 20)
        {
            try
            {
                return await _helix!.Videos.GetVideosAsync(userId: userId, first: limit);
            }
            catch (Exception e)
            {
                await UiForm!.WriteToLog(e.Message);
                return null;
            }
        }

        public async Task<TwitchLib.Api.Helix.Models.Subscriptions.GetUserSubscriptionsResponse?> GetSubscriptions(string userId, List<string> UserIds)
        {
            try
            {
                return await _helix!.Subscriptions.GetUserSubscriptionsAsync(userId, UserIds);
            }
            catch (Exception e)
            {
                await UiForm!.WriteToLog(e.Message);
                return null;
            }
        }

        public async Task<TwitchLib.Api.Helix.Models.Games.GetGamesResponse?> GetGames(List<string> gameIds)
        {
            try
            {
                return await _helix!.Games.GetGamesAsync(gameIds);
            }
            catch (Exception e)
            {
                await UiForm!.WriteToLog(e.Message);
                return null;
            }
        }

        public async Task<TwitchLib.Api.Helix.Models.Games.GetTopGamesResponse?> GetTopGames(int limit = 20)
        {
            try
            {
                return await _helix!.Games.GetTopGamesAsync(first: limit);
            }
            catch (Exception e)
            {
                await UiForm!.WriteToLog(e.Message);
                return null;
            }
        }

        public static async Task<string> GetBadgeIconPathFromTwitchMessage(TwitchLibMessage twitchMessage)
        {
            if (twitchMessage!.Badges != null && twitchMessage!.Badges.Count > 0)
            {
                foreach (var badge in twitchMessage.Badges)
                {
                    // Check for the desired badge information
                    if (badge.Key == "subscriber" && badge.Value == "1")
                    {
                        // Retrieve the badge images from Twitch API
                        string badgeUrl = await GetBadgeImageUrlFromTwitch(badge.Key, badge.Value);
                        return badgeUrl;
                    }
                    else if (badge.Key == "moderator")
                    {
                        // Retrieve the badge images from Twitch API
                        string badgeUrl = await GetBadgeImageUrlFromTwitch(badge.Key, badge.Value);
                        return badgeUrl;
                    }
                    // Add more badge checks if needed
                }
            }

            return string.Empty; // No matching badge found
        }

        public static async Task<string> GetBadgeImageUrlFromTwitch(string badgeId, string version)
        {
            string apiUrl = $"https://api.twitch.tv/kraken/chat/{TwitchChannelID}/badges/{badgeId}/versions/{version}";

            using (HttpClient client = new())
            {
                client.DefaultRequestHeaders.Add("Client-ID", "your-client-id"); // Replace with your Twitch Client ID
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.twitchtv.v5+json");

                HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    dynamic jsonResult = JsonConvert.DeserializeObject(responseContent) ?? string.Empty;
                    string badgeImageUrl = jsonResult["badge_sets"][badgeId]["versions"][version]["image_url_1x"];

                    return badgeImageUrl;
                }
                else
                {
                    // Handle the case when the API request fails
                    // You can log the error, throw an exception, or return a default badge image URL
                    return string.Empty;
                }
            }
        }

        public async Task<string> GetUserId(string username)
        {
            try
            {
                GetUsersResponse usersResponse = await _helix!.Users.GetUsersAsync(logins: new List<string> { username });
                if (usersResponse.Users.Length >= 0)
                {
                    return usersResponse.Users[0].Id;
                }
                return string.Empty;
            }
            catch (Exception e)
            {
                await UiForm!.WriteToLog(e.Message);
                return string.Empty;
            }
        }

        public async Task<string> IsStreamLive(string twitchID, string BotUserName)
        {
            try
            {
                GetUsersResponse usersResponse = await _helix!.Users.GetUsersAsync(new List<string> { twitchID })!;
                var user = usersResponse.Users.FirstOrDefault()!;
                if (user == null)
                {
                    return $"{BotUserName} not found.";
                }

                GetStreamsResponse streamsResponse = await _helix!.Streams.GetStreamsAsync(userIds: new List<string> { user.Id })!;
                var stream = streamsResponse.Streams.FirstOrDefault();
                if (stream == null)
                {
                    return string.Empty;
                }

                return $"{BotUserName} is live: https://www.twitch.tv/{stream.UserName.ToUpper()}";
            }
            catch (Exception e)
            {
                await UiForm!.WriteToLog(e.Message);
                return $"An error occurred checking if {BotUserName} is live.";
            }
        }

        public async static void LaunchUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                // Handle any exceptions if the browser launch fails
                await UiForm!.WriteToLog($"Failed to launch the default web browser: {ex.Message}");
            }
        }

        public class AccessTokenResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; } = string.Empty;

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; } = int.MinValue;

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; } = string.Empty;
        }

        class TwitchAccessToken
        {
            [JsonProperty("access_token")]
            public string? AccessToken { get; set; }
            [JsonProperty("expires_in")]
            public int? ExpiresIn { get; set; }
            [JsonProperty("token_type")]
            public string? TokenType { get; set; }
        }
    }
}
