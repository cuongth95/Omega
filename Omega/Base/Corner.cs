using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega
{
    class Corner: GameObject
    {
        private SpriteBatch sp;
        private PointF[] shape;
        private Direction direction;
        public Direction Direction {
            get { return direction; }
            set
            {
                direction = value;
                CalculateShape();
            }

        }
        private Color color;
        public Color Color {
            get {
                return Color;
            }
            set
            {
                color = value;
               
            }
        }
        
        private Hex HolderHex;
        public Corner( Direction dir)
        {
            shape = new PointF[4];
            direction = dir;
        }
        public override void Init()
        {
            base.Init();
            Color = Color.Black;
            sp = SpriteBatch.GetInstance();
        }
        public void SetHolderHex(Hex holder)
        {
            HolderHex = holder;
            Position = new Vector2(HolderHex.Position);

            CalculateShape();
        }

        public override void Draw()
        {
            DrawCorner();
        }

        private void DrawCorner()
        {
            ////Create 6 points
            //for (int a = 0; a < 3; a++)
            //{
            //    shape[a] = new PointF(
            //        holderHex.PixelPoint.X + holderHex.Radius * (float)Math.Sin(a * 60 * Math.PI / 180f),
            //        holderHex.PixelPoint.Y + holderHex.Radius * (float)Math.Cos(a * 60 * Math.PI / 180f));
            //}

            //Color = Color.Black;



            sp.DrawLine(shape[0],shape[1],color, 6, LineCap.Round, LineCap.Round);
            sp.DrawLine( shape[1], shape[2], color, 6, LineCap.Round, LineCap.Round);
            sp.DrawLine( shape[2], shape[3], color, 6, LineCap.Round, LineCap.Round);

            

        }

        private void CalculateShape()
        {
            int times0 = 0;
            int times1 = 0;
            int times2 = 0;
            int times3 = 0;
            switch (direction)
            {
                case Direction.Right:
                    times0 = 0;
                    times1 = 1;
                    times2 = 2;
                    times3 = 3;
                    break;
                case Direction.TopRight:

                    times0 = 1;
                    times1 = 2;
                    times2 = 3;
                    times3 = 4;
                    break;
                case Direction.TopLeft:

                    times0 = 2;
                    times1 = 3;
                    times2 = 4;
                    times3 = 5;

                    break;
                case Direction.Left:

                    times0 = 3;
                    times1 = 4;
                    times2 = 5;
                    times3 = 0;
                    break;
                case Direction.BotLeft:

                    times0 = 4;
                    times1 = 5;
                    times2 = 0;
                    times3 = 1;
                    break;
                case Direction.BotRight:

                    times0 = 5;
                    times1 = 0;
                    times2 = 1;
                    times3 = 2;

                    break;
                default:
                    break;
            }

            shape[0] = new PointF(
                    HolderHex.PixelPoint.X + HolderHex.Radius * (float)Math.Sin(times0 * 60 * Math.PI / 180f),
                    HolderHex.PixelPoint.Y + HolderHex.Radius * (float)Math.Cos(times0 * 60 * Math.PI / 180f));

            shape[1] = new PointF(
            HolderHex.PixelPoint.X + HolderHex.Radius * (float)Math.Sin(times1 * 60 * Math.PI / 180f),
            HolderHex.PixelPoint.Y + HolderHex.Radius * (float)Math.Cos(times1 * 60 * Math.PI / 180f));

            shape[2] = new PointF(
            HolderHex.PixelPoint.X + HolderHex.Radius * (float)Math.Sin(times2 * 60 * Math.PI / 180f),
            HolderHex.PixelPoint.Y + HolderHex.Radius * (float)Math.Cos(times2 * 60 * Math.PI / 180f));

            shape[3] = new PointF(
            HolderHex.PixelPoint.X + HolderHex.Radius * (float)Math.Sin(times3 * 60 * Math.PI / 180f),
            HolderHex.PixelPoint.Y + HolderHex.Radius * (float)Math.Cos(times3 * 60 * Math.PI / 180f));
        }

        public Vector2 GetPositionOfHolder()
        {
            return this.HolderHex.Position;
        }
    }
}
