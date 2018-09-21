using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Omega
{
    public enum GameState {
        START,
        PAUSE,
        RUNNING,
        GAME_OVER

    }
    public enum EventType
    {
        MOVE_STONE

    }
    public class EventHandler {
        public EventType type;
        public EventArgs eArgs;

        public EventHandler(EventType type,EventArgs eventArgs)
        {
            this.type = type;
            this.eArgs = eventArgs;
        }
    }
    public class BundleArgs: EventArgs
    {
        Dictionary<string, object> bundle;

        public BundleArgs()
        {
            bundle = new Dictionary<string, object>();
        }
        public void AddItem(string key, object obj)
        {
            bundle.Add(key, obj);
        }
        public T GetItemByKey<T>(string key)
        {
            return (T)bundle[key];
        }
    }

    public struct Vector2
    {
        
        public int X;
        public int Y;
        
        public Vector2(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public Vector2(Vector2 vec)
        {
            this.X = vec.X;
            this.Y = vec.Y;
        }

        public override string ToString()
        {
            return "("+X+","+Y+")";
        }

        public float GetLength()
        {
            return (float)Math.Sqrt(Math.Abs( X * X )+ Math.Abs(Y * Y));
        }
        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            Vector2 ret = new Vector2(a.X + b.X, a.Y + b.Y);

            return ret;
        }
        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            Vector2 ret = new Vector2(a.X-b.X,a.Y-b.Y);

            return ret;
        }
        public static bool operator == (Vector2 a, Vector2 b)
        {
            if (a.X == b.X
                && a.Y == b.Y)
            {
                return true;
            }
            return false;
        }
        public static bool operator !=(Vector2 a, Vector2 b)
        {
            if (a.X != b.X
                || a.Y != b.Y)
                return true;
            return false;
        }
    }
    public enum Direction
    {
        Right=0,
        TopRight=1,
        TopLeft=2,
        Left=3,
        BotLeft=4,
        BotRight=5
    }
    public enum ResourceType {
        Texture,
        Font
    }

    public struct ColorRegion
    {
        public int regionId;
        public Color color;
        public ColorRegion(int regionId,Color color)
        {
            this.regionId = regionId;
            this.color = color;
        }
    }

    public class Constants
    {

        public const bool DEBUG_DRAW_POSITION_IN_HEX = false;
        public const bool DEBUG_DRAW_LABEL = true;

        public readonly static Color BACKGROUND_COLOR = Color.White;
        public const int PLAYSCOPE_HEXS_PER_SIDE = 2;
        public const int MIN_HEXS_PER_SIDE = 10;
        public const int MAX_HEXS_PER_SIDE = 15;
        public const float STONE_RADIUS = 32;
        public const int WINDOW_WIDTH = 1280;
        public const int WINDOW_HEIGHT = 800;
        public const float SPACING = 1.1f;
    }

    public class Utils
    {
        public static PointF PositionToPixel(Vector2 pos,PointF origin, float radius)
        {
            
            var x = Constants.SPACING * radius * (float)(Math.Sqrt(3) * pos.X + Math.Sqrt(3) / 2 * pos.Y);
            var y = Constants.SPACING * radius * (3f / 2 * pos.Y);
            var point = new PointF(origin.X + x, origin.Y + y);
            return point;
        }

        public static Vector2 PixelToPosition(Point mousePoint,PointF origin,float radius)
        {
            Vector2 pos;

            var tempPoint = new PointF((mousePoint.X - origin.X), (mousePoint.Y - origin.Y) );
            var posX = (Math.Sqrt(3) / 3 * tempPoint.X - 1f / 3 * tempPoint.Y) / (radius * Constants.SPACING);
            var posY = (2.0 / 3 * tempPoint.Y) / (radius * Constants.SPACING);

            Console.WriteLine("rawTF = " + posX + "," + posY);

            //var rx = Math.Round(posX, 1, MidpointRounding.AwayFromZero);
            //var ry = Math.Round(posY, 1, MidpointRounding.AwayFromZero);

            var tempX = (int)Math.Round(posX, 1, MidpointRounding.AwayFromZero);// (int)(posX / 1);
            var tempY = (int)Math.Round(posY, 1, MidpointRounding.AwayFromZero); //(int)(posY / 1);

            if (Math.Abs(posX - tempX) >= 0.5f)
            {
                if (posX >= 0)
                {
                    tempX++;
                }
                else
                {
                    tempX--;
                }
            }

            if (Math.Abs(posY - tempY) >= 0.5f)
            {
                if (posY >= 0)
                {
                    tempY++;
                }
                else
                {
                        tempY--;
                }
            }

            pos = new Vector2(tempX, tempY);
            //pos = new Vector2((int)rx, (int)ry);
            return pos;

            //var point = new PointF((mousePoint.X - origin.X) / Constants.SPACING, (mousePoint.Y - origin.Y) / Constants.SPACING);
            //var x = (Math.Sqrt(3) / 3 * point.X - 1.0 / 3 * point.Y) / radius;
            //var y = (2.0 / 3 * point.Y) / radius;


            //var rx = Math.Round(x, 1, MidpointRounding.AwayFromZero);
            //var ry = Math.Round(y, 1, MidpointRounding.AwayFromZero);
            //Console.WriteLine(" x =" + x + ",y=" + y);
            //Console.WriteLine("rx=" + rx + ",ry=" + ry);

            //var tempX = (int)(x / 1);
            //var tempY = (int)(y / 1);
            //if (x - tempX >= 0.5)
            //{
            //    if (tempX > 0)
            //        tempX++;
            //    else if (tempX < 0)
            //        tempX--;
            //}
            //if (y - tempY >= 0.5)
            //{
            //    if (tempY > 0)
            //        tempY++;
            //    else if (tempY < 0)
            //        tempY--;
            //}

            ////Console.WriteLine(mousePoint.ToString());
            //return new Vector2(tempX, tempY);

            //return new Vector2((int)x, (int)y);
        }
        
    }
}
