using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega
{
    public class Hex: GameObject
    {
        private SpriteBatch sp;
        private PointF[] shape;
        public Stone Holder { get; set; }
        public PointF PixelPoint { get; set; }
        public float Radius
        {
            get;set;
        }
        public Color Color
        {
            get;set;
        }

        public bool IsHold
        {
            get
            {
                return Holder != null;
            }
        }

        public override void Reset(bool isActive = false)
        {
            Id = 0;
            Holder = null;
            
        }

        public Hex(int x,int y)
        {
            Position = new Vector2(x,y);
            shape = new PointF[6];
            Holder = null;
        }
        public Hex(Vector2 pos)
        {
            Position = new Vector2( pos);
            shape = new PointF[6];
            Holder = null;
        }

        public void Inject(float rad, PointF originPixelPoint)
        {
            this.Radius = rad;
            this.PixelPoint = Utils.PositionToPixel(Position, originPixelPoint, rad);


            //Create 6 points
            for (int a = 0; a < 6; a++)
            {
                shape[a] = new PointF(
                    PixelPoint.X + Radius * (float)Math.Sin(a * 60 * Math.PI / 180f),
                    PixelPoint.Y + Radius * (float)Math.Cos(a * 60 * Math.PI / 180f));
            }
        }

        public override void Init()
        {
            base.Init();
            sp = SpriteBatch.GetInstance();
            Color = Color.ForestGreen;
            Holder = null;
        }
        public override void Draw()
        {
            DrawShape();
        }
        
        private void DrawShape()
        {
            sp.FillPolygon( shape,Color);
            if(Constants.DEBUG_DRAW_POSITION_IN_HEX)
                sp.DrawString(Position.X + ","+Position.Y, "arial",this.PixelPoint,Color.White);
            
        }

        public void TestDrawLabel()
        {
            if (Constants.DEBUG_DRAW_LABEL)
            {
                sp.DrawString(this.Id + "", "arial", this.PixelPoint, Color.DeepPink);
            }
        }

        public bool EqualTo(Hex b)
        {
            if (this.Position.X == b.Position.X
                &&
                this.Position.Y == b.Position.Y)
                return true;
            return false;
        }

        public Vector2 GetPosistionOfNeighbor(Direction dir)
        {
            return Neighbor(dir).Position;
        }

        public Hex Neighbor(Direction dir)
        {
            switch (dir)
            {
                case Direction.Right:
                    return this+ new Hex(1,0);
                case Direction.TopRight:
                    return this + new Hex(1, -1);
                case Direction.TopLeft:
                    return this + new Hex(0, -1);
                case Direction.Left:
                    return this + new Hex(-1, 0);
                case Direction.BotLeft:
                    return this + new Hex(-1, 1);
                case Direction.BotRight:
                    return this + new Hex(0, 1);
                default:
                    return null;
            }
        }
        
        public static float Distance(Hex a,Hex b)
        {
            return (Math.Abs(a.Position.X - b.Position.X)
                + Math.Abs(a.Position.X + a.Position.Y - b.Position.X - b.Position.Y)
                + Math.Abs(a.Position.Y - b.Position.Y)) / 2;
        }


        public static Hex operator +(Hex a, Hex b)
        {
            return new Hex(a.Position.X + b.Position.X, a.Position.Y + b.Position.Y);
        }
        public static Hex operator -(Hex a,Hex b)
        {
            return new Hex(a.Position.X - b.Position.X, a.Position.Y - b.Position.Y);
        }
        public static Hex operator *(Hex a,Hex b)
        {
            return new Hex(a.Position.X * b.Position.X, a.Position.Y * b.Position.Y);

        }

    }
}
