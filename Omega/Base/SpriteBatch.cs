using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega
{
    class SpriteBatch
    {
        private static SpriteBatch Instance;

        private Graphics g;
        private Graphics gFrontBuffer;
        private Bitmap backBuffer;
        private SolidBrush brush;
        private Pen pen;
        private Font stringFont;
        private ResourceManager rm;

        private SpriteBatch()
        {
        }

        public static SpriteBatch GetInstance()
        {
            if (Instance == null)
            {
                Instance = new SpriteBatch();
            }
            return Instance;
        }

        public void SetConfig(Graphics graphics,IntPtr formHandle,Bitmap buffer)
        {
            g = graphics;
            gFrontBuffer = Graphics.FromHwnd(formHandle);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            gFrontBuffer.SmoothingMode = SmoothingMode.AntiAlias;
            backBuffer = buffer;

            rm = ResourceManager.GetInstance();

            rm.AddFont("arial5", new Font("arial", 10));
            rm.AddBrush("solid", new SolidBrush(Color.Black));
            rm.AddPen("master", new Pen(Color.Black));

            stringFont = rm.GetFont("arial5");
            brush =  rm.GetBrush("solid") as SolidBrush;
            pen = rm.GetPen("master");

        }


        public void FillPolygon(PointF[] shape, Color color)
        {
            brush.Color = color;
            g.FillPolygon(brush, shape);
        }
        public void FillPie(float tlPointX,float tlPointY,float width,float height, Color c,float startAngle=0,float sweepAngle=360)
        {
            brush.Color = c;
            g.FillPie(brush, tlPointX, tlPointY, width, height, startAngle, sweepAngle);
        }
        public void DrawString(string text,string fontName,PointF point,Color color)
        {
            brush.Color = color;
            g.DrawString(text, stringFont, brush, point);
        }
        public void DrawLine(PointF p1,PointF p2,Color color)
        {
            pen.Color = color;
            g.DrawLine(pen,p1,p2);
        }

        public void DrawLine(PointF p1, PointF p2, Color color, float width, LineCap startCap = LineCap.Flat, LineCap endCap = LineCap.Flat)
        {
            var tempWidth = pen.Width;
            var tempStartCap = pen.StartCap;
            var tempEndCap = pen.EndCap;


            SetPenConfig(width, startCap, endCap);

            DrawLine(p1,p2, color);

            SetPenConfig(tempWidth,tempStartCap,tempEndCap);

        }
        public void FillRectangle(Rectangle rect, Color color)
        {
            brush.Color = color;
            g.FillRectangle(brush, rect);
        }
        private void SetPenConfig(float width,LineCap startCap,LineCap endCap )
        {
            pen.Width = width;
            pen.StartCap = startCap;
            pen.EndCap = endCap;
        }


        public void Begin()
        {
            g.Clear(Color.White);
        }
        public void End()
        {
            gFrontBuffer.DrawImage(backBuffer, new Point(0, 0));

           
        }
    }
}
