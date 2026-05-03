using System;
using System.Drawing;
using System.Windows.Forms;
using SecurIT_Memory.Logic;
using SecurIT_Memory.UIComponents;

namespace SecurIT_Memory.Forms
{
    public partial class MainForm : Form
    {
        private Label lblBest = null!;
        private ListView leaderboardList = null!;

        public MainForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "SecurIT Memory - Menu Principal";
            this.Size = new Size(720, 600);
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
                Size = new Size(720, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblWelcome = new Label
            {
                Text = $"Bienvenue, {user?.Nom ?? "Agent"}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 100),
                Size = new Size(720, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblBest = new Label
            {
                Text = user != null ? user.ToString() : "Aucun record",
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 200, 255),
                Location = new Point(0, 130),
                Size = new Size(720, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            CyberButton btnPlay = new CyberButton { Text = "DÉMARRER", Location = new Point(80, 200), Size = new Size(200, 50) };
            btnPlay.Click += (s, e) => {
                GameForm game = new GameForm(4, 4);
                game.Show();
                this.Hide();
                game.FormClosed += (s2, e2) => {
                    RefreshLeaderboard();
                    this.Show();
                };
            };

            CyberButton btnOptions = new CyberButton { Text = "DIFFICULTÉ", Location = new Point(80, 270), Size = new Size(200, 50), BackColor = Color.Transparent };
            btnOptions.Click += (s, e) => {
                using (OptionsForm opt = new OptionsForm())
                {
                    if (opt.ShowDialog() == DialogResult.OK)
                    {
                        GameForm game = new GameForm(opt.Rows, opt.Cols);
                        game.Show();
                        this.Hide();
                        game.FormClosed += (s2, e2) => {
                            RefreshLeaderboard();
                            this.Show();
                        };
                    }
                }
            };

            CyberButton btnRefresh = new CyberButton { Text = "RAFRAÎCHIR", Location = new Point(80, 340), Size = new Size(200, 50), BackColor = Color.FromArgb(20, 60, 80) };
            btnRefresh.Click += (s, e) => RefreshLeaderboard();

            CyberButton btnLogout = new CyberButton { Text = "DÉCONNEXION", Location = new Point(80, 410), Size = new Size(200, 50), BackColor = Color.FromArgb(40, 40, 60) };
            btnLogout.Click += (s, e) => {
                this.Close(); // Retourne au Program.cs qui rebouclera si on veut, mais ici on ferme simplement
            };

            CyberButton btnQuit = new CyberButton { Text = "QUITTER", Location = new Point(80, 480), Size = new Size(200, 50), BackColor = Color.DarkRed };
            btnQuit.Click += (s, e) => Application.Exit();

            Label lblLeaderboard = new Label
            {
                Text = "CLASSEMENT SQL",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.Cyan,
                Location = new Point(330, 185),
                Size = new Size(330, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            leaderboardList = new ListView
            {
                Location = new Point(330, 220),
                Size = new Size(330, 310),
                BackColor = Color.FromArgb(10, 10, 25),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 9),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            leaderboardList.Columns.Add("#", 35);
            leaderboardList.Columns.Add("Nom", 115);
            leaderboardList.Columns.Add("Temps", 70);
            leaderboardList.Columns.Add("Essais", 70);

            this.Controls.AddRange(new Control[] { lblTitle, lblWelcome, lblBest, btnPlay, btnOptions, btnRefresh, btnLogout, btnQuit, lblLeaderboard, leaderboardList });
            RefreshLeaderboard();
        }

        private void RefreshLeaderboard()
        {
            lblBest.Text = AuthService.UtilisateurConnecte?.ToString() ?? "Aucun record";
            leaderboardList.Items.Clear();

            var scores = ScoreManager.GetLeaderboard(10);
            foreach (var score in scores)
            {
                var item = new ListViewItem(score.Rang.ToString());
                item.SubItems.Add(score.Nom);
                item.SubItems.Add($"{score.Temps}s");
                item.SubItems.Add(score.Essais.ToString());
                leaderboardList.Items.Add(item);
            }

            if (scores.Count == 0)
            {
                var item = new ListViewItem("-");
                item.SubItems.Add("Aucun score");
                item.SubItems.Add("-");
                item.SubItems.Add("-");
                leaderboardList.Items.Add(item);
            }
        }
    }
}
