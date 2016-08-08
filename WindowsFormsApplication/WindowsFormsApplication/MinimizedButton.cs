using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication
{
    public partial class MinimizedButton : Form
    {
        public MinimizedButton()
        {
            InitializeComponent();

            this.TopMost = true;
            pictureBox1.Click += new EventHandler(myButton_Click);
        }
        void myButton_Click(Object sender, System.EventArgs e)
        {
            Form1 f1 = new Form1();
            f1.Show();

            this.Visible = false;
        }

        private void MinimizedButton_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process[] ProcessList = Process.GetProcessesByName("WindowsFormsApplication");
            if (ProcessList.Length > 0)
                ProcessList[0].Kill();
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(pictureBox1, "이름 : 주황버섯\n레벨 : 8");
        }
    }
    public class myButtonObject : UserControl
    {
        // Draw the new button.
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Pen myPen = new Pen(Color.Black);
            // Draw the button in the form of a circle
            graphics.DrawEllipse(myPen, 0, 0, 100, 100);
            myPen.Dispose();
        }
    }
}
