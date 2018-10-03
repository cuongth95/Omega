using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Omega
{
    public class GUI 
    {
        private static GUI Instance;
        private int index;
        private Color backColor;
        private Control.ControlCollection controls;
        public Dictionary<int, Label> lblDict;
        public Dictionary<int, TextBox> txtDict;
        public Dictionary<int, Button> btnDict;
        public Dictionary<int, PictureBox> imgDict;
        private GUI()
        {
            lblDict = new Dictionary<int, Label>();
            txtDict = new Dictionary<int, TextBox>();
            btnDict = new Dictionary<int, Button>();
            imgDict = new Dictionary<int, PictureBox>();
        }
        public static void SetConfig(Color backColor, Control.ControlCollection controls)
        {
            if (Instance == null)
            {
                Instance = new GUI();
            }
            Instance.backColor = Color.Transparent;
            Instance.controls = controls;
        }
        public static void Begin()
        {
            Instance.index = 0;
        }
        public static void End()
        {

        }

        public static void Label(Rectangle pixelRect, string content)
        {

            if (!Instance.lblDict.ContainsKey(Instance.index))
            {
                Label label = new Label();
                label.BackColor = Instance.backColor;
                Instance.lblDict.Add(Instance.index, label);
                Instance.controls.Add(label);
            }
            var lbl = Instance.lblDict[Instance.index];
            lbl.Text = content;
            lbl.Location = pixelRect.Location;
            lbl.Size = pixelRect.Size;

            Instance.index++;
        }
        public static void Label(Rectangle pixelRect, string content,Color frontColor,Color backColor)
        {

            if (!Instance.lblDict.ContainsKey(Instance.index))
            {
                Label label = new Label();
                label.BackColor = Instance.backColor;
                Instance.lblDict.Add(Instance.index, label);
                Instance.controls.Add(label);
            }
            var lbl = Instance.lblDict[Instance.index];
            lbl.Text = content;
            lbl.Location = pixelRect.Location;
            lbl.Size = pixelRect.Size;
            lbl.ForeColor = frontColor;
            lbl.BackColor = backColor;

            Instance.index++;
        }

        public static void PictureBox(Rectangle pixelRect,string path)
        {

            if (!Instance.imgDict.ContainsKey(Instance.index))
            {
                ResourceManager rm = ResourceManager.GetInstance();
                PictureBox pic = new PictureBox();
                pic.Image = rm.GetTexture(path);
                pic.BackColor = Color.White;
                Instance.imgDict.Add(Instance.index, pic);
                Instance.controls.Add(pic);
            }
            var box = Instance.imgDict[Instance.index];
            box.Location = pixelRect.Location;
            box.Size = pixelRect.Size;
            Instance.index++;
        }

        public static void Button(Rectangle pixelRect, string content, Action onClick)
        {
            if (!Instance.lblDict.ContainsKey(Instance.index))
            {
                Button btn = new Button();
                btn.BackColor = Instance.backColor;
                Instance.btnDict[Instance.index] = btn;
                //Instance.btnDict.Add(Instance.index, btn);
                Instance.controls.Add(btn);
                btn.Click += (sender, args) =>
                {
                    onClick();
                };
            }
            var button = Instance.btnDict[Instance.index];
            button.Text = content;
            button.Location = pixelRect.Location;
            button.Size = pixelRect.Size;
            Instance.index++;
        }


        public static string TextBox(Rectangle pixelRect, string text)
        {
            string stringToEdit = "";
            TextBox txtBox = null;
            if (!Instance.txtDict.ContainsKey(Instance.index))
            {
                txtBox = new TextBox();
                //txtBox.BackColor = Instance.backColor;
                Instance.txtDict.Add(Instance.index, txtBox);
                Instance.controls.Add(txtBox);
                txtBox.Text = text;
            }
            else
                txtBox = Instance.txtDict[Instance.index];

            txtBox.Location = pixelRect.Location;
            txtBox.Size = pixelRect.Size;

            stringToEdit = txtBox.Text;
            return stringToEdit;
        }

    }
}
