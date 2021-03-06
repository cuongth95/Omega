﻿using Omega.Test;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Omega
{
    public enum TFlag
    {
        UPPER_BOUND=0,
        LOWER_BOUND=1,
        EXACT_VALUE=2
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
        public EventHandler Clone()
        {
            return new EventHandler(this.type, this.eArgs);
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
    [System.Serializable]
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
        Left = 0,
        TopLeft = 1,
        TopRight = 2,
        Right =3,
        BotRight = 4,
        BotLeft =5
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
    public struct Region
    {
        public int regionId;
        public int holderId;
        public Region(int regionId, int holderId)
        {
            this.regionId = regionId;
            this.holderId = holderId;
        }
    }


    public class Utils
    {
        public static Array Directions = Enum.GetValues(typeof(Direction));
        private static Random rng = new Random();
        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static List<Vector2> Clone(List<Vector2> list)
        {
            List<Vector2> ret = new List<Vector2>();

            foreach (var item in list)
            {
                ret.Add(new Vector2(item));
            }

            return ret;
        }
        public static List<Player> Clone(List<Player> list)
        {
            List<Player> ret = new List<Player>();

            foreach (var item in list)
            {
                ret.Add(new Player(item));
            }

            return ret;
        }
        public static List<Command> Clone(List<Command> cmdList)
        {
            List<Command> ret = new List<Command>();

            foreach (var cmd in cmdList)
            {
                ret.Add(cmd.Clone());
            }

            return ret;
        }

        public static Dictionary<Vector2, Unit> Clone(Dictionary<Vector2, Unit> board)
        {
            Dictionary<Vector2, Unit> temp = new Dictionary<Vector2, Unit>();

            foreach (var pair in board)
            {
                temp[pair.Key] = pair.Value.Clone();
            }

            return temp;
        }
        public static int Dot(int[] x, int[] y)
        {
            if (x.Length != y.Length) throw new NullReferenceException("Illegal vector dimensions.");
            int sum = 0;
            for (int i = 0; i < x.Length; i++)
                sum += x[i] * y[i];
            return sum;
        }
        public static float Dot(float[] x, float[] y)
        {
            if (x.Length != y.Length) throw new NullReferenceException("Illegal vector dimensions.");
            float sum = 0f;
            for (int i = 0; i < x.Length; i++)
                sum += x[i] * y[i];
            return sum;
        }
        public static PointF PositionToPixel(Vector2 pos, PointF origin, float radius)
        {

            var x = Constants.SPACING * radius * (float)(Math.Sqrt(3) * pos.X + Math.Sqrt(3) / 2 * pos.Y);
            var y = Constants.SPACING * radius * (3f / 2 * pos.Y);
            var point = new PointF(origin.X + x, origin.Y + y);
            return point;
        }

        public static Vector2 PixelToPosition(Point mousePoint, PointF origin, float radius)
        {
            Vector2 pos;

            var tempPoint = new PointF((mousePoint.X - origin.X), (mousePoint.Y - origin.Y));
            var posX = (Math.Sqrt(3) / 3 * tempPoint.X - 1f / 3 * tempPoint.Y) / (radius * Constants.SPACING);
            var posY = (2.0 / 3 * tempPoint.Y) / (radius * Constants.SPACING);

            //Console.WriteLine("rawTF = " + posX + "," + posY);

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

        }
        public static string ConvertBinaryToString(int n)
        {
            char[] b = new char[32];
            int pos = 31;
            int i = 0;

            while (i < 32)
            {
                if ((n & (1 << i)) != 0)
                {
                    b[pos] = '1';
                }
                else
                {
                    b[pos] = '0';
                }
                pos--;
                i++;
            }
            return new string(b);
        }
        public static ulong GetRandomBitString()
        {
            byte[] buf = new byte[8];
            rng.NextBytes(buf);
            ulong longRand = BitConverter.ToUInt64(buf, 0);


            return longRand % (ulong.MaxValue);
        }
    }
}
