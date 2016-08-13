using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Absen
{
    public partial class Form2 : Form
    {

        private int second = 0;
        public Form2(int x, int y)
        {
            InitializeComponent();
            timer1.Interval = 1000;
            pengunjung.Text = "";
            this.Location = new Point(x, y);
        }

        

        private void Form2_Load(object sender, EventArgs e)
        {
            
            
            
        }

        public void populateData(String name) 
        {

            pengunjung.Text = name;
            timer1.Start();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            second += 1;
            if (second > 5) 
            {
                pengunjung.Text = "";
                timer1.Stop();
                second = 0;
            }
        }

    }
}
