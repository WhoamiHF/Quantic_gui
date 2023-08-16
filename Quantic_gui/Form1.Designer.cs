namespace Quantic_gui
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
            PlayerXPlayer = new Button();
            PlayerXComputer = new Button();
            SuspendLayout();
            // 
            // PlayerXPlayer
            // 
            PlayerXPlayer.Location = new Point(654, 509);
            PlayerXPlayer.Name = "PlayerXPlayer";
            PlayerXPlayer.Size = new Size(221, 46);
            PlayerXPlayer.TabIndex = 0;
            PlayerXPlayer.Text = "Player x Player";
            PlayerXPlayer.UseVisualStyleBackColor = true;
            PlayerXPlayer.Click += SetPlayerXPlayer;
            // 
            // PlayerXComputer
            // 
            PlayerXComputer.Location = new Point(1334, 509);
            PlayerXComputer.Name = "PlayerXComputer";
            PlayerXComputer.Size = new Size(258, 46);
            PlayerXComputer.TabIndex = 1;
            PlayerXComputer.Text = "Player x Computer";
            PlayerXComputer.UseVisualStyleBackColor = true;
            PlayerXComputer.Click += SetPlayerXComputer;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoValidate = AutoValidate.EnableAllowFocusChange;
            ClientSize = new Size(2297, 807);
            Controls.Add(PlayerXComputer);
            Controls.Add(PlayerXPlayer);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button PlayerXPlayer;
        private Button PlayerXComputer;
    }
}