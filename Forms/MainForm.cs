using System;
using System.Drawing;
using System.Windows.Forms;
using SecurIT_Memory.Logic;

namespace SecurIT_Memory.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "SecurIT Memory - Menu Principal";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(15, 15, 30);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            Label lblTitle = new Label
            {
                Text = "SecurIT Memory",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.Cyan,
                Location = new Point(0, 50),
                Size = new Size(400, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblBest = new Label
            {
                Text = $"Record : {ScoreManager.GetMeilleurScore()}",
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 200, 255),
                Location = new Point(0, 100),
                Size = new Size(400, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button btnPlay = CreateMenuButton("JOUER", 150);
            btnPlay.Click += (s, e) => {
                GameForm game = new GameForm(4, 4); // Par défaut 4x4
                game.Show();
                this.Hide();
                game.FormClosed += (s2, e2) => this.Show();
            };

            Button btnOptions = CreateMenuButton("OPTIONS", 220);
            btnOptions.Click += (s, e) => {
                using (OptionsForm opt = new OptionsForm())
                {
                    if (opt.ShowDialog() == DialogResult.OK)
                    {
                        GameForm game = new GameForm(opt.Rows, opt.Cols);
                        game.Show();
                        this.Hide();
                        game.FormClosed += (s2, e2) => this.Show();
                    }
                }
            };
            Button btnQuit = CreateMenuButton("QUITTER", 290);
            btnQuit.Click += (s, e) => Application.Exit();

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblBest);
            this.Controls.Add(btnPlay);
            this.Controls.Add(btnOptions);
            this.Controls.Add(btnQuit);
        }

        private Button CreateMenuButton(string text, int y)
        {
            return new Button
            {
                Text = text,
                Location = new Point(100, y),
                Size = new Size(200, 50),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(40, 40, 80),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }
    }
}
