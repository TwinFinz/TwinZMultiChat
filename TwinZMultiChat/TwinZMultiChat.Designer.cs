namespace TwinZMultiChat
{
    partial class MainForm
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
            this.DiscordBotTokenBox = new System.Windows.Forms.TextBox();
            this.StopBtn = new System.Windows.Forms.Button();
            this.StartBtn = new System.Windows.Forms.Button();
            this.LogBox = new System.Windows.Forms.RichTextBox();
            this.DiscordBotLabel = new System.Windows.Forms.Label();
            this.TwitchBotNameLabel = new System.Windows.Forms.Label();
            this.TwitchBotNameBox = new System.Windows.Forms.TextBox();
            this.TwitchClientSecretLabel = new System.Windows.Forms.Label();
            this.TwitchClientSecretBox = new System.Windows.Forms.TextBox();
            this.TwitchClientIDLabel = new System.Windows.Forms.Label();
            this.TwitchClientIDBox = new System.Windows.Forms.TextBox();
            this.DiscordChannelLabel = new System.Windows.Forms.Label();
            this.DiscordChannelIDBox = new System.Windows.Forms.TextBox();
            this.SaveBtn = new System.Windows.Forms.Button();
            this.ResetBtn = new System.Windows.Forms.Button();
            this.YouTubeApplicationNameLabel = new System.Windows.Forms.Label();
            this.YouTubeApplicationNameBox = new System.Windows.Forms.TextBox();
            this.YouTubeCheckBox = new System.Windows.Forms.CheckBox();
            this.DiscordCheckBox = new System.Windows.Forms.CheckBox();
            this.TwitchCheckBox = new System.Windows.Forms.CheckBox();
            this.KickCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // DiscordBotTokenBox
            // 
            this.DiscordBotTokenBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DiscordBotTokenBox.Location = new System.Drawing.Point(940, 43);
            this.DiscordBotTokenBox.Name = "DiscordBotTokenBox";
            this.DiscordBotTokenBox.Size = new System.Drawing.Size(314, 23);
            this.DiscordBotTokenBox.TabIndex = 0;
            this.DiscordBotTokenBox.TextChanged += new System.EventHandler(this.DiscordBotBoxChanged);
            // 
            // StopBtn
            // 
            this.StopBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.StopBtn.Location = new System.Drawing.Point(1098, 630);
            this.StopBtn.Name = "StopBtn";
            this.StopBtn.Size = new System.Drawing.Size(154, 39);
            this.StopBtn.TabIndex = 1;
            this.StopBtn.Text = "Stop";
            this.StopBtn.UseVisualStyleBackColor = true;
            this.StopBtn.Click += new System.EventHandler(this.StopBtn_Click);
            // 
            // StartBtn
            // 
            this.StartBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.StartBtn.Location = new System.Drawing.Point(938, 630);
            this.StartBtn.Name = "StartBtn";
            this.StartBtn.Size = new System.Drawing.Size(154, 39);
            this.StartBtn.TabIndex = 2;
            this.StartBtn.Text = "Start";
            this.StartBtn.UseVisualStyleBackColor = true;
            this.StartBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // LogBox
            // 
            this.LogBox.Location = new System.Drawing.Point(12, 25);
            this.LogBox.Name = "LogBox";
            this.LogBox.ReadOnly = true;
            this.LogBox.Size = new System.Drawing.Size(456, 644);
            this.LogBox.TabIndex = 3;
            this.LogBox.Text = "";
            // 
            // DiscordBotLabel
            // 
            this.DiscordBotLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DiscordBotLabel.AutoSize = true;
            this.DiscordBotLabel.Location = new System.Drawing.Point(938, 25);
            this.DiscordBotLabel.Name = "DiscordBotLabel";
            this.DiscordBotLabel.Size = new System.Drawing.Size(102, 15);
            this.DiscordBotLabel.TabIndex = 4;
            this.DiscordBotLabel.Text = "Discord Bot Token";
            // 
            // TwitchBotNameLabel
            // 
            this.TwitchBotNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TwitchBotNameLabel.AutoSize = true;
            this.TwitchBotNameLabel.Location = new System.Drawing.Point(940, 113);
            this.TwitchBotNameLabel.Name = "TwitchBotNameLabel";
            this.TwitchBotNameLabel.Size = new System.Drawing.Size(97, 15);
            this.TwitchBotNameLabel.TabIndex = 6;
            this.TwitchBotNameLabel.Text = "Twitch Bot Name";
            // 
            // TwitchBotNameBox
            // 
            this.TwitchBotNameBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TwitchBotNameBox.Location = new System.Drawing.Point(940, 131);
            this.TwitchBotNameBox.Name = "TwitchBotNameBox";
            this.TwitchBotNameBox.Size = new System.Drawing.Size(314, 23);
            this.TwitchBotNameBox.TabIndex = 5;
            this.TwitchBotNameBox.TextChanged += new System.EventHandler(this.TwitchBotNameBoxChanged);
            // 
            // TwitchClientSecretLabel
            // 
            this.TwitchClientSecretLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TwitchClientSecretLabel.AutoSize = true;
            this.TwitchClientSecretLabel.Location = new System.Drawing.Point(941, 201);
            this.TwitchClientSecretLabel.Name = "TwitchClientSecretLabel";
            this.TwitchClientSecretLabel.Size = new System.Drawing.Size(110, 15);
            this.TwitchClientSecretLabel.TabIndex = 8;
            this.TwitchClientSecretLabel.Text = "Twitch Client Secret";
            // 
            // TwitchClientSecretBox
            // 
            this.TwitchClientSecretBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TwitchClientSecretBox.Location = new System.Drawing.Point(941, 219);
            this.TwitchClientSecretBox.Name = "TwitchClientSecretBox";
            this.TwitchClientSecretBox.Size = new System.Drawing.Size(314, 23);
            this.TwitchClientSecretBox.TabIndex = 7;
            this.TwitchClientSecretBox.TextChanged += new System.EventHandler(this.TwitchClientSecretBoxChanged);
            // 
            // TwitchClientIDLabel
            // 
            this.TwitchClientIDLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TwitchClientIDLabel.AutoSize = true;
            this.TwitchClientIDLabel.Location = new System.Drawing.Point(940, 157);
            this.TwitchClientIDLabel.Name = "TwitchClientIDLabel";
            this.TwitchClientIDLabel.Size = new System.Drawing.Size(89, 15);
            this.TwitchClientIDLabel.TabIndex = 10;
            this.TwitchClientIDLabel.Text = "Twitch Client ID";
            // 
            // TwitchClientIDBox
            // 
            this.TwitchClientIDBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TwitchClientIDBox.Location = new System.Drawing.Point(940, 175);
            this.TwitchClientIDBox.Name = "TwitchClientIDBox";
            this.TwitchClientIDBox.Size = new System.Drawing.Size(314, 23);
            this.TwitchClientIDBox.TabIndex = 9;
            this.TwitchClientIDBox.TextChanged += new System.EventHandler(this.TwitchClientIDBoxChanged);
            // 
            // DiscordChannelLabel
            // 
            this.DiscordChannelLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DiscordChannelLabel.AutoSize = true;
            this.DiscordChannelLabel.Location = new System.Drawing.Point(940, 69);
            this.DiscordChannelLabel.Name = "DiscordChannelLabel";
            this.DiscordChannelLabel.Size = new System.Drawing.Size(108, 15);
            this.DiscordChannelLabel.TabIndex = 18;
            this.DiscordChannelLabel.Text = "Discord Channel ID";
            // 
            // DiscordChannelIDBox
            // 
            this.DiscordChannelIDBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DiscordChannelIDBox.Location = new System.Drawing.Point(940, 87);
            this.DiscordChannelIDBox.Name = "DiscordChannelIDBox";
            this.DiscordChannelIDBox.Size = new System.Drawing.Size(314, 23);
            this.DiscordChannelIDBox.TabIndex = 17;
            this.DiscordChannelIDBox.TextChanged += new System.EventHandler(this.DiscordChannelIDBoxChanged);
            // 
            // SaveBtn
            // 
            this.SaveBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveBtn.Location = new System.Drawing.Point(1098, 600);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Size = new System.Drawing.Size(154, 24);
            this.SaveBtn.TabIndex = 19;
            this.SaveBtn.Text = "Save";
            this.SaveBtn.UseVisualStyleBackColor = true;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // ResetBtn
            // 
            this.ResetBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ResetBtn.Location = new System.Drawing.Point(939, 600);
            this.ResetBtn.Name = "ResetBtn";
            this.ResetBtn.Size = new System.Drawing.Size(154, 24);
            this.ResetBtn.TabIndex = 20;
            this.ResetBtn.Text = "Reset";
            this.ResetBtn.UseVisualStyleBackColor = true;
            this.ResetBtn.Click += new System.EventHandler(this.ResetBtn_Click);
            // 
            // YouTubeApplicationNameLabel
            // 
            this.YouTubeApplicationNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.YouTubeApplicationNameLabel.AutoSize = true;
            this.YouTubeApplicationNameLabel.Location = new System.Drawing.Point(941, 245);
            this.YouTubeApplicationNameLabel.Name = "YouTubeApplicationNameLabel";
            this.YouTubeApplicationNameLabel.Size = new System.Drawing.Size(152, 15);
            this.YouTubeApplicationNameLabel.TabIndex = 24;
            this.YouTubeApplicationNameLabel.Text = "YouTube Application Name";
            // 
            // YouTubeApplicationNameBox
            // 
            this.YouTubeApplicationNameBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.YouTubeApplicationNameBox.Location = new System.Drawing.Point(941, 263);
            this.YouTubeApplicationNameBox.Name = "YouTubeApplicationNameBox";
            this.YouTubeApplicationNameBox.Size = new System.Drawing.Size(314, 23);
            this.YouTubeApplicationNameBox.TabIndex = 23;
            this.YouTubeApplicationNameBox.TextChanged += new System.EventHandler(this.YouTubeApplicationNameBoxChanged);
            // 
            // YouTubeCheckBox
            // 
            this.YouTubeCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.YouTubeCheckBox.AutoSize = true;
            this.YouTubeCheckBox.Location = new System.Drawing.Point(1020, 575);
            this.YouTubeCheckBox.Name = "YouTubeCheckBox";
            this.YouTubeCheckBox.Size = new System.Drawing.Size(72, 19);
            this.YouTubeCheckBox.TabIndex = 27;
            this.YouTubeCheckBox.Text = "YouTube";
            this.YouTubeCheckBox.UseVisualStyleBackColor = true;
            this.YouTubeCheckBox.CheckedChanged += new System.EventHandler(this.YouTubeCheckBox_CheckedChanged);
            // 
            // DiscordCheckBox
            // 
            this.DiscordCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DiscordCheckBox.AutoSize = true;
            this.DiscordCheckBox.Location = new System.Drawing.Point(941, 575);
            this.DiscordCheckBox.Name = "DiscordCheckBox";
            this.DiscordCheckBox.Size = new System.Drawing.Size(66, 19);
            this.DiscordCheckBox.TabIndex = 28;
            this.DiscordCheckBox.Text = "Discord";
            this.DiscordCheckBox.UseVisualStyleBackColor = true;
            this.DiscordCheckBox.CheckedChanged += new System.EventHandler(this.DiscordCheckBox_CheckedChanged);
            // 
            // TwitchCheckBox
            // 
            this.TwitchCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.TwitchCheckBox.AutoSize = true;
            this.TwitchCheckBox.Location = new System.Drawing.Point(1098, 575);
            this.TwitchCheckBox.Name = "TwitchCheckBox";
            this.TwitchCheckBox.Size = new System.Drawing.Size(60, 19);
            this.TwitchCheckBox.TabIndex = 29;
            this.TwitchCheckBox.Text = "Twitch";
            this.TwitchCheckBox.UseVisualStyleBackColor = true;
            this.TwitchCheckBox.CheckedChanged += new System.EventHandler(this.TwitchCheckBox_CheckedChanged);
            // 
            // KickCheckBox
            // 
            this.KickCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.KickCheckBox.AutoSize = true;
            this.KickCheckBox.Location = new System.Drawing.Point(1164, 575);
            this.KickCheckBox.Name = "KickCheckBox";
            this.KickCheckBox.Size = new System.Drawing.Size(48, 19);
            this.KickCheckBox.TabIndex = 30;
            this.KickCheckBox.Text = "Kick";
            this.KickCheckBox.UseVisualStyleBackColor = true;
            this.KickCheckBox.CheckedChanged += new System.EventHandler(this.KickCheckBox_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.KickCheckBox);
            this.Controls.Add(this.TwitchCheckBox);
            this.Controls.Add(this.DiscordCheckBox);
            this.Controls.Add(this.YouTubeCheckBox);
            this.Controls.Add(this.YouTubeApplicationNameLabel);
            this.Controls.Add(this.YouTubeApplicationNameBox);
            this.Controls.Add(this.ResetBtn);
            this.Controls.Add(this.SaveBtn);
            this.Controls.Add(this.DiscordChannelLabel);
            this.Controls.Add(this.DiscordChannelIDBox);
            this.Controls.Add(this.TwitchClientIDLabel);
            this.Controls.Add(this.TwitchClientIDBox);
            this.Controls.Add(this.TwitchClientSecretLabel);
            this.Controls.Add(this.TwitchClientSecretBox);
            this.Controls.Add(this.TwitchBotNameLabel);
            this.Controls.Add(this.TwitchBotNameBox);
            this.Controls.Add(this.DiscordBotLabel);
            this.Controls.Add(this.LogBox);
            this.Controls.Add(this.StartBtn);
            this.Controls.Add(this.StopBtn);
            this.Controls.Add(this.DiscordBotTokenBox);
            this.Name = "MainForm";
            this.Text = "TwinZMultiChat";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox DiscordBotTokenBox;
        private Button StopBtn;
        private Button StartBtn;
        private RichTextBox LogBox;
        private Label DiscordBotLabel;
        private Label TwitchBotNameLabel;
        private TextBox TwitchBotNameBox;
        private Label TwitchClientSecretLabel;
        private TextBox TwitchClientSecretBox;
        private Label TwitchClientIDLabel;
        private TextBox TwitchClientIDBox;
        private Label DiscordChannelLabel;
        private TextBox DiscordChannelIDBox;
        private Button SaveBtn;
        private Button ResetBtn;
        private Label YouTubeApplicationNameLabel;
        private TextBox YouTubeApplicationNameBox;
        private CheckBox YouTubeCheckBox;
        private CheckBox DiscordCheckBox;
        private CheckBox TwitchCheckBox;
        private CheckBox KickCheckBox;
    }
}