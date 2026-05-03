using System;
using System.Drawing;
using System.Windows.Forms;
using SecurIT_Memory.Logic;
using SecurIT_Memory.UIComponents;

namespace SecurIT_Memory.Forms
{
    public partial class LoginForm : Form
    {
        private TextBox txtUsername;
        private Label lblError;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "SecurIT - Authentification";
            this.Size = new Size(400, 550);
            this.BackColor = Color.FromArgb(10, 10, 25);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Titre
            Label lblTitle = new Label
            {
                Text = "SecurIT\nMEMORY",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.Cyan,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(400, 120),
                Location = new Point(0, 50)
            };

            // Champ de saisie
            Label lblUser = new Label { Text = "NOM D'UTILISATEUR", ForeColor = Color.Gray, Location = new Point(50, 200), AutoSize = true };
            txtUsername = new TextBox
            {
                Location = new Point(50, 225),
                Size = new Size(300, 30),
                BackColor = Color.FromArgb(20, 20, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 12)
            };

            lblError = new Label
            {
                ForeColor = Color.Salmon,
                Location = new Point(50, 260),
                Size = new Size(300, 20),
                TextAlign = ContentAlignment.TopCenter,
                Text = ""
            };

            // Boutons
            CyberButton btnLogin = new CyberButton { Text = "CONNEXION", Location = new Point(100, 300) };
            btnLogin.Click += BtnLogin_Click;

            CyberButton btnSignup = new CyberButton { Text = "S'INSCRIRE", Location = new Point(100, 360), BackColor = Color.Transparent };
            btnSignup.Click += BtnSignup_Click;

            CyberButton btnExit = new CyberButton { Text = "QUITTER", Location = new Point(100, 420), BackColor = Color.DarkRed };
            btnExit.Click += (s, e) => Application.Exit();

            this.Controls.AddRange(new Control[] { lblTitle, lblUser, txtUsername, lblError, btnLogin, btnSignup, btnExit });
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (AuthService.Connexion(txtUsername.Text))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                lblError.Text = "Utilisateur inconnu.";
            }
        }

        private void BtnSignup_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                lblError.Text = "Veuillez entrer un nom.";
                return;
            }
            AuthService.Inscription(txtUsername.Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
