using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.Test
{
    public class GridMan: GameObject
    {
        private GameState gs;
        private Dictionary<Vector2, Hex> hexGrid;
        private Dictionary<Vector2, Stone> stoneList;
        private Dictionary<Direction, Corner> cornerDict;

        public PointF Origin { get; set; }

        public int PlayRad { get; private set; }
        public int MapRad { get; set; }
        public float HexRad { get; set; }

        private float angle;
        public float Angle
        {
            get { return this.angle; }
            set
            {
                if (this.angle > 0)
                    this.angle = value;
            }
        }
        public int TotalPlaygroundHexes { get; private set; }
       
        public void SetGameState(GameState gs)
        {
            this.gs = gs;
            stoneList.Clear();
            foreach (var pair in gs.Board)
            {
                if (pair.Value.IsHold)
                {
                    if (!stoneList.ContainsKey(pair.Key))
                    {
                        var pixelPoint = Utils.PositionToPixel(pair.Value.Position, Constants.GRID_ORIGIN, Constants.GRID_HEX_RADIUS);
                        stoneList.Add(pair.Key, new Stone(pixelPoint, GetColorByPlayerId(pair.Value.Holder)));
                    }

                }
                else
                {
                    if (stoneList.ContainsKey(pair.Key))
                    {
                        stoneList.Remove(pair.Key);
                    }
                }
            }
        }

        public GridMan(GameState gs,PointF origin, float hexRadius)
        {
            this.gs = gs;

            this.MapRad = gs.MapRad;
            this.PlayRad = gs.PlayRad;

            this.Origin = origin;
            this.HexRad = hexRadius;

        }

        public override void Init()
        {
            //grid = new List<Hex>();
            this.angle = 60f;
            HexRad = 20;
            hexGrid = new Dictionary<Vector2, Hex>();
            stoneList = new Dictionary<Vector2, Stone>();

            //draw Hexagon board
            for (int x = -MapRad; x <= MapRad; x++)
            {
                int z1 = Math.Max(-MapRad, -x - MapRad);
                int z2 = Math.Min(MapRad, -x + MapRad);
                for (int z = z1; z <= z2; z++)
                {
                    var position = new Vector2(x, z);
                    var hex = new Hex(position);
                    hex.Init();
                    hex.Inject(HexRad, Origin);
                    hexGrid.Add(position, hex);
                }
            }
            SetupConerns();
        }
        public void SetupConerns()
        {
            this.MapRad = gs.MapRad;
            this.PlayRad = gs.PlayRad;
            cornerDict = new Dictionary<Direction, Corner>();
            TotalPlaygroundHexes = 0;
            for (int x = -MapRad; x <= MapRad; x++)
            {
                int z1 = Math.Max(-MapRad, -x - MapRad);
                int z2 = Math.Min(MapRad, -x + MapRad);


                int scopeZ1 = Math.Max(-PlayRad, -x - PlayRad);
                int scopeZ2 = Math.Min(PlayRad, -x + PlayRad);

                for (int z = z1; z <= z2; z++)
                {
                    var pos = new Vector2(x, z);
                    var hex = hexGrid[pos];
                    if (x >= -PlayRad && x <= PlayRad &&
                        z >= scopeZ1 && z <= scopeZ2)
                    {
                        TotalPlaygroundHexes++;
                        //play scope
                        hex.Color = Color.LightGreen;

                    }
                    else
                    {
                        //other scope
                        hex.Color = Color.ForestGreen;
                    }
                }
            }


            var cornScope = PlayRad;

            Vector2 cornerPos = new Vector2();
            var cornerL = new Corner(Direction.Left);
            cornerPos = new Vector2(-cornScope, 0);
            cornerL.SetHolderHex(hexGrid[cornerPos]);
            cornerDict.Add(cornerL.Direction, cornerL);

            var cornerTL = new Corner(Direction.TopLeft);
            cornerPos = new Vector2(0, -cornScope);
            cornerTL.SetHolderHex(hexGrid[cornerPos]);
            cornerDict.Add(cornerTL.Direction, cornerTL);

            var cornerTR = new Corner(Direction.TopRight);
            cornerPos = new Vector2(cornScope, -cornScope);
            cornerTR.SetHolderHex(hexGrid[cornerPos]);
            cornerDict.Add(cornerTR.Direction, cornerTR);

            var cornerR = new Corner(Direction.Right);
            cornerPos = new Vector2(cornScope, 0);
            cornerR.SetHolderHex(hexGrid[cornerPos]);
            cornerDict.Add(cornerR.Direction, cornerR);

            var cornerBR = new Corner(Direction.BotRight);
            cornerPos = new Vector2(0, cornScope);
            cornerBR.SetHolderHex(hexGrid[cornerPos]);
            cornerDict.Add(cornerBR.Direction, cornerBR);

            var cornerBL = new Corner(Direction.BotLeft);
            cornerPos = new Vector2(-cornScope, cornScope);
            cornerBL.SetHolderHex(hexGrid[cornerPos]);
            cornerDict.Add(cornerBL.Direction, cornerBL);


            foreach (var pair in cornerDict)
            {
                pair.Value.Init();
            }
        }
        public bool IsPositionInPlayScope(Vector2 pos)
        {
            bool ret = false;

            int scopeZ1 = Math.Max(-PlayRad, -pos.X - PlayRad);
            int scopeZ2 = Math.Min(PlayRad, -pos.X + PlayRad);
            if (pos.X >= -PlayRad && pos.X <= PlayRad &&
                        pos.Y >= scopeZ1 && pos.Y <= scopeZ2)
            {
                ret = true;
            }
            return ret;
        }
        public override void Draw()
        {
            //var g = GameScreen.Graphics;
            //var brush = GameScreen.MasterBrush;

            
            foreach (var hexPair in hexGrid)
            {
                hexPair.Value.Draw();
            }

            foreach (var pair in gs.Board)
            {
                if (pair.Value.IsHold)
                {
                    if (!stoneList.ContainsKey(pair.Key))
                    {
                        var pixelPoint = Utils.PositionToPixel(pair.Value.Position, Constants.GRID_ORIGIN, Constants.GRID_HEX_RADIUS);
                        stoneList.Add(pair.Key, new Stone(pixelPoint, GetColorByPlayerId(pair.Value.Holder)));
                    }

                    stoneList[pair.Key].Draw();
                }
            }
            foreach (var pair in gs.Board)
            {
                if (pair.Value.IsHold)
                {
                    hexGrid[pair.Key].TestDrawLabel(pair.Value.Id);
                }
            }

            foreach (var cornerPair in cornerDict)
            {
                cornerPair.Value.Draw();
            }
        }

        private Color GetColorByPlayerId(int playerId)
        {
            if (playerId == 1)
            {
                return Color.White;
            }
            else
                return Color.Black;
        }
    }
}
