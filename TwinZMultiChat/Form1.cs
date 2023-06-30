using Newtonsoft.Json;
using System.ComponentModel.Design;
using System.Text.Json.Nodes;
using TwitchLib.Client;

namespace TwinZMultiChat
{
    public partial class Form1 : Form
    {
        private static string DiscordBotToken = "";
        private static ulong DiscordChannelID = 0;
        private static string YouTubeOauthToken = "";
        private static string YouTubeClientID = "";
        private static string YouTubeClientSecret = "";
        private static string TwitchOauthToken = "";
        private static string TwitchClientID = "";
        private static string TwitchClientSecret = "";

        public Form1() // Fix Loading Saved Variables
        {
            InitializeComponent();
            LogBox.AppendText("Init Successful.\n");
            LoadSavedVariables();

        }

        private void LoadSavedVariables()
        {
            if (File.Exists("Config.xml"))
            {
                StreamReader CFG = File.OpenText("Config.xml");
                string cfgInput = CFG.ReadToEnd();
                string[] Configs = cfgInput.Split("|||");
                foreach (string CurString in Configs)
                {
                    CurString.Replace("|||", "");
                }

                if (!string.IsNullOrWhiteSpace(Configs[0]))
                {
                    DiscordBotToken = Configs[0].Replace("\"", "");
                    DiscordBotBox.Text = DiscordBotToken;
                }
                if (ulong.TryParse(Configs[1], out ulong j))
                {
                    DiscordChannelID = j;
                    DiscordChannelIDBox.Text = DiscordChannelID.ToString();
                }
                if (!string.IsNullOrWhiteSpace(Configs[2]))
                {
                    YouTubeOauthToken = Configs[2].Replace("\"", "");
                    YouTubeOauthTokenBox.Text = YouTubeOauthToken;
                }
                if (!string.IsNullOrWhiteSpace(Configs[3]))
                {
                    YouTubeClientID = Configs[3].Replace("\"", "");
                    YouTubeClientIDBox.Text = YouTubeClientID;
                }
                if (!string.IsNullOrWhiteSpace(Configs[4]))
                {
                    YouTubeClientSecret = Configs[4].Replace("\"", "");
                    YouTubeClientSecretBox.Text = YouTubeClientSecret;
                }
                if (!string.IsNullOrWhiteSpace(Configs[5]))
                {
                    TwitchOauthToken = Configs[5].Replace("\"", "");
                    TwitchOauthTokenBox.Text = TwitchOauthToken;
                }
                if (!string.IsNullOrWhiteSpace(Configs[6]))
                {
                    TwitchClientID = Configs[6].Replace("\"", "");
                    TwitchClientIDBox.Text = TwitchClientID;
                }
                if (!string.IsNullOrWhiteSpace(Configs[7]))
                {
                    TwitchClientSecret = Configs[7].Replace("\"", "");
                    TwitchClientSecretBox.Text = TwitchClientSecret;
                }
                    CFG.Close();
                LogBox.AppendText("Loaded Successfully.\n");
            }
            else
            {
                LogBox.AppendText("Config Not Found.\n");
            }
        }

        private async void SaveBtn_Click(object sender, EventArgs e)
        {
            string JsonOutput = "";
            JsonOutput += JsonConvert.SerializeObject(DiscordBotToken) + "|||";
            JsonOutput += JsonConvert.SerializeObject(DiscordChannelID) + "|||";
            JsonOutput += JsonConvert.SerializeObject(YouTubeOauthToken) + "|||";
            JsonOutput += JsonConvert.SerializeObject(YouTubeClientID) + "|||";
            JsonOutput += JsonConvert.SerializeObject(YouTubeClientSecret) + "|||";
            JsonOutput += JsonConvert.SerializeObject(TwitchOauthToken) + "|||";
            JsonOutput += JsonConvert.SerializeObject(TwitchClientID) + "|||";
            JsonOutput += JsonConvert.SerializeObject(TwitchClientSecret) + "|||";
            File.WriteAllText("Config.xml", JsonOutput);
            LogBox.AppendText("Save Successfully.\n");
            await Task.Delay(0); // Fake Await
        }

        private void ResetBtn_Click(object sender, EventArgs e)
        {
            DiscordBotBox.Text = "";
            DiscordChannelIDBox.Text = "";
            TwitchOauthTokenBox.Text = "";
            TwitchClientIDBox.Text = "";
            TwitchClientSecretBox.Text = "";
            YouTubeOauthTokenBox.Text = "";
            YouTubeClientIDBox.Text = "";
            YouTubeClientSecretBox.Text = "";
            File.Delete("Config.xml");
            LogBox.AppendText("Reset Successfully.\n");

        }

        private async void StartBtn_Click(object sender, EventArgs e)
        {
            LogBox.AppendText(DiscordBotToken);
            await Task.Delay(0); // Fake Await
        }

        private async void StopBtn_Click(object sender, EventArgs e)
        {

            await Task.Delay(0); // Fake Await
        }

        private void DiscordBotBoxChanged(object sender, EventArgs e)
        {
            DiscordBotToken = DiscordBotBox.Text ?? "";

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

        private void TwichOauthBoxChanged(object sender, EventArgs e)
        {
            TwitchOauthToken = TwitchOauthTokenBox.Text ?? "";

        }

        private void TwitchClientIDBoxChanged(object sender, EventArgs e)
        {
            TwitchClientID = TwitchClientIDBox.Text ?? "";

        }

        private void TwitchClientSecretBoxChanged(object sender, EventArgs e)
        {
            TwitchClientSecret = TwitchClientSecretBox.Text ?? "";

        }

        private void YouTubeOauthBoxChanged(object sender, EventArgs e)
        {
            YouTubeOauthToken = YouTubeOauthTokenBox.Text ?? "";

        }

        private void YouTubeClientIDBoxChanged(object sender, EventArgs e)
        {
            YouTubeClientID = YouTubeClientIDBox.Text ?? "";
        }

        private void YouTubeClientSecretBoxChanged(object sender, EventArgs e)
        {
            YouTubeClientSecret = YouTubeClientSecretBox.Text ?? "";

        }
    }
}