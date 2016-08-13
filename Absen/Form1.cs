using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using ZXing.QrCode;

namespace Absen
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection CaptureDevice;
        private VideoCaptureDevice FinalFrame;
        private Form2 form2;
        private int x, y;
        private Connection connectionDb;
        public Form1()
        {
            InitializeComponent();
            x = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Right + this.Width;
            form2 = new Form2(x,this.Location.Y);
            form2.Show();
            connectionDb = new Connection();
            timer1.Interval = 3000;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            countScanned();
            statusLabel.Text = "Idle";
            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo Device in CaptureDevice) 
            {
                comboBox1.Items.Add(Device.Name);
            }
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FinalFrame = new VideoCaptureDevice(CaptureDevice[comboBox1.SelectedIndex].MonikerString);
            FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);
            FinalFrame.Start();
        }

        private void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs) 
        {

            try
            {

                var bitmapClone = (Bitmap)eventArgs.Frame.Clone();
                pictureBox1.Image = bitmapClone;
               
            }
            catch (Exception e) 
            {
                MessageBox.Show(e.Message);
            }
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            BarcodeReader Reader = new BarcodeReader();
            Result result = Reader.Decode((Bitmap)pictureBox1.Image);
            try 
            {
                string decode = result.ToString().Trim();
                timer1.Stop();
                processScanning();
                if (decode != "") 
                {
                    
                    if (form2 == null) 
                    {
                        form2 = new Form2(x, this.Location.Y);
                        form2.Show();
                    }

                    string name = connectionDb.Scan(decode);
                    if (name != "")
                    {
                        form2.Focus();
                        form2.populateData(name);
                    }
                    else 
                    {
                        form2.Focus();
                        form2.populateData("User telah terdaftar.");
                    }
                    countScanned();
                }
                stopScanning();
            }
            catch(Exception ex)
            {
               
            }

            
        }

        public void startScanning() 
        {
            timer1.Enabled = true;
            timer1.Start();
            statusLabel.Text = "Scanning";
        }

        public void stopScanning() 
        {
            timer1.Enabled = false;
            timer1.Stop();
            statusLabel.Text = "Idle";
        }

        public void processScanning() 
        {
            timer1.Enabled = false;
            timer1.Stop();
            statusLabel.Text = "Processing";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FinalFrame.IsRunning == true) 
            {
                FinalFrame.Stop();
            }
        }

        private void check_Click(object sender, EventArgs e)
        {
            startScanning();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer1.Stop();
            statusLabel.Text = "Idle";
        }

        private void settingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Setting setting = new Setting(this);
            setting.Show();
        }

        private void lihatDaftarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listPengunjung list = new listPengunjung();
            list.Show();
        }

        private void btnStopCamera_Click(object sender, EventArgs e)
        {
            FinalFrame.Stop();
        }


        public void countScanned() 
        {
            countLabel.Text = connectionDb.getCountScanned().ToString();
        }

    }
}
