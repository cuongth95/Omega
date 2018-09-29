using Omega.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega
{
    public class GameState
    {
        public Dictionary<Vector2, Unit> Board { get { return board; } }
        private Dictionary<Vector2, Unit> board;
        private List<Command> commandList;
        private List<Command> exeCommandList;
        private List<Player> playerList;

        public int MapRad { get;  private set; }
        public int PlayRad { get;  private set;}

        public int HexesPerSide { get; private set; }
        public GameState(GameState gs,bool keepPlayerList=true)
        {
            this.MapRad = gs.MapRad;
            this.PlayRad = gs.PlayRad;
            this.HexesPerSide = gs.HexesPerSide;

            this.board = Utils.Clone(gs.board);
            if (keepPlayerList)
                this.playerList = gs.playerList;
            else
                this.playerList = Utils.Clone(gs.playerList);
            this.commandList = Utils.Clone(gs.commandList);
            this.exeCommandList = new List<Command>();
        }
        public GameState(int playHexesPerSide, int hexesPerSide)
        {
            HexesPerSide = hexesPerSide;
            exeCommandList = new List<Command>();
            commandList = new List<Command>();
            playerList = new List<Player>();
            board = new Dictionary<Vector2, Unit>();
            MapRad = HexesPerSide - 1;
            UpdatePlayBoard(Math.Min(playHexesPerSide - 1, MapRad));
        }

        public void UpdatePlayBoard(int newPlayRad)
        {
            this.PlayRad = newPlayRad;
            for (int x = -PlayRad; x <= PlayRad; x++)
            {
                int scopeZ1 = Math.Max(-PlayRad, -x - PlayRad);
                int scopeZ2 = Math.Min(PlayRad, -x + PlayRad);

                for (int z = scopeZ1; z <= scopeZ2; z++)
                {
                    var pos = new Vector2(x, z);
                    if(!board.ContainsKey(pos))
                        board.Add(pos, new Unit(pos));
                }
            }
        }


        public void ResetBoard()
        {
            foreach (var unit in board)
            {
                unit.Value.Holder = Unit.HOLDER_EMPTY;
            }

        }

        public bool IsFreeAtUnit(Vector2 pos)
        {
            if(board.ContainsKey(pos))
            {
                return board[pos].IsHold;
            }
            return false;
        }
        private bool RemoveStone(Vector2 position)
        {
            if (board.ContainsKey(position))
            {
                Unit unit = board[position];
                if (unit.IsHold)
                {
                    unit.Holder = Unit.HOLDER_EMPTY;
                    return true;
                }
                else
                {
                    Console.WriteLine("remove stone in empty position");
                }
            }
            else
            {
                Console.WriteLine("position is out of play board");
            }
            return false;
        }
        /// <summary>
        /// put a stone on board
        /// </summary>
        /// <param name="presentId">color of stone</param>
        /// <param name="position">which unit of board</param>
        /// <returns></returns>
        private bool AddStone(int presentId, Vector2 position)
        {
            if (board.ContainsKey(position))
            {
                Unit unit = board[position];
                if (!unit.IsHold)
                {
                    //commandList.Add(unit);
                    unit.Holder = presentId;
                    return true;
                }
                else
                {
                    Console.WriteLine("add stone in holding position");
                }
            }
            else
            {
                Console.WriteLine("position is out of play board");
            }

            return false;
        }

        public void AddPlayer(Player player)
        {
            this.playerList.Add(player);
        }

        public List<int> GetWinner()
        {
            List<int> winnerList = new List<int>();
            long maxScore = 0;

            foreach (var player in playerList)
            {
                if(player.Score > maxScore)
                {
                    maxScore = player.Score;
                    if(!winnerList.Contains(player.PlayerId))
                        winnerList.Add(player.PlayerId);
                }
            }
            return winnerList;
        }
        public bool CheckGameOver()
        {
            return this.board.Count - commandList.Count < Constants.NUM_OF_PLAYERS * Constants.NUM_OF_PLAYERS;
        }
        private List<List<int>> sameRegionsList;
        int largestLabel = 0;
        public void UpdateScores()
        {
            foreach (var player in playerList)
            {
                player.Score = 1;
            }

            if (CheckGameOver()){
                sameRegionsList = new List<List<int>>();

                for (int z = -PlayRad; z <= PlayRad; z++)
                {

                    int scopeX1 = Math.Max(-PlayRad, -z - PlayRad);
                    int scopeX2 = Math.Min(PlayRad, -z + PlayRad);

                    for (int x = scopeX1; x <= scopeX2; x++)
                    {
                        var pos = new Vector2(x, z);
                        var unit = board[pos];
                        TagMultiLabels(unit, playerList);
                    }
                }

                //for (int i = 0; i < sameRegionsList.Count; i++)
                //{
                //    Console.Write("region " + i + "-{");
                //    for (int j = 0; j < sameRegionsList[i].Count; j++)
                //    {
                //        Console.Write(sameRegionsList[i][j] + ",");
                //    }

                //    Console.WriteLine("}");
                //}

                Dictionary<Region, int> regionPoints = new Dictionary<Region, int>();
                //reupdate
                for (int z = -PlayRad; z <= PlayRad; z++)
                {

                    int scopeX1 = Math.Max(-PlayRad, -z - PlayRad);
                    int scopeX2 = Math.Min(PlayRad, -z + PlayRad);

                    for (int x = scopeX1; x <= scopeX2; x++)
                    {
                        var pos = new Vector2(x, z);
                        var unit = board[pos];
                        if (unit.Id != 0)
                        {
                            //merge same label into first label found
                            foreach (var reg in sameRegionsList)
                            {
                                if (reg.Contains(unit.Id))
                                {
                                    unit.Id = reg[0];


                                    break;
                                }
                            }
                            //remind code CALCULATE TOTAL POINTS AT HERE for saving
                            var region = new Region(unit.Id, unit.Holder);
                            if (regionPoints.ContainsKey(region))
                            {
                                regionPoints[region] = regionPoints[region] + 1;
                            }
                            else
                            {
                                regionPoints[region] = 1;
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
                        if (pair.Key.holderId == player.PlayerId)
                        {
                            player.Score *= pair.Value;
                        }
                    }
                    Console.WriteLine("player " + player.PlayerId + "-score= " + player.Score);

                }

            }
        }

        public Unit GetNeighbor(Vector2 currPos, Direction dir)
        {
            if (board.ContainsKey(currPos))
            {
                Unit currUnit = board[currPos];
                var posNeighbor = currUnit.GetPosistionOfNeighbor(dir);
                if (board.ContainsKey(posNeighbor))
                {
                    return board[posNeighbor];
                }
            }
            return null;
        }
        private bool CheckOccupied(Unit unit, int side)
        {
            if (unit == null || !unit.IsHold || unit.Holder != side)
                return false;
            else
                return true;
        }
        private bool TagLabel(Unit unit, int side)
        {
            var pos = unit.Position;
            if (unit.IsHold && unit.Holder == side)
            {

                var leftUnit = GetNeighbor(pos, Direction.Left);
                var topLeftUnit = GetNeighbor(pos, Direction.TopLeft);
                var topRightUnit = GetNeighbor(pos, Direction.TopRight);

                var occupiedLeft = CheckOccupied(leftUnit, side); 
                var occupiedTopLeft = CheckOccupied(topLeftUnit, side);
                var occupiedTopRight = CheckOccupied(topRightUnit, side);

                if (!occupiedLeft && !occupiedTopLeft && !occupiedTopRight)
                {
                    unit.Id = ++largestLabel;
                }
                else if (occupiedLeft && !occupiedTopLeft && !occupiedTopRight)
                {
                    //hex.Id = FindLabel(leftHex);
                    UnionLabel(unit, leftUnit);

                }
                else if (!occupiedLeft && occupiedTopLeft && !occupiedTopRight)
                {
                    //hex.Id = FindLabel(topLeftHex);
                    UnionLabel(unit, topLeftUnit);

                }
                else if (!occupiedLeft && !occupiedTopLeft && occupiedTopRight)
                {
                    //hex.Id = FindLabel(topRightHex);
                    UnionLabel(unit, topRightUnit);
                }
                else if (occupiedLeft && occupiedTopLeft)
                {
                    UnionLabel(leftUnit, topLeftUnit);
                    unit.Id = FindLabel(leftUnit);
                }
                else if (occupiedLeft && occupiedTopRight)
                {
                    UnionLabel(topRightUnit, leftUnit);
                    unit.Id = FindLabel(leftUnit);
                }
                else if (occupiedTopLeft && occupiedTopRight)
                {
                    UnionLabel(topRightUnit, topLeftUnit);
                    unit.Id = FindLabel(topLeftUnit);
                }
                else if (occupiedTopLeft && occupiedTopRight && occupiedLeft)
                {
                    UnionLabel(leftUnit, topLeftUnit);
                    UnionLabel(topRightUnit, leftUnit);
                    unit.Id = FindLabel(topLeftUnit);
                }
                return unit.Id != 0;
            }
            return false;
        }


        internal void Cycle(float dt)
        {
            

            foreach (var cmd in exeCommandList)
            {
                if (ExecuteCommand(cmd))
                {
                    commandList.Add(cmd);
                }
            }

            exeCommandList.Clear();

        }
        private bool ExecuteCommand(Command cmd)
        {
            if (cmd.CmdType == CommandType.MoveStone)
            {
                return this.AddStone(cmd.PresentId, cmd.Position);
            }
            else if(cmd.CmdType == CommandType.Undo)
            {
                return this.RemoveStone(cmd.Position);
            }
            return false;
        }

        

        public void AddCommand(Command cmd)
        {
            if (cmd != null)
            {
                exeCommandList.Add(cmd);
            }
        }

        private void UnionLabel(Unit unitA, Unit unitB)
        {
            if (unitA.Id != 0 && unitB.Id != 0 && unitA.Id != unitB.Id)
            {
                int unitAContainIndex = -1;
                int unitBContainIndex = -1;

                for (int i = 0; i < sameRegionsList.Count; i++)
                {
                    for (int j = 0; j < sameRegionsList[i].Count; j++)
                    {
                        if (sameRegionsList[i][j] == unitA.Id)
                        {
                            unitAContainIndex = i;
                            if (unitBContainIndex >= 0)
                                break;
                        }
                        if (sameRegionsList[i][j] == unitB.Id)
                        {
                            unitBContainIndex = i;
                            if (unitAContainIndex >= 0)
                                break;
                        }
                    }
                    if (unitAContainIndex >= 0 && unitBContainIndex >= 0)
                        break;
                }

                if (unitAContainIndex < 0 && unitBContainIndex < 0)
                {
                    //create new 
                    var tempList = new List<int>();
                    tempList.Add(unitA.Id);
                    tempList.Add(unitB.Id);
                    sameRegionsList.Add(tempList);
                }
                else if (unitAContainIndex < 0 && unitBContainIndex >= 0)
                {
                    sameRegionsList[unitBContainIndex].Add(unitA.Id);
                }
                else if (unitAContainIndex >= 0 && unitBContainIndex < 0)
                {
                    sameRegionsList[unitAContainIndex].Add(unitB.Id);
                }
                else
                {
                    if (unitAContainIndex != unitBContainIndex)
                    {
                        //hex A and hex B has separated regions
                        List<int> tempList = sameRegionsList[unitBContainIndex];
                        sameRegionsList[unitAContainIndex].AddRange(tempList);
                        sameRegionsList.RemoveAt(unitBContainIndex);
                    }
                }
            }
            else
            {
                unitA.Id = FindLabel(unitB);
            }
        }
        private int FindLabel(Unit unit)
        {
            return unit.Id;
        }

        private void TagMultiLabels(Unit unit, List<Player> playerList)
        {
            for (int i = 0; i < playerList.Count; i++)
            {
                if (TagLabel(unit, playerList[i].PlayerId))
                {
                    break;
                }
            }
        }


        public GameState Clone()
        {
            return new GameState(this);
        }

        public GameState Undo()
        {
            GameState clone = this.Clone();
            if (clone.commandList.Count > 0)
            {
                var lastCmd = clone.commandList[clone.commandList.Count - 1];
                clone.commandList.RemoveAt(clone.commandList.Count - 1);

                lastCmd = new Command(CommandType.Undo, lastCmd.PresentId, lastCmd.Position);

                clone.ExecuteCommand(lastCmd);
                return clone;
            }
            return null;
        }
    }
}
