using System;
using System.Drawing;
using System.Windows.Forms;
using SecurIT_Memory.Logic;
using SecurIT_Memory.UIComponents;

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
            this.Size = new Size(400, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(15, 15, 30);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            var user = AuthService.UtilisateurConnecte;

            Label lblTitle = new Label
            {
                Text = "SecurIT Memory",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.Cyan,
                Location = new Point(0, 40),
                Size = new Size(400, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblWelcome = new Label
            {
                Text = $"Bienvenue, {user?.Nom ?? "Agent"}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 100),
                Size = new Size(400, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblBest = new Label
            {
                Text = user != null ? user.ToString() : "Aucun record",
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 200, 255),
                Location = new Point(0, 130),
                Size = new Size(400, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            CyberButton btnPlay = new CyberButton { Text = "DÉMARRER", Location = new Point(100, 200), Size = new Size(200, 50) };
            btnPlay.Click += (s, e) => {
                GameForm game = new GameForm(4, 4);
                game.Show();
                this.Hide();
                game.FormClosed += (s2, e2) => {
                    lblBest.Text = AuthService.UtilisateurConnecte?.ToString();
                    this.Show();
                };
            };

            CyberButton btnOptions = new CyberButton { Text = "DIFFICULTÉ", Location = new Point(100, 270), Size = new Size(200, 50), BackColor = Color.Transparent };
            btnOptions.Click += (s, e) => {
                using (OptionsForm opt = new OptionsForm())
                {
                    if (opt.ShowDialog() == DialogResult.OK)
                    {
                        GameForm game = new GameForm(opt.Rows, opt.Cols);
                        game.Show();
                        this.Hide();
                        game.FormClosed += (s2, e2) => {
                            lblBest.Text = AuthService.UtilisateurConnecte?.ToString();
                            this.Show();
                        };
                    }
                }
            };

            CyberButton btnLogout = new CyberButton { Text = "DÉCONNEXION", Location = new Point(100, 340), Size = new Size(200, 50), BackColor = Color.FromArgb(40, 40, 60) };
            btnLogout.Click += (s, e) => {
                this.Close(); // Retourne au Program.cs qui rebouclera si on veut, mais ici on ferme simplement
            };

            CyberButton btnQuit = new CyberButton { Text = "QUITTER", Location = new Point(100, 410), Size = new Size(200, 50), BackColor = Color.DarkRed };
            btnQuit.Click += (s, e) => Application.Exit();

            this.Controls.AddRange(new Control[] { lblTitle, lblWelcome, lblBest, btnPlay, btnOptions, btnLogout, btnQuit });
        }
    }
}
