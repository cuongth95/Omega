using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega
{
    public class Stone: GameObject
    {
        private SpriteBatch sp;

        public PointF PixelPoint { get; set; }
        public Color Color { get; set; }

        private RectangleF pixelRect;

        public void MoveTo( Hex hex)
        {
            sp = SpriteBatch.GetInstance();
            hex.Holder = this;
            this.Position = hex.Position;
            this.PixelPoint = hex.PixelPoint;
            var tlPoint = new PointF(PixelPoint.X, PixelPoint.Y);
            tlPoint.X -= Constants.STONE_RADIUS / 2;
            tlPoint.Y -= Constants.STONE_RADIUS / 2;
            pixelRect = new RectangleF(tlPoint, new SizeF(Constants.STONE_RADIUS, Constants.STONE_RADIUS));
        }
        public override void Draw()
        {
            sp.FillPie(pixelRect.X, pixelRect.Y, pixelRect.Width, pixelRect.Height, Color);
        }
        public Stone(Color color)
        {
            this.Color = color;
        }

        public Stone(PointF pixelPoint, Color color)
        {
            sp = SpriteBatch.GetInstance();
            var tlPoint = new PointF(pixelPoint.X, pixelPoint.Y);
            tlPoint.X -= Constants.STONE_RADIUS / 2;
            tlPoint.Y -= Constants.STONE_RADIUS / 2;
            pixelRect = new RectangleF(tlPoint, new SizeF(Constants.STONE_RADIUS, Constants.STONE_RADIUS));
            this.Color = color;
        }
        

    }
}
