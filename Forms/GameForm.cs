using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using SecurIT_Memory.Logic;

namespace SecurIT_Memory.Forms
{
    public partial class GameForm : Form
    {
        private JeuMemory gameLogic;
        private System.Windows.Forms.Timer animationTimer;
        private System.Windows.Forms.Timer gameChronoTimer;
        private System.Windows.Forms.Timer flipTimer;

        private int rows, cols;
        private int secondsElapsed = 0;
        private List<CardVisual> cardVisuals = new List<CardVisual>();
        private Rectangle sidebarRect;
        private Rectangle gameAreaRect;
        
        private CardVisual hoveredCard = null;
        private List<CardVisual> selectedVisuals = new List<CardVisual>();

        // Constantes visuelles
        private readonly Color CyberBackground = Color.FromArgb(10, 10, 20);
        private readonly Color SidebarStart = Color.FromArgb(15, 15, 40);
        private readonly Color SidebarEnd = Color.FromArgb(40, 20, 80);
        private readonly Color NeonCyan = Color.Cyan;
        private readonly Color NeonPurple = Color.FromArgb(180, 0, 255);

        // Animation de score
        private float scoreScale = 1.0f;
        private int lastScore = 0;

        public GameForm(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            InitializeGame(rows, cols);
            InitializeAnimations();
        }

        private void InitializeGame(int rows, int cols)
        {
            this.Text = "SecurIT Memory - Cyber Terminal v2.0";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = CyberBackground;

            gameLogic = new JeuMemory();
            List<Image> icons = GenerateHackerIcons((rows * cols) / 2);
            Image backImage = null; // On dessinera le dos manuellement

            gameLogic.InitialiserPartie((rows * cols) / 2, icons, backImage);

            SetupLayout();
        }

        private void SetupLayout()
        {
            int sidebarWidth = 250;
            sidebarRect = new Rectangle(0, 0, sidebarWidth, this.ClientSize.Height);
            gameAreaRect = new Rectangle(sidebarWidth, 0, this.ClientSize.Width - sidebarWidth, this.ClientSize.Height);

            cardVisuals.Clear();
            int padding = 15;
            int cardW = (gameAreaRect.Width - (cols + 1) * padding) / cols;
            int cardH = (gameAreaRect.Height - (rows + 1) * padding) / rows;

            for (int i = 0; i < gameLogic.Cartes.Count; i++)
            {
                int r = i / cols;
                int c = i % cols;
                Rectangle rect = new Rectangle(
                    gameAreaRect.X + padding + c * (cardW + padding),
                    gameAreaRect.Y + padding + r * (cardH + padding),
                    cardW, cardH
                );
                cardVisuals.Add(new CardVisual(gameLogic.Cartes[i], rect));
            }
        }

        private void InitializeAnimations()
        {
            animationTimer = new System.Windows.Forms.Timer { Interval = 16 }; // ~60 FPS
            animationTimer.Tick += (s, e) => {
                UpdateAnimations();
                this.Invalidate();
            };
            animationTimer.Start();

            gameChronoTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            gameChronoTimer.Tick += (s, e) => secondsElapsed++;
            gameChronoTimer.Start();

            flipTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            flipTimer.Tick += FlipTimer_Tick;

            this.MouseMove += GameForm_MouseMove;
            this.MouseDown += GameForm_MouseDown;
            this.Resize += (s, e) => SetupLayout();
        }

        private void UpdateAnimations()
        {
            float dt = 0.05f;
            foreach (var cv in cardVisuals)
            {
                cv.Update(dt);
            }

            if (scoreScale > 1.0f) scoreScale -= 0.05f;
            if (scoreScale < 1.0f) scoreScale = 1.0f;
        }

        private void GameForm_MouseMove(object sender, MouseEventArgs e)
        {
            CardVisual nextHover = cardVisuals.FirstOrDefault(cv => cv.Bounds.Contains(e.Location));
            if (nextHover != hoveredCard)
            {
                if (hoveredCard != null) hoveredCard.IsHovered = false;
                hoveredCard = nextHover;
                if (hoveredCard != null) hoveredCard.IsHovered = true;
            }
        }

        private void GameForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (flipTimer.Enabled || hoveredCard == null) return;
            
            Carte card = hoveredCard.LogicCard;
            if (card.Etat != EtatCarte.Cachee || selectedVisuals.Contains(hoveredCard)) return;

            // Retourner la carte
            card.Etat = EtatCarte.Revelee;
            selectedVisuals.Add(hoveredCard);

            if (selectedVisuals.Count == 2)
            {
                CheckMatch();
            }
        }

        private void CheckMatch()
        {
            bool isMatch = gameLogic.VerifierPaire(selectedVisuals[0].LogicCard, selectedVisuals[1].LogicCard);
            
            if (gameLogic.Score > lastScore)
            {
                scoreScale = 1.5f;
                lastScore = gameLogic.Score;
                selectedVisuals[0].TriggerSuccess();
                selectedVisuals[1].TriggerSuccess();
            }

            if (isMatch)
            {
                selectedVisuals.Clear();
                if (gameLogic.EstPartieTerminee(gameLogic.Cartes.Count / 2))
                {
                    DeclencherVictoire();
                }
            }
            else
            {
                selectedVisuals[0].TriggerError();
                selectedVisuals[1].TriggerError();
                flipTimer.Start();
            }
        }

        private void FlipTimer_Tick(object sender, EventArgs e)
        {
            flipTimer.Stop();
            foreach (var cv in selectedVisuals)
            {
                cv.LogicCard.Etat = EtatCarte.Cachee;
            }
            selectedVisuals.Clear();
        }

        private void DeclencherVictoire()
        {
            gameChronoTimer.Stop();
            ScoreManager.EnregistrerScore(secondsElapsed, gameLogic.Tentatives);
            
            MessageBox.Show($"[ACCÈS ROOT OBTENU]\n\nSystème sécurisé en {secondsElapsed}s.\nScore final: {gameLogic.Score}", 
                "VICTOIRE", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            DrawBackground(g);
            DrawSidebar(g);
            
            foreach (var cv in cardVisuals)
            {
                cv.Draw(g);
            }
        }

        private void DrawBackground(Graphics g)
        {
            // Fond sombre
            g.Clear(CyberBackground);

            // Grille de circuit
            using (Pen gridPen = new Pen(Color.FromArgb(15, 0, 255, 255), 1))
            {
                for (int x = gameAreaRect.Left; x <= gameAreaRect.Right; x += 40)
                    g.DrawLine(gridPen, x, gameAreaRect.Top, x, gameAreaRect.Bottom);
                for (int y = gameAreaRect.Top; y <= gameAreaRect.Bottom; y += 40)
                    g.DrawLine(gridPen, gameAreaRect.Left, y, gameAreaRect.Right, y);
            }

            // Quelques "nodes" de circuit
            using (Brush nodeBrush = new SolidBrush(Color.FromArgb(20, 0, 255, 255)))
            {
                for (int x = gameAreaRect.Left + 40; x < gameAreaRect.Right; x += 120)
                {
                    for (int y = gameAreaRect.Top + 40; y < gameAreaRect.Bottom; y += 120)
                    {
                        g.FillEllipse(nodeBrush, x - 3, y - 3, 6, 6);
                    }
                }
            }
        }

        private void DrawSidebar(Graphics g)
        {
            // Gradient Sidebar
            using (LinearGradientBrush brush = new LinearGradientBrush(sidebarRect, SidebarStart, SidebarEnd, LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, sidebarRect);
            }

            // Ligne de séparation néon
            using (Pen neonLine = new Pen(NeonCyan, 2))
            {
                g.DrawLine(neonLine, sidebarRect.Right, 0, sidebarRect.Right, sidebarRect.Height);
            }

            // Texte de statut
            DrawTextGlow(g, "SYSTEM STATUS: ACTIVE", new Font("Consolas", 10, FontStyle.Bold), Color.Lime, 20, 30);
            
            // Séparateur
            DrawSeparator(g, 60);

            // Score avec animation de scale
            string scoreStr = $"SCORE: {gameLogic.Score:D4}";
            using (Font f = new Font("Consolas", 18, FontStyle.Bold))
            {
                SizeF size = g.MeasureString(scoreStr, f);
                float x = 20;
                float y = 100;
                
                g.TranslateTransform(x + size.Width / 2, y + size.Height / 2);
                g.ScaleTransform(scoreScale, scoreScale);
                DrawTextGlow(g, scoreStr, f, NeonCyan, -size.Width / 2, -size.Height / 2);
                g.ResetTransform();
            }

            DrawTextGlow(g, $"TRIES: {gameLogic.Tentatives}", new Font("Consolas", 12), Color.White, 20, 150);
            DrawSeparator(g, 180);

            DrawTextGlow(g, "TIME ELAPSED", new Font("Consolas", 10), Color.Gray, 20, 220);
            DrawTextGlow(g, $"{secondsElapsed}s", new Font("Consolas", 24, FontStyle.Bold), Color.White, 20, 240);
            
            DrawSeparator(g, 300);
            
            DrawTextGlow(g, "TERMINAL_LOG:", new Font("Consolas", 8), Color.FromArgb(100, 255, 255), 20, 340);
            DrawTextGlow(g, "> Waiting for input...", new Font("Consolas", 8), Color.FromArgb(0, 200, 0), 20, 360);
        }

        private void DrawSeparator(Graphics g, int y)
        {
            using (LinearGradientBrush lineBrush = new LinearGradientBrush(new Rectangle(10, y, 230, 2), Color.Transparent, NeonCyan, 0f))
            {
                ColorBlend cb = new ColorBlend();
                cb.Colors = new Color[] { Color.Transparent, NeonCyan, Color.Transparent };
                cb.Positions = new float[] { 0.0f, 0.5f, 1.0f };
                lineBrush.InterpolationColors = cb;
                g.FillRectangle(lineBrush, 10, y, 230, 2);
            }
        }

        private void DrawTextGlow(Graphics g, string text, Font font, Color color, float x, float y)
        {
            // Glow effect
            for (int i = 1; i <= 3; i++)
            {
                using (Brush glowBrush = new SolidBrush(Color.FromArgb(50 / i, color)))
                {
                    g.DrawString(text, font, glowBrush, x - i, y);
                    g.DrawString(text, font, glowBrush, x + i, y);
                    g.DrawString(text, font, glowBrush, x, y - i);
                    g.DrawString(text, font, glowBrush, x, y + i);
                }
            }
            using (Brush b = new SolidBrush(color))
            {
                g.DrawString(text, font, b, x, y);
            }
        }

        private List<Image> GenerateHackerIcons(int count)
        {
            List<Image> icons = new List<Image>();
            for (int i = 0; i < count; i++)
            {
                Bitmap bmp = new Bitmap(256, 256);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.Clear(Color.Transparent);
                    Pen p = new Pen(NeonCyan, 8);
                    
                    switch (i % 8)
                    {
                        case 0: // Cadenas
                            g.DrawPath(p, GetLockPath()); break;
                        case 1: // Shield
                            g.DrawPath(p, GetShieldPath()); break;
                        case 2: // Server
                            g.DrawRectangle(p, 60, 40, 136, 176);
                            g.DrawLine(p, 60, 100, 196, 100);
                            g.DrawLine(p, 60, 160, 196, 160);
                            break;
                        case 3: // Key
                            g.DrawEllipse(p, 60, 60, 60, 60);
                            g.DrawLine(p, 120, 90, 190, 90);
                            g.DrawLine(p, 160, 90, 160, 120);
                            g.DrawLine(p, 180, 90, 180, 120);
                            break;
                        case 4: // Wifi
                            g.DrawArc(p, 40, 40, 176, 176, 220, 100);
                            g.DrawArc(p, 80, 80, 96, 96, 220, 100);
                            g.FillEllipse(Brushes.Cyan, 118, 180, 20, 20);
                            break;
                        case 5: // Database
                            g.DrawEllipse(p, 60, 40, 136, 60);
                            g.DrawLine(p, 60, 70, 60, 180);
                            g.DrawLine(p, 196, 70, 196, 180);
                            g.DrawArc(p, 60, 150, 136, 60, 0, 180);
                            break;
                        case 6: // Code
                            g.DrawLine(p, 80, 60, 40, 128);
                            g.DrawLine(p, 40, 128, 80, 196);
                            g.DrawLine(p, 176, 60, 216, 128);
                            g.DrawLine(p, 216, 128, 176, 196);
                            g.DrawLine(p, 150, 40, 106, 216);
                            break;
                        case 7: // Password
                            g.DrawString("****", new Font("Arial", 40, FontStyle.Bold), Brushes.Cyan, 60, 90);
                            break;
                    }
                }
                icons.Add(bmp);
            }
            return icons;
        }

        private GraphicsPath GetLockPath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new Rectangle(60, 100, 136, 100));
            path.AddArc(80, 40, 96, 120, 180, 180);
            return path;
        }

        private GraphicsPath GetShieldPath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddPolygon(new Point[] { 
                new Point(128, 40), new Point(200, 70), new Point(200, 160), 
                new Point(128, 220), new Point(56, 160), new Point(56, 70) 
            });
            return path;
        }
    }

    public class CardVisual
    {
        public Carte LogicCard { get; }
        public Rectangle Bounds { get; private set; }
        public bool IsHovered { get; set; }
        
        private float pulsePhase = 0;
        private float hoverGlow = 0;
        private Point shakeOffset = Point.Empty;
        private float shakeTime = 0;
        private float successFlash = 0;
        private List<Particle> particles = new List<Particle>();
        private Random rng = new Random();

        public CardVisual(Carte card, Rectangle bounds)
        {
            this.LogicCard = card;
            this.Bounds = bounds;
        }

        public void Update(float dt)
        {
            pulsePhase += dt * 3;
            if (IsHovered) hoverGlow = Math.Min(1, hoverGlow + dt * 10);
            else hoverGlow = Math.Max(0, hoverGlow - dt * 5);

            if (shakeTime > 0)
            {
                shakeTime -= dt * 2;
                int s = (int)(shakeTime * 10);
                shakeOffset = new Point(rng.Next(-s, s + 1), rng.Next(-s, s + 1));
            }
            else
            {
                shakeOffset = Point.Empty;
            }

            if (successFlash > 0) successFlash -= dt * 2;

            foreach (var p in particles.ToList())
            {
                p.Update(dt);
                if (p.Life <= 0) particles.Remove(p);
            }
        }

        public void TriggerError()
        {
            shakeTime = 1.0f;
        }

        public void TriggerSuccess()
        {
            successFlash = 1.0f;
            for (int i = 0; i < 20; i++)
            {
                particles.Add(new Particle(
                    Bounds.X + Bounds.Width / 2,
                    Bounds.Y + Bounds.Height / 2,
                    Color.Lime
                ));
            }
        }

        public void Draw(Graphics g)
        {
            Rectangle r = Bounds;
            r.Offset(shakeOffset);

            if (hoverGlow > 0)
            {
                r.Inflate((int)(hoverGlow * 5), (int)(hoverGlow * 5));
                using (Brush glowBrush = new SolidBrush(Color.FromArgb((int)(hoverGlow * 40), Color.Cyan)))
                {
                    g.FillRectangle(glowBrush, r.X - 10, r.Y - 10, r.Width + 20, r.Height + 20);
                }
            }

            if (LogicCard.Etat == EtatCarte.Cachee)
            {
                DrawBack(g, r);
            }
            else
            {
                DrawFace(g, r);
            }

            if (successFlash > 0)
            {
                using (Brush b = new SolidBrush(Color.FromArgb((int)(successFlash * 150), Color.Lime)))
                {
                    g.FillRectangle(b, r);
                }
            }

            foreach (var p in particles) p.Draw(g);
        }

        private void DrawBack(Graphics g, Rectangle r)
        {
            using (LinearGradientBrush b = new LinearGradientBrush(r, Color.FromArgb(20, 20, 60), Color.FromArgb(40, 20, 80), 45f))
            {
                g.FillRectangle(b, r);
            }

            using (Pen p = new Pen(Color.FromArgb(30, 0, 255, 255), 1))
            {
                g.DrawLine(p, r.X + 10, r.Y + 10, r.Right - 10, r.Bottom - 10);
                g.DrawLine(p, r.Right - 10, r.Y + 10, r.X + 10, r.Bottom - 10);
                g.DrawEllipse(p, r.X + r.Width/4, r.Y + r.Height/4, r.Width/2, r.Height/2);
            }

            int alpha = (int)(150 + 100 * Math.Sin(pulsePhase));
            Color neon = (LogicCard.Etat == EtatCarte.Trouvee) ? Color.Lime : Color.Cyan;
            using (Pen neonPen = new Pen(Color.FromArgb(alpha, neon), 2 + hoverGlow * 2))
            {
                g.DrawRectangle(neonPen, r);
            }
        }

        private void DrawFace(Graphics g, Rectangle r)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(30, 30, 50)), r);
            
            if (LogicCard.ImageFace != null)
            {
                int iconSize = (int)(Math.Min(r.Width, r.Height) * 0.7f);
                Rectangle iconRect = new Rectangle(
                    r.X + (r.Width - iconSize) / 2,
                    r.Y + (r.Height - iconSize) / 2,
                    iconSize, iconSize
                );
                g.DrawImage(LogicCard.ImageFace, iconRect);
            }

            Color borderColor = (LogicCard.Etat == EtatCarte.Trouvee) ? Color.Lime : Color.White;
            g.DrawRectangle(new Pen(borderColor, 2), r);
        }
    }

    public class Particle
    {
        public float X, Y, VX, VY, Life;
        public Color Color;
        private static Random rng = new Random();

        public Particle(float x, float y, Color c)
        {
            X = x; Y = y;
            Color = c;
            double angle = rng.NextDouble() * Math.PI * 2;
            double speed = rng.NextDouble() * 5 + 2;
            VX = (float)(Math.Cos(angle) * speed);
            VY = (float)(Math.Sin(angle) * speed);
            Life = 1.0f;
        }

        public void Update(float dt)
        {
            X += VX; Y += VY;
            VY += 0.1f; // Gravité
            Life -= dt * 2;
        }

        public void Draw(Graphics g)
        {
            int alpha = (int)(Life * 255);
            if (alpha < 0) alpha = 0;
            using (Brush b = new SolidBrush(Color.FromArgb(alpha, Color)))
            {
                g.FillEllipse(b, X - 2, Y - 2, 4, 4);
            }
        }
    }
}