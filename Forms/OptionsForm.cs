using System;
using System.Drawing;
using System.Windows.Forms;

namespace SecurIT_Memory.Forms
{
    public partial class OptionsForm : Form
    {
        public int Rows { get; private set; } = 4;
        public int Cols { get; private set; } = 4;

        public OptionsForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Options - Taille de la Grille";
            this.Size = new Size(300, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(20, 20, 40);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            Label lblInfo = new Label
            {
                Text = "Choisissez la difficulté :",
                ForeColor = Color.Cyan,
                Location = new Point(20, 20),
                Size = new Size(250, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            Button btn4x4 = CreateOptionButton("Facile (4x4)", 60, 4, 4);
            Button btn6x6 = CreateOptionButton("Difficile (6x6)", 120, 6, 6);

            this.Controls.Add(lblInfo);
            this.Controls.Add(btn4x4);
            this.Controls.Add(btn6x6);
        }

        private Button CreateOptionButton(string text, int y, int r, int c)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new Point(50, y),
                Size = new Size(200, 40),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(40, 40, 80),
                Cursor = Cursors.Hand
            };
            btn.Click += (s, e) => {
                this.Rows = r;
                this.Cols = c;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            return btn;
        }
    }
}
