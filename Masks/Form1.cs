using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace Masks
{
    public partial class Form1 : Form
    {
        static readonly CascadeClassifier cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");
        Image[] masks = new Image[2];
        Image mask1 = Image.FromFile("mask1.png");
        Image mask2 = Image.FromFile("mask2.png");
        Image selectedMask;
        public Form1()
        {
            InitializeComponent();

            cboMasks.Items.Add("Maska 1");
            cboMasks.Items.Add("Maska 2");

            selectedMask = mask1;
            masks[0] = mask1;
            masks[1] = mask2;
        }

        FilterInfoCollection filter;
        VideoCaptureDevice device;

        private void Form1_Load(object sender, EventArgs e)
        {
            filter = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in filter)
            {
                cboDevice.Items.Add(device.Name);
            }
            cboDevice.SelectedIndex = 0;
            device = new VideoCaptureDevice();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btnDetect_Click(object sender, EventArgs e)
        {
            device = new VideoCaptureDevice(filter[cboDevice.SelectedIndex].MonikerString);
            device.NewFrame += Device_NewFrame;
            device.Start();
        }

        private void Device_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap video = (Bitmap)eventArgs.Frame.Clone();
            Bitmap bitmap = new Bitmap(video, 800, 500);
            Image<Bgr, byte> greyImage = new Image<Bgr, byte>(bitmap);
            Rectangle[] rectangles = cascadeClassifier.DetectMultiScale(greyImage, 1.2, 0);
            //foreach (Rectangle rectangle in rectangles)
            if (rectangles[0] != null)
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.DrawImage(selectedMask, rectangles[0].X, rectangles[0].Y, rectangles[0].Width, rectangles[0].Height);
                }
            }
            pic.Image = bitmap;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (device.IsRunning)
                device.Stop();
        }

        private void cboMasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedMask = masks[cboMasks.SelectedIndex];
        }
    }
}