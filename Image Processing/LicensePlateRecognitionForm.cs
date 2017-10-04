﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System.Diagnostics;
using Emgu.CV.Util;
using LicensePlateRecognition;

namespace Image_Processing
{
    public partial class LicensePlateRecognitionForm : Form
    {

        private LicensePlateDetector _licensePlateDetector;
        public LicensePlateRecognitionForm()
        {
            InitializeComponent();
            _licensePlateDetector = new LicensePlateDetector("");
            Mat m = new Mat("E:\\C#\\高级程序设计\\图像处理\\王先文高级程序设计\\Image+Processing\\Image Processing\\Image Processing\\license-plate.jpg");
            UMat um = m.GetUMat(AccessType.ReadWrite);
            imageBox1.Image = um;
            ProcessImage(m);
        }
        private void ProcessImage(IInputOutputArray image)
        {
            Stopwatch watch = Stopwatch.StartNew(); // time the detection process

            List<IInputOutputArray> licensePlateImagesList = new List<IInputOutputArray>();
            List<IInputOutputArray> filteredLicensePlateImagesList = new List<IInputOutputArray>();
            List<RotatedRect> licenseBoxList = new List<RotatedRect>();
            List<string> words = _licensePlateDetector.DetectLicensePlate(
               image,
               licensePlateImagesList,
               filteredLicensePlateImagesList,
               licenseBoxList);

            watch.Stop(); //stop the timer
            processTimeLabel.Text = String.Format("License Plate Recognition time: {0} milli-seconds", watch.Elapsed.TotalMilliseconds);

            panel1.Controls.Clear();
            Point startPoint = new Point(10, 10);
            for (int i = 0; i < words.Count; i++)
            {
                Mat dest = new Mat();
                CvInvoke.VConcat(licensePlateImagesList[i], filteredLicensePlateImagesList[i], dest);
                AddLabelAndImage(
                   ref startPoint,
                   String.Format("License: {0}", words[i]),
                   dest);
                PointF[] verticesF = licenseBoxList[i].GetVertices();
                Point[] vertices = Array.ConvertAll(verticesF, Point.Round);
                using (VectorOfPoint pts = new VectorOfPoint(vertices))
                    CvInvoke.Polylines(image, pts, true, new Bgr(Color.Red).MCvScalar, 2);

            }

        }
        private void AddLabelAndImage(ref Point startPoint, String labelText, IImage image)
        {
            Label label = new Label();
            panel1.Controls.Add(label);
            label.Text = labelText;
            label.Width = 100;
            label.Height = 30;
            label.Location = startPoint;
            startPoint.Y += label.Height;

            ImageBox box = new ImageBox();
            panel1.Controls.Add(box);
            box.ClientSize = image.Size;
            box.Image = image;
            box.Location = startPoint;
            startPoint.Y += box.Height + 10;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                Mat img;
                try
                {
                    img = CvInvoke.Imread(openFileDialog1.FileName);

                }
                catch
                {
                    MessageBox.Show(String.Format("Invalide File: {0}", openFileDialog1.FileName));
                    return;
                }

                imageBox1.Image = img;   //显示载入的图片


                UMat uImg = img.GetUMat(AccessType.ReadWrite);
                ProcessImage(uImg);
            }
        }
    }
}
