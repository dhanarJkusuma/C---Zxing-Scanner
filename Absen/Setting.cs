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
    public partial class Setting : Form
    {
        private Form1 home;
        public Setting(Form1 home)
        {
            InitializeComponent();
            this.home = home;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {

            DialogResult dialogResult = MessageBox.Show("Reset data?", "Dialog Confirmation", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Connection connection = new Connection();
                connection.resetAll();
                home.countScanned();
            }
            
            
        }
    }
}
