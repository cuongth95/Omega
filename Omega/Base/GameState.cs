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
        public List<Command> CommandList { get { return commandList; } }
        private List<Command> commandList;
        private List<Command> exeCommandList;
        public List<Player> PlayerList { get { return playerList; } }
        private List<Player> playerList;
        public Queue<int> RoundQueue { get { return roundQueue; } }
        private Queue<int> roundQueue;
        public int CurrentPlayerId { get; private set; }
        public int MapRad { get; private set; }
        public int PlayRad { get; private set; }
        public int HexesPerSide { get; private set; }

        public GameState(GameState gs, bool doVisualize = false)
        {
            this.MapRad = gs.MapRad;
            this.PlayRad = gs.PlayRad;
            this.HexesPerSide = gs.HexesPerSide;
            this.CurrentPlayerId = gs.CurrentPlayerId;
            this.board = Utils.Clone(gs.board);
            if (!doVisualize)
            {
                this.roundQueue = gs.roundQueue;
                this.playerList = gs.playerList;
            }
            else
            {
                this.roundQueue = new Queue<int>(gs.roundQueue);

                this.playerList = Utils.Clone(gs.playerList);
            }
            this.commandList = Utils.Clone(gs.commandList);
            this.exeCommandList = new List<Command>();

            foreach (var player in playerList)
            {
                player.SetGameState(this);
            }

        }
        public GameState(int playHexesPerSide, int hexesPerSide)
        {
            roundQueue = new Queue<int>();
            HexesPerSide = hexesPerSide;
            exeCommandList = new List<Command>();
            commandList = new List<Command>();
            playerList = new List<Player>();
            board = new Dictionary<Vector2, Unit>();
            MapRad = HexesPerSide - 1;
            UpdatePlayBoard(Math.Min(playHexesPerSide - 1, MapRad));
        }

        public List<Vector2> GetAllPositions()
        {
            List<Vector2> posList = new List<Vector2>();
            for (int x = -PlayRad; x <= PlayRad; x++)
            {
                int scopeZ1 = Math.Max(-PlayRad, -x - PlayRad);
                int scopeZ2 = Math.Min(PlayRad, -x + PlayRad);

                for (int z = scopeZ1; z <= scopeZ2; z++)
                {
                    var pos = new Vector2(x, z);
                    posList.Add(pos);
                }
            }
            return posList;
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
                    if (!board.ContainsKey(pos))
                        board.Add(pos, new Unit(pos));
                }
            }
        }

        public void ResetPlayers()
        {
            foreach (var player in playerList)
            {
                player.Reset();
            }
        }
        public void ResetPlayersScore(bool doReset = false)
        {
            foreach (var player in playerList)
            {
                player.Score = player.UnionFinder.GetScore();
            }
        }

        public void RestartNewGame()
        {
            ResetPlayers();
            ResetPlayersScore();
            ResetBoard();
            ResetNewTurn();
            exeCommandList.Clear();
            commandList.Clear();
        }

        public void ResetBoard()
        {
            CurrentPlayerId = playerList[0].PlayerId;
            foreach (var unit in board)
            {
                unit.Value.Holder = Unit.HOLDER_EMPTY;
            }

        }
        public void ResetNewTurn()
        {
            roundQueue.Clear();
            foreach (var player in playerList)
            {
                roundQueue.Enqueue(player.PlayerId);
            }

        }

        public bool IsRoundQueueEmpty()
        {
            return roundQueue.Count <= 0;
        }

        private int GetNextStone()
        {
            var presentIdOfStone = roundQueue.Dequeue();

            return presentIdOfStone;
        }

        public Command GetNextStone(CommandType type, Vector2 pos)
        {
            if (board.ContainsKey(pos))
            {
                Unit unit = board[pos];
                if (type == CommandType.MoveStone)
                {
                    if (!unit.IsHold)
                    {
                        Command ret = new Command(type, roundQueue.Dequeue(), pos);
                        return ret;
                    }
                }
                else if (type == CommandType.Undo)
                {
                    if (unit.Holder == roundQueue.Peek())
                    {
                        Command ret = new Command(type, roundQueue.Dequeue(), pos);
                        return ret;
                    }
                }
            }
            return null;
        }

        public bool IsFreeAtUnit(Vector2 pos)
        {
            if (board.ContainsKey(pos))
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
                    var oldPlayerId= unit.Holder;
                    var player = playerList.Find(x => x.PlayerId == oldPlayerId);


                    var uf= player.UnionFinder;
                    uf.RemoveLastPiece(unit.CountId);
                    unit.CountId = -1;
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
                    var player = playerList.Find(x => x.PlayerId == presentId);
                    int unionCountId = player.UnionFinder.AddPiece();

                    unit.CountId = unionCountId;

                    var dirs = Utils.Directions;
                    for (int i = 0; i < dirs.Length; i++)
                    {
                        var direction = (Direction)dirs.GetValue(i);

                        var neighborPos = unit.GetPosistionOfNeighbor(direction);
                        if (board.ContainsKey(neighborPos)
                            && board[neighborPos].Holder == player.PlayerId
                            && board[neighborPos].CountId >= 0
                            )
                        {

                            player.UnionFinder.Union(board[neighborPos].CountId, unionCountId);

                        }
                    }


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

        public List<int> GetWinners()
        {
            List<int> winnerList = new List<int>();
            long maxScore = 0;

            foreach (var player in playerList)
            {
                if (player.Score > maxScore)
                {
                    maxScore = player.Score;
                    if (!winnerList.Contains(player.PlayerId))
                        winnerList.Add(player.PlayerId);
                }
            }
            return winnerList;
        }

        public int GetWinner() {
            //int winnerId = playerList[0].PlayerId;
            //long maxScore = playerList[0].Score;

            if (playerList[1].Score == playerList[0].PlayerId)
                return 0;
            else if (playerList[1].Score > playerList[0].PlayerId)
                return playerList[1].PlayerId;
            else
                return playerList[0].PlayerId;


            //foreach (var player in playerList)
            //{
            //    if (player.Score >= maxScore)
            //    {
            //        if (maxScore > 0)
            //        {
            //            //draw
            //            winnerId = 0;
            //            break;
            //        }
            //        maxScore = player.Score;
            //        winnerId = player.PlayerId;
            //    }
            //}
            //return winnerId;

        }

        public bool CheckGameOver()
        {
            return CurrentPlayerId == playerList[0].PlayerId && this.board.Count - commandList.Count < Constants.NUM_OF_PLAYERS * Constants.NUM_OF_PLAYERS;
        }

        public void UnionFindAlgorithm()
        {

            foreach (var player in playerList)
            {
                player.Score = player.UnionFinder.GetScore();
            }
        }
        private List<List<int>> sameRegionsList;
        int largestLabel = 0;
        public Dictionary<Region, int> UpdateScores()
        {
            foreach (var player in playerList)
            {
                player.Score = 1;
            }
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

            return regionPoints;

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

        public void Simulate(CommandType type ,Vector2 pos, int playerId)
        {
            var cmd = this.GetNextStone(type, pos);
            if (cmd == null)
            {
                throw new NullReferenceException("GameState-Simulate-Cmd is null because of same pos or wrong pos");
            }
            else
            {
                cmd.PlayerId = playerId;
                if (ExecuteCommand(cmd))
                {
                    commandList.Add(cmd);
                }
                if (roundQueue.Count <= 0)
                {
                    ResetNewTurn();
                    NextPlayer();
                }
            }
        }
            

        public void SimulateCommand(Command cmd,int playerId)
        {
            cmd.PlayerId = playerId;
            cmd.PresentId = this.GetNextStone();
            if (ExecuteCommand(cmd))
            {
                commandList.Add(cmd);
            }
            if (roundQueue.Count <= 0)
            {
                ResetNewTurn();
                NextPlayer();
            }
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

            if (roundQueue.Count <= 0)
            {
                ResetNewTurn();
                NextPlayer();
            }
        }
        private bool ExecuteCommand(Command cmd)
        {
            if (cmd.CmdType == CommandType.MoveStone)
            {
                return this.AddStone(cmd.PresentId, cmd.Position);
            }
            else if (cmd.CmdType == CommandType.Undo)
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


        public GameState Clone(bool doVisualize = false)
        {
            return new GameState(this, doVisualize);
        }
        public void AssignFrom(GameState gs)
        {
            this.MapRad = gs.MapRad;
            this.PlayRad = gs.PlayRad;
            this.HexesPerSide = gs.HexesPerSide;
            this.CurrentPlayerId = gs.CurrentPlayerId;
            this.board = Utils.Clone(gs.board);
            this.roundQueue = gs.roundQueue;
            this.playerList = gs.playerList;
             
            this.commandList = Utils.Clone(gs.commandList);
            this.exeCommandList = new List<Command>();

            foreach (var player in playerList)
            {
                player.SetGameState(this);
            }
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
                if (clone.commandList.Count >0 && clone.roundQueue.Count == clone.playerList.Count)
                {
                    clone.NextPlayer();
                    clone.roundQueue.Dequeue();
                }
                else
                {
                    clone.ResetNewTurn();
                }
                return clone;
            }
            return null;
        }

        private void NextPlayer()
        {
            //CurrentPlayerId = (CurrentPlayerId + 1) % playerList.Count;
            CurrentPlayerId++;
            if (CurrentPlayerId > playerList.Count)
            {
                CurrentPlayerId = 1;
            }
        }

    }
}
