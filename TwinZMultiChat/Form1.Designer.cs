namespace TwinZMultiChat
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.DiscordBotBox = new System.Windows.Forms.TextBox();
            this.StopBtn = new System.Windows.Forms.Button();
            this.StartBtn = new System.Windows.Forms.Button();
            this.LogBox = new System.Windows.Forms.RichTextBox();
            this.DiscordBotLabel = new System.Windows.Forms.Label();
            this.TwitchOauthTokenLabel = new System.Windows.Forms.Label();
            this.TwitchOauthTokenBox = new System.Windows.Forms.TextBox();
            this.TwitchClientSecretLabel = new System.Windows.Forms.Label();
            this.TwitchClientSecretBox = new System.Windows.Forms.TextBox();
            this.TwitchClientIDLabel = new System.Windows.Forms.Label();
            this.TwitchClientIDBox = new System.Windows.Forms.TextBox();
            this.YouTubeOauthTokenLabel = new System.Windows.Forms.Label();
            this.YouTubeOauthTokenBox = new System.Windows.Forms.TextBox();
            this.YouTubeClientIDLabel = new System.Windows.Forms.Label();
            this.YouTubeClientIDBox = new System.Windows.Forms.TextBox();
            this.YouTubeClientSecretLabel = new System.Windows.Forms.Label();
            this.YouTubeClientSecretBox = new System.Windows.Forms.TextBox();
            this.DiscordChannelLabel = new System.Windows.Forms.Label();
            this.DiscordChannelIDBox = new System.Windows.Forms.TextBox();
            this.SaveBtn = new System.Windows.Forms.Button();
            this.ResetBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // DiscordBotBox
            // 
            this.DiscordBotBox.Location = new System.Drawing.Point(474, 30);
            this.DiscordBotBox.Name = "DiscordBotBox";
            this.DiscordBotBox.Size = new System.Drawing.Size(314, 23);
            this.DiscordBotBox.TabIndex = 0;
            this.DiscordBotBox.TextChanged += new System.EventHandler(this.DiscordBotBoxChanged);
            // 
            // StopBtn
            // 
            this.StopBtn.Location = new System.Drawing.Point(634, 399);
            this.StopBtn.Name = "StopBtn";
            this.StopBtn.Size = new System.Drawing.Size(154, 39);
            this.StopBtn.TabIndex = 1;
            this.StopBtn.Text = "Stop";
            this.StopBtn.UseVisualStyleBackColor = true;
            this.StopBtn.Click += new System.EventHandler(this.StopBtn_Click);
            // 
            // StartBtn
            // 
            this.StartBtn.Location = new System.Drawing.Point(474, 399);
            this.StartBtn.Name = "StartBtn";
            this.StartBtn.Size = new System.Drawing.Size(154, 39);
            this.StartBtn.TabIndex = 2;
            this.StartBtn.Text = "Start";
            this.StartBtn.UseVisualStyleBackColor = true;
            this.StartBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // LogBox
            // 
            this.LogBox.Location = new System.Drawing.Point(12, 12);
            this.LogBox.Name = "LogBox";
            this.LogBox.ReadOnly = true;
            this.LogBox.Size = new System.Drawing.Size(456, 426);
            this.LogBox.TabIndex = 3;
            this.LogBox.Text = "";
            // 
            // DiscordBotLabel
            // 
            this.DiscordBotLabel.AutoSize = true;
            this.DiscordBotLabel.Location = new System.Drawing.Point(474, 12);
            this.DiscordBotLabel.Name = "DiscordBotLabel";
            this.DiscordBotLabel.Size = new System.Drawing.Size(102, 15);
            this.DiscordBotLabel.TabIndex = 4;
            this.DiscordBotLabel.Text = "Discord Bot Token";
            // 
            // TwitchOauthTokenLabel
            // 
            this.TwitchOauthTokenLabel.AutoSize = true;
            this.TwitchOauthTokenLabel.Location = new System.Drawing.Point(474, 100);
            this.TwitchOauthTokenLabel.Name = "TwitchOauthTokenLabel";
            this.TwitchOauthTokenLabel.Size = new System.Drawing.Size(111, 15);
            this.TwitchOauthTokenLabel.TabIndex = 6;
            this.TwitchOauthTokenLabel.Text = "Twitch Oauth Token";
            // 
            // TwitchOauthTokenBox
            // 
            this.TwitchOauthTokenBox.Location = new System.Drawing.Point(474, 118);
            this.TwitchOauthTokenBox.Name = "TwitchOauthTokenBox";
            this.TwitchOauthTokenBox.Size = new System.Drawing.Size(314, 23);
            this.TwitchOauthTokenBox.TabIndex = 5;
            this.TwitchOauthTokenBox.TextChanged += new System.EventHandler(this.TwichOauthBoxChanged);
            // 
            // TwitchClientSecretLabel
            // 
            this.TwitchClientSecretLabel.AutoSize = true;
            this.TwitchClientSecretLabel.Location = new System.Drawing.Point(475, 188);
            this.TwitchClientSecretLabel.Name = "TwitchClientSecretLabel";
            this.TwitchClientSecretLabel.Size = new System.Drawing.Size(110, 15);
            this.TwitchClientSecretLabel.TabIndex = 8;
            this.TwitchClientSecretLabel.Text = "Twitch Client Secret";
            // 
            // TwitchClientSecretBox
            // 
            this.TwitchClientSecretBox.Location = new System.Drawing.Point(475, 206);
            this.TwitchClientSecretBox.Name = "TwitchClientSecretBox";
            this.TwitchClientSecretBox.Size = new System.Drawing.Size(314, 23);
            this.TwitchClientSecretBox.TabIndex = 7;
            this.TwitchClientSecretBox.TextChanged += new System.EventHandler(this.TwitchClientSecretBoxChanged);
            // 
            // TwitchClientIDLabel
            // 
            this.TwitchClientIDLabel.AutoSize = true;
            this.TwitchClientIDLabel.Location = new System.Drawing.Point(474, 144);
            this.TwitchClientIDLabel.Name = "TwitchClientIDLabel";
            this.TwitchClientIDLabel.Size = new System.Drawing.Size(89, 15);
            this.TwitchClientIDLabel.TabIndex = 10;
            this.TwitchClientIDLabel.Text = "Twitch Client ID";
            // 
            // TwitchClientIDBox
            // 
            this.TwitchClientIDBox.Location = new System.Drawing.Point(474, 162);
            this.TwitchClientIDBox.Name = "TwitchClientIDBox";
            this.TwitchClientIDBox.Size = new System.Drawing.Size(314, 23);
            this.TwitchClientIDBox.TabIndex = 9;
            this.TwitchClientIDBox.TextChanged += new System.EventHandler(this.TwitchClientIDBoxChanged);
            // 
            // YouTubeOauthTokenLabel
            // 
            this.YouTubeOauthTokenLabel.AutoSize = true;
            this.YouTubeOauthTokenLabel.Location = new System.Drawing.Point(474, 232);
            this.YouTubeOauthTokenLabel.Name = "YouTubeOauthTokenLabel";
            this.YouTubeOauthTokenLabel.Size = new System.Drawing.Size(123, 15);
            this.YouTubeOauthTokenLabel.TabIndex = 12;
            this.YouTubeOauthTokenLabel.Text = "YouTube Oauth Token";
            // 
            // YouTubeOauthTokenBox
            // 
            this.YouTubeOauthTokenBox.Location = new System.Drawing.Point(474, 250);
            this.YouTubeOauthTokenBox.Name = "YouTubeOauthTokenBox";
            this.YouTubeOauthTokenBox.Size = new System.Drawing.Size(314, 23);
            this.YouTubeOauthTokenBox.TabIndex = 11;
            this.YouTubeOauthTokenBox.TextChanged += new System.EventHandler(this.YouTubeOauthBoxChanged);
            // 
            // YouTubeClientIDLabel
            // 
            this.YouTubeClientIDLabel.AutoSize = true;
            this.YouTubeClientIDLabel.Location = new System.Drawing.Point(474, 276);
            this.YouTubeClientIDLabel.Name = "YouTubeClientIDLabel";
            this.YouTubeClientIDLabel.Size = new System.Drawing.Size(101, 15);
            this.YouTubeClientIDLabel.TabIndex = 14;
            this.YouTubeClientIDLabel.Text = "YouTube Client ID";
            // 
            // YouTubeClientIDBox
            // 
            this.YouTubeClientIDBox.Location = new System.Drawing.Point(474, 294);
            this.YouTubeClientIDBox.Name = "YouTubeClientIDBox";
            this.YouTubeClientIDBox.Size = new System.Drawing.Size(314, 23);
            this.YouTubeClientIDBox.TabIndex = 13;
            this.YouTubeClientIDBox.TextChanged += new System.EventHandler(this.YouTubeClientIDBoxChanged);
            // 
            // YouTubeClientSecretLabel
            // 
            this.YouTubeClientSecretLabel.AutoSize = true;
            this.YouTubeClientSecretLabel.Location = new System.Drawing.Point(474, 320);
            this.YouTubeClientSecretLabel.Name = "YouTubeClientSecretLabel";
            this.YouTubeClientSecretLabel.Size = new System.Drawing.Size(122, 15);
            this.YouTubeClientSecretLabel.TabIndex = 16;
            this.YouTubeClientSecretLabel.Text = "YouTube Client Secret";
            // 
            // YouTubeClientSecretBox
            // 
            this.YouTubeClientSecretBox.Location = new System.Drawing.Point(474, 338);
            this.YouTubeClientSecretBox.Name = "YouTubeClientSecretBox";
            this.YouTubeClientSecretBox.Size = new System.Drawing.Size(314, 23);
            this.YouTubeClientSecretBox.TabIndex = 15;
            this.YouTubeClientSecretBox.TextChanged += new System.EventHandler(this.YouTubeClientSecretBoxChanged);
            // 
            // DiscordChannelLabel
            // 
            this.DiscordChannelLabel.AutoSize = true;
            this.DiscordChannelLabel.Location = new System.Drawing.Point(474, 56);
            this.DiscordChannelLabel.Name = "DiscordChannelLabel";
            this.DiscordChannelLabel.Size = new System.Drawing.Size(108, 15);
            this.DiscordChannelLabel.TabIndex = 18;
            this.DiscordChannelLabel.Text = "Discord Channel ID";
            // 
            // DiscordChannelIDBox
            // 
            this.DiscordChannelIDBox.Location = new System.Drawing.Point(474, 74);
            this.DiscordChannelIDBox.Name = "DiscordChannelIDBox";
            this.DiscordChannelIDBox.Size = new System.Drawing.Size(314, 23);
            this.DiscordChannelIDBox.TabIndex = 17;
            this.DiscordChannelIDBox.TextChanged += new System.EventHandler(this.DiscordChannelIDBoxChanged);
            // 
            // SaveBtn
            // 
            this.SaveBtn.Location = new System.Drawing.Point(634, 369);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Size = new System.Drawing.Size(154, 24);
            this.SaveBtn.TabIndex = 19;
            this.SaveBtn.Text = "Save";
            this.SaveBtn.UseVisualStyleBackColor = true;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // ResetBtn
            // 
            this.ResetBtn.Location = new System.Drawing.Point(475, 369);
            this.ResetBtn.Name = "ResetBtn";
            this.ResetBtn.Size = new System.Drawing.Size(154, 24);
            this.ResetBtn.TabIndex = 20;
            this.ResetBtn.Text = "Reset";
            this.ResetBtn.UseVisualStyleBackColor = true;
            this.ResetBtn.Click += new System.EventHandler(this.ResetBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ResetBtn);
            this.Controls.Add(this.SaveBtn);
            this.Controls.Add(this.DiscordChannelLabel);
            this.Controls.Add(this.DiscordChannelIDBox);
            this.Controls.Add(this.YouTubeClientSecretLabel);
            this.Controls.Add(this.YouTubeClientSecretBox);
            this.Controls.Add(this.YouTubeClientIDLabel);
            this.Controls.Add(this.YouTubeClientIDBox);
            this.Controls.Add(this.YouTubeOauthTokenLabel);
            this.Controls.Add(this.YouTubeOauthTokenBox);
            this.Controls.Add(this.TwitchClientIDLabel);
            this.Controls.Add(this.TwitchClientIDBox);
            this.Controls.Add(this.TwitchClientSecretLabel);
            this.Controls.Add(this.TwitchClientSecretBox);
            this.Controls.Add(this.TwitchOauthTokenLabel);
            this.Controls.Add(this.TwitchOauthTokenBox);
            this.Controls.Add(this.DiscordBotLabel);
            this.Controls.Add(this.LogBox);
            this.Controls.Add(this.StartBtn);
            this.Controls.Add(this.StopBtn);
            this.Controls.Add(this.DiscordBotBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox DiscordBotBox;
        private Button StopBtn;
        private Button StartBtn;
        private RichTextBox LogBox;
        private Label DiscordBotLabel;
        private Label TwitchOauthTokenLabel;
        private TextBox TwitchOauthTokenBox;
        private Label TwitchClientSecretLabel;
        private TextBox TwitchClientSecretBox;
        private Label TwitchClientIDLabel;
        private TextBox TwitchClientIDBox;
        private Label YouTubeOauthTokenLabel;
        private TextBox YouTubeOauthTokenBox;
        private Label YouTubeClientIDLabel;
        private TextBox YouTubeClientIDBox;
        private Label YouTubeClientSecretLabel;
        private TextBox YouTubeClientSecretBox;
        private Label DiscordChannelLabel;
        private TextBox DiscordChannelIDBox;
        private Button SaveBtn;
        private Button ResetBtn;
    }
}