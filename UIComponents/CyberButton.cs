using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SecurIT_Memory.UIComponents
{
    /// <summary>
    /// Un bouton au style Cyberpunk avec bordures lumineuses et effets de survol.
    /// </summary>
    public class CyberButton : Button
    {
        private Color _borderColor = Color.Cyan;
        private bool _isHovered = false;

        public CyberButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.FromArgb(20, 20, 50);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 11, FontStyle.Bold);
            Size = new Size(180, 45);
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            base.OnMouseEnter(e);
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            base.OnMouseLeave(e);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Fond
            Color currentBg = _isHovered ? Color.FromArgb(40, 40, 90) : BackColor;
            using (SolidBrush brush = new SolidBrush(currentBg))
            {
                g.FillRectangle(brush, ClientRectangle);
            }

            // Bordure Néon
            using (Pen pen = new Pen(_isHovered ? Color.White : _borderColor, 2))
            {
                g.DrawRectangle(pen, 1, 1, Width - 3, Height - 3);
            }

            // Texte
            TextRenderer.DrawText(g, Text, Font, ClientRectangle, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }
}
