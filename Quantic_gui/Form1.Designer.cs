using System.Windows.Forms;

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

            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            PlayerXPlayer.Location = new Point(screenWidth/4, screenHeight/2 - 50);
            PlayerXPlayer.Name = "PlayerXPlayer";
            PlayerXPlayer.Size = new Size(500, 500);
            PlayerXPlayer.TabIndex = 0;
            PlayerXPlayer.Text = "Player x Player";
            PlayerXPlayer.UseVisualStyleBackColor = true;
            PlayerXPlayer.Click += SetPlayerXPlayer;
            // 
            // PlayerXComputer
            // 
            PlayerXComputer.Location = new Point(3* screenWidth/4 - 500, screenHeight /2 -50);
            PlayerXComputer.Name = "PlayerXComputer";
            PlayerXComputer.Size = new Size(500, 500);
            PlayerXComputer.TabIndex = 1;
            PlayerXComputer.Text = "Player x Computer";
            PlayerXComputer.UseVisualStyleBackColor = true;
            PlayerXComputer.Click += SetPlayerXComputer;

            pictureBox1 = new PictureBox();
            pictureBox1.Location = new Point(screenWidth/2 - 350, screenHeight/4);
            pictureBox1.Image = Image.FromFile("logo.png");
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1600, 1000);
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;

          

            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoValidate = AutoValidate.EnableAllowFocusChange;
            ClientSize = new Size(2297, 807);
            Controls.Add(PlayerXComputer);
            Controls.Add(PlayerXPlayer);
            Controls.Add(pictureBox1);
            Name = "Form1";
            Text = "Quantik";
            Load += Form1_Load;
            ResumeLayout(false);
            BackColor = Color.White;
        }

        #endregion

        private Button PlayerXPlayer;
        private Button PlayerXComputer;
        private PictureBox pictureBox1;
    }
}