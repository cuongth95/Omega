using Omega.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.Test
{
    public class Unit
    {
        public const int HOLDER_EMPTY = 0;
        public const int HOLDER_PLAYER_0 = 1;
        public const int HOLDER_PLAYER_1 = 2;

        public int Id { get; set; }
        public Vector2 Position { get; set; }
        public int Holder { get; set; }
        public bool IsHold
        {
            get
            {
                return Holder > 0;
            }   
        }
        public int CountId { get; set; }

        
        public Unit(int x, int y)
        {
            this.Position = new Vector2(x,y);
            this.Holder = HOLDER_EMPTY;
            CountId = -1;
        }
        public Unit(Vector2 Position)
        {
            this.Position = Position;
            this.Holder = HOLDER_EMPTY;
            CountId = -1;
        }
        
        public Unit Clone()
        {
            Unit ret= new Unit(Position.X,Position.Y);
            ret.Holder = this.Holder;
            ret.CountId = this.CountId;
            return ret;
        }

        public bool EqualTo(Unit b)
        {
            if (this.Position.X == b.Position.X
                &&
                this.Position.Y == b.Position.Y)
                return true;
            return false;
        }

        public int GetNumberOfFreeSlots(int checkId)
        {
            int freeSlots = 6;
            var dirs = Utils.Directions;
            for (int i = 0; i < dirs.Length; i++)
            {
                if (this.Neighbor((Direction)dirs.GetValue(i)).Holder == checkId)
                    freeSlots--;
            }

            return freeSlots;
        }

        public Vector2 GetPosistionOfNeighbor(Direction dir)
        {
            return Neighbor(dir).Position;
        }

        public Unit Neighbor(Direction dir)
        {
            switch (dir)
            {
                case Direction.Right:
                    return this + new Unit(1, 0);
                case Direction.TopRight:
                    return this + new Unit(1, -1);
                case Direction.TopLeft:
                    return this + new Unit(0, -1);
                case Direction.Left:
                    return this + new Unit(-1, 0);
                case Direction.BotLeft:
                    return this + new Unit(-1, 1);
                case Direction.BotRight:
                    return this + new Unit(0, 1);
                default:
                    return null;
            }
        }

        public static float Distance(Unit a, Unit b)
        {
            return (Math.Abs(a.Position.X - b.Position.X)
                + Math.Abs(a.Position.X + a.Position.Y - b.Position.X - b.Position.Y)
                + Math.Abs(a.Position.Y - b.Position.Y)) / 2;
        }


        public static Unit operator +(Unit a, Unit b)
        {
            return new Unit(a.Position.X + b.Position.X, a.Position.Y + b.Position.Y);
        }
        public static Unit operator -(Unit a, Unit b)
        {
            return new Unit(a.Position.X - b.Position.X, a.Position.Y - b.Position.Y);
        }
        public static Unit operator *(Unit a, Unit b)
        {
            return new Unit(a.Position.X * b.Position.X, a.Position.Y * b.Position.Y);

        }

       
    }
}
