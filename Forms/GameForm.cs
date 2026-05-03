using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SecurIT_Memory.Logic;

namespace SecurIT_Memory.Forms
{
    public partial class GameForm : Form
    {
        private JeuMemory gameLogic;
        private TableLayoutPanel gridLayout;
        private Label lblScore;
        private Label lblChrono;
        private System.Windows.Forms.Timer gameTimer;
        private System.Windows.Forms.Timer flipTimer;
        
        private List<PictureBox> selectedPictures = new List<PictureBox>();
        private List<Carte> selectedCards = new List<Carte>();
        private int secondsElapsed = 0;

        public GameForm(int rows, int cols)
        {
            InitializeComponents(rows, cols);
            gameLogic = new JeuMemory();
            
            // Simulation d'icônes pour l'instant (à remplacer par de vraies images)
            List<Image> icons = GeneratePlaceholderIcons((rows * cols) / 2);
            Image backImage = CreateBackImage();

            gameLogic.InitialiserPartie((rows * cols) / 2, icons, backImage);
            PopulateGrid(rows, cols);
        }

        private void InitializeComponents(int rows, int cols)
        {
            this.Text = "SecurIT Memory - Game Board";
            this.Size = new Size(800, 600);
            this.BackColor = Color.FromArgb(20, 20, 40); // Dark Cyber Theme

            // Layout pour les infos (Score / Temps)
            Panel infoPanel = new Panel { Dock = DockStyle.Top, Height = 50 };
            lblScore = new Label { Text = "Score: 0", ForeColor = Color.White, Location = new Point(10, 15), AutoSize = true };
            lblChrono = new Label { Text = "Temps: 0s", ForeColor = Color.White, Location = new Point(150, 15), AutoSize = true };
            infoPanel.Controls.Add(lblScore);
            infoPanel.Controls.Add(lblChrono);
            this.Controls.Add(infoPanel);

            // Grille dynamique
            gridLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = rows,
                ColumnCount = cols,
                Padding = new Padding(20)
            };

            // Ajustement automatique des tailles de lignes/colonnes
            for (int i = 0; i < rows; i++) gridLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / rows));
            for (int i = 0; i < cols; i++) gridLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / cols));

            this.Controls.Add(gridLayout);

            // Timers
            gameTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            gameTimer.Tick += (s, e) => { secondsElapsed++; lblChrono.Text = $"Temps: {secondsElapsed}s"; };
            gameTimer.Start();

            flipTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            flipTimer.Tick += FlipTimer_Tick;
        }

        private void PopulateGrid(int rows, int cols)
        {
            int cardIndex = 0;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Carte card = gameLogic.Cartes[cardIndex];
                    PictureBox pb = new PictureBox
                    {
                        Dock = DockStyle.Fill,
                        BorderStyle = BorderStyle.FixedSingle,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Image = card.GetImageAffichee(),
                        Tag = card, // On lie l'objet Carte à la PictureBox
                        Cursor = Cursors.Hand
                    };
                    pb.Click += Card_Click;
                    gridLayout.Controls.Add(pb, c, r);
                    cardIndex++;
                }
            }
        }

        private void Card_Click(object sender, EventArgs e)
        {
            if (flipTimer.Enabled) return; // Bloque les clics pendant le délai

            PictureBox pb = (PictureBox)sender;
            Carte card = (Carte)pb.Tag;

            if (card.Etat != EtatCarte.Cachee || selectedPictures.Contains(pb)) return;

            // Retourner la carte
            card.Etat = EtatCarte.Revelee;
            pb.Image = card.GetImageAffichee();
            selectedPictures.Add(pb);
            selectedCards.Add(card);

            if (selectedCards.Count == 2)
            {
                CheckMatch();
            }
        }

        private void CheckMatch()
        {
            bool isMatch = gameLogic.VerifierPaire(selectedCards[0], selectedCards[1]);
            lblScore.Text = $"Score: {gameLogic.Score} (Essais: {gameLogic.Tentatives})";

            if (isMatch)
            {
                selectedPictures.Clear();
                selectedCards.Clear();
                if (gameLogic.EstPartieTerminee(gameLogic.Cartes.Count / 2))
                {
                    DeclencherVictoire();
                }
            }
            else
            {
                flipTimer.Start();
            }
        }

        private void DeclencherVictoire()
        {
            gameTimer.Stop();
            ScoreManager.EnregistrerScore(secondsElapsed, gameLogic.Tentatives);

            // Effet visuel : Toutes les cartes deviennent vertes
            foreach (Control c in gridLayout.Controls)
            {
                if (c is PictureBox pb)
                {
                    Bitmap bmp = new Bitmap(pb.Image);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.DrawRectangle(new Pen(Color.Lime, 8), 0, 0, bmp.Width, bmp.Height);
                    }
                    pb.Image = bmp;
                }
            }

            MessageBox.Show($"[MISSION RÉUSSIE]\n\nSystème sécurisé en {secondsElapsed} secondes.\nTentatives : {gameLogic.Tentatives}\n\nVotre score a été enregistré.", 
                            "Victoire - SecurIT Memory", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void FlipTimer_Tick(object sender, EventArgs e)
        {
            flipTimer.Stop();
            foreach (var pb in selectedPictures)
            {
                Carte c = (Carte)pb.Tag;
                c.Etat = EtatCarte.Cachee;
                pb.Image = c.GetImageAffichee();
            }
            selectedPictures.Clear();
            selectedCards.Clear();
        }

        // --- Fonctions utilitaires pour les graphiques ---

        private List<Image> GeneratePlaceholderIcons(int count)
        {
            List<Image> icons = new List<Image>();
            for (int i = 0; i < count; i++)
            {
                Bitmap bmp = new Bitmap(128, 128);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.Clear(Color.FromArgb(30, 30, 60));

                    Pen neonPen = new Pen(Color.Cyan, 3);
                    
                    // Dessin d'icônes thématiques distinctes (au moins 8 nécessaires pour une grille 4x4)
                    switch (i % 8)
                    {
                        case 0: // Cadenas
                            g.DrawRectangle(neonPen, 40, 55, 48, 40);
                            g.DrawArc(neonPen, 45, 30, 38, 50, 180, 180);
                            break;
                        case 1: // Bouclier / Hexagone
                            Point[] shield = { new Point(64, 25), new Point(100, 40), new Point(100, 75), new Point(64, 105), new Point(28, 75), new Point(28, 40) };
                            g.DrawPolygon(neonPen, shield);
                            break;
                        case 2: // Serveur
                            g.DrawRectangle(neonPen, 32, 22, 64, 84);
                            g.DrawLine(neonPen, 32, 50, 96, 50);
                            g.DrawLine(neonPen, 32, 78, 96, 78);
                            g.FillEllipse(Brushes.Lime, 82, 30, 8, 8);
                            g.FillEllipse(Brushes.Lime, 82, 58, 8, 8);
                            g.FillEllipse(Brushes.Red,  82, 86, 8, 8);
                            break;
                        case 3: // Clé
                            g.DrawEllipse(neonPen, 35, 35, 30, 30);
                            g.DrawLine(neonPen, 65, 50, 100, 50);
                            g.DrawLine(neonPen, 85, 50, 85, 65);
                            g.DrawLine(neonPen, 95, 50, 95, 65);
                            break;
                        case 4: // WiFi (Le seul, pas de "Radar")
                            g.DrawArc(neonPen, 14, 14, 100, 100, 210, 120);
                            g.DrawArc(neonPen, 30, 30, 68, 68, 210, 120);
                            g.DrawArc(neonPen, 46, 46, 36, 36, 210, 120);
                            g.FillEllipse(Brushes.Cyan, 61, 88, 6, 6);
                            break;
                        case 5: // Database
                            g.DrawEllipse(neonPen, 32, 18, 64, 22);
                            g.DrawLine(neonPen, 32, 29, 32, 95);
                            g.DrawLine(neonPen, 96, 29, 96, 95);
                            g.DrawArc(neonPen, 32, 84, 64, 22, 0, 180);
                            g.DrawArc(neonPen, 32, 52, 64, 22, 0, 180);
                            break;
                        case 6: // Code
                            g.DrawLine(neonPen, 44, 35, 22, 64);
                            g.DrawLine(neonPen, 22, 64, 44, 93);
                            g.DrawLine(neonPen, 84, 35, 106, 64);
                            g.DrawLine(neonPen, 106, 64, 84, 93);
                            g.DrawLine(neonPen, 74, 22, 54, 106);
                            break;
                        case 7: // Password
                            g.DrawRectangle(neonPen, 16, 44, 96, 40);
                            g.DrawString("* * * *", new Font("Consolas", 18, FontStyle.Bold), Brushes.Cyan, new PointF(18, 48));
                            break;
                    }
                    // Bordure de la carte
                    g.DrawRectangle(new Pen(Color.Cyan, 1), 5, 5, 118, 118);
                }
                icons.Add(bmp);
            }
            return icons;
        }

        private Image CreateBackImage()
        {
            Bitmap bmp = new Bitmap(128, 128);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(10, 10, 30));
                // Dessin d'un motif de circuit imprimé
                Pen gridPen = new Pen(Color.FromArgb(40, 40, 80), 1);
                for (int x = 0; x < 128; x += 16) g.DrawLine(gridPen, x, 0, x, 128);
                for (int y = 0; y < 128; y += 16) g.DrawLine(gridPen, 0, y, 128, y);
                
                g.DrawString("SECURE", new Font("Consolas", 14, FontStyle.Bold), new SolidBrush(Color.FromArgb(60, 60, 120)), new PointF(30, 50));
                g.DrawRectangle(new Pen(Color.FromArgb(60, 60, 120), 2), 10, 10, 108, 108);
            }
            return bmp;
        }
    }
}