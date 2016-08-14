using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication
{
    public partial class MinimizedButton : Form
    {
        private Point mCurrentPosition = new Point(0, 0);

        public MinimizedButton()
        {
            InitializeComponent();

            this.TopMost = true;
            this.button1.Click += new EventHandler(button1_Click);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 f1 = new Form1();
            f1.Show();

            this.Visible = false;
        }

        #region drag
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
                mCurrentPosition = new Point(-e.X, -e.Y);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(
                    this.Location.X + (mCurrentPosition.X + e.X),
                    this.Location.Y + (mCurrentPosition.Y + e.Y));// 마우스의 이동치를 Form Location에 반영한다.
            }
        }
        #endregion

        private void MinimizedButton_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process[] ProcessList = Process.GetProcessesByName("WindowsFormsApplication");
            if (ProcessList.Length > 0)
                ProcessList[0].Kill();
        }

        private void MinimizedButton_MouseUp(object sender, MouseEventArgs e)
        {
            Form1 f1 = new Form1();
            f1.Show();

            this.Visible = false;
        }

    }

    public class ButtonModified : System.Windows.Forms.Button
    {
        //we can use this to modify the color of the border 
        public Color BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(222)))), ((int)(((byte)(255)))));
        //we can use this to modify the border size
        public int BorderSize = 5;
        public ButtonModified()
        {
            FlatStyle = FlatStyle.Flat;
            BackColor = Color.White;
            FlatAppearance.BorderColor = BorderColor;
            FlatAppearance.BorderSize = BorderSize;
            Font = new Font("VAGRounded-Light", 30F, FontStyle.Regular, GraphicsUnit.Pixel, ((byte)(0)));
            ForeColor = Color.FromArgb(192, 222, 255);

            this.MouseDown += new MouseEventHandler(ButtonLastest_MouseDown);
            this.MouseUp += new MouseEventHandler(ButtonLastest_MouseUp);
            this.MouseEnter += new EventHandler(ButtonLastest_MouseEnter);
        }

        void ButtonLastest_MouseUp(object sender, MouseEventArgs e)
        {
            ForeColor = Color.FromArgb(192, 255, 255);
            BackColor = Color.White;
        }

        void ButtonLastest_MouseDown(object sender, MouseEventArgs e)
        {
            BackColor = Color.FromArgb(192, 222, 255);
            ForeColor = Color.White;
        }

        void ButtonLastest_MouseEnter(object sender, EventArgs e)
        {
            BackColor = Color.White;
            ForeColor = Color.FromArgb(192, 222, 255);
        }

        int top;
        int left;
        int right;
        int bottom;

        protected override void OnPaint(PaintEventArgs pevent)
        {
            // to draw the control using base OnPaint
            base.OnPaint(pevent);
            //to modify the corner radius
            int CornerRadius = 18;

            Pen DrawPen = new Pen(BorderColor);
            GraphicsPath gfxPath_mod = new GraphicsPath();

            top = 0;
            left = 0;
            right = Width;
            bottom = Height;

            gfxPath_mod.AddArc(left, top, CornerRadius, CornerRadius, 180, 90);
            gfxPath_mod.AddArc(right - CornerRadius, top, CornerRadius, CornerRadius, 270, 90);
            gfxPath_mod.AddArc(right - CornerRadius, bottom - CornerRadius, CornerRadius, CornerRadius, 0, 90);
            gfxPath_mod.AddArc(left, bottom - CornerRadius, CornerRadius, CornerRadius, 90, 90);

            gfxPath_mod.CloseAllFigures();

            pevent.Graphics.DrawPath(DrawPen, gfxPath_mod);

            int inside = 1;

            Pen newPen = new Pen(BorderColor, BorderSize);
            GraphicsPath gfxPath = new GraphicsPath();
            gfxPath.AddArc(left + inside + 1, top + inside, CornerRadius, CornerRadius, 180, 100);

            gfxPath.AddArc(right - CornerRadius - inside - 2, top + inside, CornerRadius, CornerRadius, 270, 90);
            gfxPath.AddArc(right - CornerRadius - inside - 2, bottom - CornerRadius - inside - 1, CornerRadius, CornerRadius, 0, 90);

            gfxPath.AddArc(left + inside + 1, bottom - CornerRadius - inside, CornerRadius, CornerRadius, 95, 95);
            pevent.Graphics.DrawPath(newPen, gfxPath);

            this.Region = new System.Drawing.Region(gfxPath_mod);
        }
    }
}
