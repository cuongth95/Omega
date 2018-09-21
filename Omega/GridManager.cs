using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega
{
    class GridManager:GameObject
    {
        //private List<Hex> grid;
        private Dictionary<Vector2, Hex> hexGrid;

        private PointF normal;
        public PointF Origin { get; set; }

        public int PlayRad { get; private set; }
        public int MapRad { get; set; }
        public float UnitRad { get;set; }

        private float angle;
        public float Angle {
            get { return this.angle; }
            set {
                if(this.angle>0)
                    this.angle = value;
            }
        }
        public int TotalPlaygroundHexes { get; private set; }

        private int hexesPerSide;
        public int HexesPerSide
        {
            get { return this.hexesPerSide; }
            set
            {
                if (value >= Constants.MIN_HEXS_PER_SIDE
                    || value <= Constants.MAX_HEXS_PER_SIDE)
                    hexesPerSide = value;
                else
                {
                    Console.WriteLine("hexs per side is invalid");
                }
            }
        }

        public List<Vector2> GetAllPositions()
        {
            return this.hexGrid.Keys.ToList();
        }

        public override void Init()
        {
            //grid = new List<Hex>();
            hexGrid = new Dictionary<Vector2, Hex>();
            this.hexesPerSide =Constants.MIN_HEXS_PER_SIDE;
            this.MapRad = hexesPerSide - 1;
            this.angle = 60f;
            Origin = new PointF(GameScreen.Width/2, GameScreen.Height/2);
            normal= new PointF(1, 0);
            var temp =1/ Math.Cos(angle * Math.PI / 180);
            normal = new PointF(1,- (float)Math.Sqrt( Math.Abs(temp*temp-1)));

            UnitRad = 20;
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
                    hex.Inject(UnitRad, Origin);
                    hexGrid.Add(position,hex);
                }
            }
        }

        public override void Reset(bool isActive = false)
        {
            foreach (var pair in hexGrid)
            {
                pair.Value.Reset();
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
        public Dictionary<Direction,Corner> SetHexagonPlayingScope(int playHexPerSide)
        {
            Dictionary<Direction, Corner> cornerDict = new Dictionary<Direction, Corner>();

            if (playHexPerSide > hexesPerSide)
                return cornerDict;

            PlayRad = Math.Min(playHexPerSide-1, MapRad);
            TotalPlaygroundHexes = 0;
            for (int x = -MapRad; x <= MapRad; x++)
            {
                int z1 = Math.Max(-MapRad, -x - MapRad);
                int z2 = Math.Min(MapRad, -x + MapRad);


                int scopeZ1 = Math.Max(-PlayRad, -x - PlayRad);
                int scopeZ2 = Math.Min(PlayRad , -x + PlayRad);

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
            cornerDict.Add(cornerL.Direction,cornerL);

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
            cornerPos = new Vector2( 0, cornScope);
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
            return cornerDict;

        }

        public bool IsExisted(Vector2 pos)
        {
           
            return hexGrid.ContainsKey(pos);
        }

        public Hex GetHex(Vector2 pos)
        {
            if (IsExisted(pos))
                return hexGrid[pos];
            else
                return null;
        }

        private void TagMultiLabels(Hex hex, List<Player> playerList)
        {
            for(int i = 0; i < playerList.Count; i++)
            {
                if(TagLabel(hex, playerList[i].PresentedColor))
                {
                    break;
                }
            }
        }
        private bool TagLabel(Hex hex,Color side)
        {
            var pos = hex.Position;
            if (hex.IsHold && hex.Holder.Color == side)
            {

                var leftHex = GetNeighborHex(pos, Direction.Left, true);
                var topLeftHex = GetNeighborHex(pos, Direction.TopLeft, true);
                var topRightHex = GetNeighborHex(pos, Direction.TopRight, true);

                var occupiedLeft = CheckOccupied(leftHex, side);
                var occupiedTopLeft = CheckOccupied(topLeftHex, side);
                var occupiedTopRight = CheckOccupied(topRightHex, side);

                if (!occupiedLeft && !occupiedTopLeft && !occupiedTopRight)
                {
                    hex.Id = ++largestLabel;
                }
                else if (occupiedLeft && !occupiedTopLeft && !occupiedTopRight)
                {
                    //hex.Id = FindLabel(leftHex);
                    UnionLabel(hex, leftHex);

                }
                else if (!occupiedLeft && occupiedTopLeft && !occupiedTopRight)
                {
                    //hex.Id = FindLabel(topLeftHex);
                    UnionLabel(hex, topLeftHex);
                    
                }
                else if (!occupiedLeft && !occupiedTopLeft && occupiedTopRight)
                {
                    //hex.Id = FindLabel(topRightHex);
                    UnionLabel(hex, topRightHex);
                }
                else if (occupiedLeft && occupiedTopLeft)
                {
                    UnionLabel(leftHex, topLeftHex);
                    hex.Id = FindLabel(leftHex);
                }
                else if (occupiedLeft && occupiedTopRight)
                {
                    UnionLabel(topRightHex, leftHex);
                    hex.Id = FindLabel(leftHex);
                }
                else if(occupiedTopLeft && occupiedTopRight)
                {
                    UnionLabel(topRightHex, topLeftHex);
                    hex.Id = FindLabel(topLeftHex);
                }
                else if(occupiedTopLeft && occupiedTopRight && occupiedLeft)
                {
                    UnionLabel(leftHex, topLeftHex);
                    UnionLabel(topRightHex, leftHex);
                    hex.Id = FindLabel(topLeftHex);
                }
                return hex.Id != 0;
            }
            return false;
        }

        private bool CheckOccupied(Hex hex,Color side)
        {
            if (hex == null || !hex.IsHold || hex.Holder.Color != side)
                return false;
            else
                return true;
        }


        private List<List<int>> sameRegionsList;
        int largestLabel = 0;
        
        public void TagLabelAlgorithm(List<Player> playerList)
        {
            
            // cluster finding algorithm
            sameRegionsList = new List<List<int>>();
            for (int z = -PlayRad; z <= PlayRad; z++)
            {

                int scopeX1 = Math.Max(-PlayRad, -z - PlayRad);
                int scopeX2 = Math.Min(PlayRad, -z + PlayRad);

                for (int x = scopeX1; x <= scopeX2; x++)
                {
                    var pos = new Vector2( x,z);
                    var hex = hexGrid[pos];
                    //TagLabel(hex, playerList[0].PresentedColor);
                    TagMultiLabels(hex, playerList);
                }
            }

            for (int i = 0; i < sameRegionsList.Count; i++)
            {
                Console.Write("region " + i + "-{");
                for (int j = 0; j < sameRegionsList[i].Count; j++)
                {
                    Console.Write(sameRegionsList[i][j]+",");
                }

                Console.WriteLine("}");
            }

            Dictionary<ColorRegion, int> regionPoints =new  Dictionary<ColorRegion, int>();
            //reupdate
            for (int z = -PlayRad; z <= PlayRad; z++)
            {

                int scopeX1 = Math.Max(-PlayRad, -z - PlayRad);
                int scopeX2 = Math.Min(PlayRad, -z + PlayRad);

                for (int x = scopeX1; x <= scopeX2; x++)
                {
                    var pos = new Vector2(x, z);
                    var hex = hexGrid[pos];
                    if(hex.Id != 0)
                    {
                        //merge same label into first label found
                        foreach (var region in sameRegionsList)
                        {
                            if (region.Contains(hex.Id))
                            {
                                hex.Id = region[0];
                                
                                
                                break;
                            }
                        }
                        //remind code CALCULATE TOTAL POINTS AT HERE for saving
                        var colorRegion = new ColorRegion(hex.Id, hex.Holder.Color);
                        if (regionPoints.ContainsKey(colorRegion))
                        {
                            regionPoints[colorRegion] = regionPoints[colorRegion] + 1;
                        }
                        else
                        {
                            regionPoints[colorRegion] = 1;
                        }
                    }
                }
            }
            //calculate scores of players
            foreach (var player in playerList)
            {
                player.Score = 1;
                foreach (var pair in regionPoints)
                {
                    if (pair.Key.color == player.PresentedColor)
                    {
                        player.Score *= pair.Value;
                    }
                }
                Console.WriteLine("player " + player.PresentedColor + "-score= " + player.Score);

            }
            
        }

        private void UnionLabel(Hex hexA, Hex hexB)
        {
            if (hexA.Id != 0 && hexB.Id != 0 && hexA.Id != hexB.Id)
            {
                int hexAContainIndex = -1;
                int hexBContainIndex = -1;

                for (int i = 0; i < sameRegionsList.Count; i++)
                {
                    for (int j = 0; j < sameRegionsList[i].Count; j++)
                    {
                        if(sameRegionsList[i][j] == hexA.Id) {
                            hexAContainIndex = i;
                            if (hexBContainIndex >= 0)
                                break;
                        }
                        if(sameRegionsList[i][j] == hexB.Id){
                            hexBContainIndex = i;
                            if (hexAContainIndex >= 0)
                                break;
                        }
                    }
                    if (hexAContainIndex >= 0 && hexBContainIndex >= 0)
                        break;
                }

                if(hexAContainIndex <0 && hexBContainIndex <0)
                {
                    //create new 
                    var tempList = new List<int>();
                    tempList.Add(hexA.Id);
                    tempList.Add(hexB.Id);
                    sameRegionsList.Add(tempList);
                }
                else if(hexAContainIndex <0 && hexBContainIndex >= 0)
                {
                    sameRegionsList[hexBContainIndex].Add(hexA.Id);
                }
                else if(hexAContainIndex >=0 && hexBContainIndex < 0)
                {
                    sameRegionsList[hexAContainIndex].Add(hexB.Id);
                }
                else
                {
                    if(hexAContainIndex != hexBContainIndex)
                    {
                        //hex A and hex B has separated regions
                        List<int> tempList = sameRegionsList[hexBContainIndex];
                        sameRegionsList[hexAContainIndex].AddRange(tempList);
                        sameRegionsList.RemoveAt(hexBContainIndex);
                    }
                }
            }
            else
            {
                hexA.Id = FindLabel(hexB);
            }
        }

        private int FindLabel(Hex hex)
        {
            return hex.Id;
        }
        
        public Hex GetHex(int x, int y)
        {
            return GetHex(new Vector2(x, y));
        }
        public Hex GetNeighborHex(Vector2 currPos, Direction dir,bool inGameScope=false)
        {
            if (IsExisted(currPos))
            {
                Hex currHex = hexGrid[currPos];
                var posNeighbor = currHex.GetPosistionOfNeighbor(dir);
                if (IsExisted(posNeighbor))
                {
                    if (inGameScope)
                    {
                        if (IsPositionInPlayScope(posNeighbor))
                        {
                            return hexGrid[posNeighbor];
                        }
                    }
                    else
                        return hexGrid[posNeighbor];

                }
            }
            return null;
        }
        public Hex GetNeighborHex(int currX,int currY, Direction dir)
        {
            return GetNeighborHex(new Vector2(currX, currY), dir);
        }
        public override void Draw()
        {
            //var g = GameScreen.Graphics;
            //var brush = GameScreen.MasterBrush;

            foreach (var pair in hexGrid)
            {
                pair.Value.Draw();
            }

        }
        public void TestDrawLabels()
        {
            foreach (var pair in hexGrid)
            {
                pair.Value.TestDrawLabel();
            }
        }
    }
}
