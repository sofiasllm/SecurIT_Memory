using System;
using System.Drawing;
using System.Windows.Forms;
using SecurIT_Memory.UIComponents;

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
            this.Text = "Options - Difficulté";
            this.Size = new Size(300, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(10, 10, 25);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            Label lblInfo = new Label
            {
                Text = "SÉLECTION DU NIVEAU",
                ForeColor = Color.Cyan,
                Location = new Point(0, 30),
                Size = new Size(300, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };

            CyberButton btn4x4 = new CyberButton { Text = "FACILE (4x4)", Location = new Point(50, 80), Size = new Size(200, 50) };
            btn4x4.Click += (s, e) => SelectDifficulty(4, 4);

            CyberButton btn6x6 = new CyberButton { Text = "EXPERT (6x6)", Location = new Point(50, 150), Size = new Size(200, 50) };
            btn6x6.Click += (s, e) => SelectDifficulty(6, 6);

            this.Controls.Add(lblInfo);
            this.Controls.Add(btn4x4);
            this.Controls.Add(btn6x6);
        }

        private void SelectDifficulty(int r, int c)
        {
            this.Rows = r;
            this.Cols = c;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
