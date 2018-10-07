using Omega.Ai.TT;
using Omega.Evaluation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Omega.Ai
{
    public class ABNMAIPlayer : Player
    {
        private TransitionalTable tt;
        private int childNodes;
        private int leafNodes;
        private int maxDepth;
        public LinearEvaluateFunction ef;
        
        public ABNMAIPlayer(int playerId, GameState gs,int maxDepth) : base(playerId, gs)
        {
            this.maxDepth = maxDepth;
            this.childNodes = 0;
            this.leafNodes = 0;
            bestPoses = new List<Vector2>();
        }
        public ABNMAIPlayer(ABNMAIPlayer p) : base(p)
        {
            this.maxDepth = p.maxDepth;
            this.childNodes = p.childNodes;
            this.leafNodes = p.leafNodes;
        }

        public ABNMAIPlayer(int playerId, GameState gs, int maxDepth, int[] myRates, int[] opRates) : this(playerId, gs, maxDepth)
        {
            ef = new LinearEvaluateFunction(this.PlayerId, myRates, opRates);
            stopWatch = new Stopwatch();
            tt = new TransitionalTable();
            tt.InitZobrishTable(gs);
        }
        public void ReinitZobrishTable(GameState gs)
        {
            tt.InitZobrishTable(gs);
        }

        public override void Reset(bool isActive = false)
        {
            base.Reset(isActive);
        }
        List<Vector2> bestPoses;
        private int turn = 0;
        private int prun = 0;
        private int beginCmdCount;
        private long visitedNodes = 0;
        private Stopwatch stopWatch;
        public override Command GetCommand()
        {
            if (gs.CurrentPlayerId != this.PlayerId)
                return null;
            GetCommandAB2();
            //GetCommandABTT();
            return base.GetCommand();
        }


        #region TT + NEGAMAX
        //TT + negamax
        private void GetCommandABTT()
        {
            if (bestPoses.Count == 0 || bestPoses[0] == Constants.INVALID_VECTOR2)
            {
                Console.WriteLine("turn " + turn);
                turn++;

                visitedNodes = 0;
                prun = 0;
                int alpha = -10000;
                int beta = 10000;
                beginCmdCount = gs.CommandList.Count - 1;

                stopWatch.Start();
                int sco = ABTT(gs, maxDepth, alpha, beta, this.PlayerId, this.PlayerId- 1);
                stopWatch.Stop();

                Console.WriteLine("final score = " + sco + " | searched " + visitedNodes + " nodes| prun "+prun+" | " + stopWatch.ElapsedMilliseconds/1000 + "s");
                stopWatch.Reset();
                nextCommand = gs.GetNextStone(CommandType.MoveStone, bestPoses[0]);
                bestPoses.RemoveAt(0);
            }
            else
            {
                nextCommand = gs.GetNextStone(CommandType.MoveStone, bestPoses[0]);
                bestPoses.RemoveAt(0);
            }
        }

        private int ABTT(GameState gs, int depth, int alpha, int beta, int checkId, int uncheckId)
        {
            //measure 
            visitedNodes++;

            //algorithm init values
            int oldAlpha = alpha;
            int bestValue = 0;
            int value = 0;

            //TT
            var property = tt.Retrieve(gs);
            if (property.depth > depth)
            {
                if (property.flag == TFlag.EXACT_VALUE)
                {
                    return property.score;
                }
                else if (property.flag == TFlag.LOWER_BOUND)
                {
                    alpha = Math.Max(alpha, property.score);
                }
                else
                {
                    beta = Math.Min(beta, property.score);
                }
                if (alpha >= beta)
                {
                    prun++;
                    return property.score;
                }
            }

            // negamax
            bool gameOver = gs.CheckGameOver();
            if (depth == 0 || gameOver)
            {
                ef.checkId = checkId;
                return ef.Evaluate(gs); //- TestEvaluate(gs);//Evaluate(gs,this.PlayerId);//
            }

            if (property.depth > 0)
            {
                //do negamax on gamestate in tt first
                GameState betterState = GenerateByPos(property.bestPoses, gs, 2);
                if (betterState == null)
                {
                    throw new NullReferenceException("ABTT-betterState is null");
                }
                bestValue = -ABTT(betterState, depth - 1, -beta, -alpha, uncheckId, checkId);

                if (depth == maxDepth)
                {
                    bestPoses.Clear();
                    bestPoses.AddRange(property.bestPoses);
                }
                if (bestValue >= beta)
                {
                    prun++;
                    StoreToTable(oldAlpha, beta, gs, property.bestPoses, bestValue, depth);
                }
            }
            else
            {
                bestValue = -10000;
                List<GameState> successorList = GenerateForTwo(gs, 2);
                //Utils.Shuffle<GameState>(successorList);
                for (int child = 0; child < successorList.Count; child++)
                {
                    value = -ABTT(successorList[child], depth - 1, -beta, -alpha, uncheckId, checkId);

                    if (value > bestValue)
                    {
                        bestValue = value;
                        if (depth == maxDepth)
                        {
                            bestPoses.Clear();
                            bestPoses.Add(successorList[child].CommandList[beginCmdCount + 1].Position);
                            if (beginCmdCount + 2 < successorList[child].CommandList.Count)
                            {
                                bestPoses.Add(successorList[child].CommandList[beginCmdCount + 2].Position);
                            }
                        }
                        if (bestValue > alpha)
                        {
                            alpha = bestValue;
                        }
                        if (bestValue >= beta)
                        {
                            prun++;
                            List<Vector2> bestMoves = new List<Vector2>();

                            int count = gs.CommandList.Count;
                            for (int i = count; i < successorList[child].CommandList.Count; i++)
                            {
                                bestMoves.Add(successorList[child].CommandList[i].Position);
                            }
                            
                            //update TT
                            StoreToTable(oldAlpha, beta, gs, bestMoves, bestValue, depth);
                            break;
                        }
                    }
                }
            }
            return bestValue;
        }

        public void StoreToTable(int alpha, int beta, GameState state, List<Vector2> bestMoves, int bestValue, int depth)
        {
            TFlag ttFlag;
            if (bestValue <= alpha)
                ttFlag = TFlag.UPPER_BOUND;
            else if (bestValue >= beta)
                ttFlag = TFlag.LOWER_BOUND;
            else
                ttFlag = TFlag.EXACT_VALUE;

            List<Vector2> cachedList = new List<Vector2>();
            foreach (var bestMove in bestMoves)
            {
                if (bestMove != Constants.INVALID_VECTOR2)
                {
                    cachedList.Add(bestMove);
                }
            }
            if (cachedList.Count > 0)
            {
                if(!tt.TryToStore(state, cachedList, bestValue, ttFlag, depth))
                {
                    throw new NullReferenceException("tt.TryToStore- collision");
                }
            }
            else
            {
                throw new NullReferenceException("StoreToTable- cached list length is 0");
            }
        }
        #endregion


        #region NEGAMAX
        //negamax
        private void GetCommandAB2()
        {
            if (bestPoses.Count == 0 || bestPoses[0] == Constants.INVALID_VECTOR2)
            {
                prun = 0;
                visitedNodes = 0;
                int alpha = -10000;
                int beta = 10000;
                int maxPlayer = this.PlayerId;
                int minPlayer = this.PlayerId - 1;
                //var bestMove = GetBestMove(gs, maxPlayer,minPlayer,alpha,beta);
                // Console.WriteLine("bestMove = " + bestMove);
                //this.nextCommand = bestMove;
                //AB(gs, maxDepth,maxPlayer, minPlayer, alpha, beta);
                beginCmdCount = gs.CommandList.Count - 1;
                //MiniMax(gs,2,MAX, maxPlayer, minPlayer);
                //maxDepth = 2;
                bestPoses.Clear();
                bestPoses.Add(new Vector2(Constants.INVALID_VECTOR2));
                bestPoses.Add(new Vector2(Constants.INVALID_VECTOR2));

                Console.WriteLine("turn " + turn);
                turn++;
                stopWatch.Start();
                int sco = AB2(gs, maxDepth, alpha, beta, this.PlayerId, 1);
                stopWatch.Stop();
                Console.WriteLine("final score = " + sco + " | searched " + visitedNodes + " nodes| prun " + prun + " | " + stopWatch.ElapsedMilliseconds / 1000 + "s");
                stopWatch.Reset();
                //int sco= AB(gs, 7, alpha, beta);
                //nextCommand = gs.GetNextStone(CommandType.MoveStone, bestPos);
                nextCommand = gs.GetNextStone(CommandType.MoveStone, bestPoses[0]);
                bestPoses.RemoveAt(0);
            }
            else
            {
                nextCommand = gs.GetNextStone(CommandType.MoveStone, bestPoses[0]);
                bestPoses.RemoveAt(0);
            }
        }
        private int AB2(GameState gs, int depth, int alpha, int beta, int checkId, int uncheckId)
        {
            visitedNodes++;
            int score = 0;
            int value = 0;
            bool gameOver = gs.CheckGameOver();
            if (depth == 0 || gameOver)
            {
                ef.checkId = checkId;
                return ef.Evaluate(gs); //- TestEvaluate(gs);//Evaluate(gs,this.PlayerId);//
            }
            score = -10000;
            List<GameState> successorList = GenerateForTwo(gs, checkId);
            //Utils.Shuffle<GameState>(successorList);
            for (int child = 0; child < successorList.Count; child++)
            {
                value = -AB2(successorList[child], depth - 1, -beta, -alpha, uncheckId, checkId);

                if (value > score)
                    score = value;
                if (score > alpha)
                {
                    if (depth == maxDepth)
                    {
                        var pos = successorList[child].CommandList[beginCmdCount + 1].Position;
                        bestPoses[0] = pos;
                        if (beginCmdCount + 2 < successorList[child].CommandList.Count)
                        {
                            pos = successorList[child].CommandList[beginCmdCount + 2].Position;
                            bestPoses[1] = pos;
                        }
                    }
                    alpha = score;
                }
                if (score >= beta)
                {
                    prun++;
                    break;
                }

            }

            return score;
        }

        #endregion


        private GameState GenerateByPos(List<Vector2> poses, GameState gs, int playerId)
        {
            GameState childState = null;

            if (poses.Count == 1)
            {
                var pos = poses[0];
                if (!gs.Board[pos].IsHold)
                {
                    childState = gs.Clone(true);
                    childState.Simulate(CommandType.MoveStone, pos, playerId);
                }
            }
            else if(poses.Count == 2)
            {
                if (!gs.Board[poses[0]].IsHold)
                {
                    var nextState = gs.Clone(true);
                    nextState.Simulate(CommandType.MoveStone, poses[0], playerId);
                    if (nextState.CheckGameOver() || poses[0].Equals(poses[1]))
                    {
                        childState = nextState;
                        
                    }
                    else
                    {
                        if (!nextState.Board[poses[1]].IsHold)
                        {
                            childState = nextState.Clone(true);
                            childState.Simulate(CommandType.MoveStone, poses[1], playerId);
                            
                        }
                    }
                }
            }
            return childState;
        }



        private List<GameState> GenerateForTwo(GameState gs, int playerId)
        {
            List<GameState> childList = new List<GameState>();
            var board = gs.Board;
            var posList = gs.GetAllPositions();


            for (int i = 0; i < posList.Count; i++)
            {
                if (!board[posList[i]].IsHold)
                {
                    var nextState = gs.Clone(true);
                    nextState.Simulate(CommandType.MoveStone, posList[i], playerId);

                    if (nextState.CheckGameOver())
                        childList.Add(nextState);
                    else
                    {
                        for (int j = 0; j < posList.Count; j++)
                        {
                            if (i != j && !board[posList[j]].IsHold)
                            {
                                var childState = nextState.Clone(true);
                                childState.Simulate(CommandType.MoveStone, posList[j], playerId);
                                childList.Add(childState);
                            }
                        }
                    }
                }
            }


            return childList;
        }
        private List<GameState> Generate(GameState gs,int playerId)
        {
            List<GameState> childList = new List<GameState>();
            var board = gs.Board;
            var posList = gs.GetAllPositions();

            
            for (int i = 0; i < posList.Count; i++)
            {
                if (!board[posList[i]].IsHold)
                {
                    var nextState = gs.Clone(true);
                    //nextState.SimulateCommand(new Command(CommandType.MoveStone, posList[i]), playerId);
                    nextState.Simulate(CommandType.MoveStone, posList[i],playerId);
                    childList.Add(nextState);
                    //for (int j = 0; j < posList.Count; j++)
                    //{
                    //    if (i != j && !board[posList[j]].IsHold)
                    //    {
                    //        var childState = nextState.Clone(true);
                    //        childState.SimulateCommand(new Command(CommandType.MoveStone, posList[j]),playerId);
                    //        childList.Add(childState);
                    //    }
                    //}
                }
            }


            return childList;
        }

        public override void GameOver(int winner)
        {
            ef.UpdateResults(winner);
        }
        

        private int TestEvaluate(GameState gs)
        {
            int evaluation = 0;
            var clone = gs.Clone(true);
            Dictionary<int, List<int>> regionsDict = new Dictionary<int, List<int>>();
            foreach (var player in clone.PlayerList)
            {
                List<int> playerRegions = null;
                player.Score = player.UnionFinder.GetScore(out playerRegions);
                regionsDict.Add(player.PlayerId, playerRegions);
            }

            if (clone.CheckGameOver())
            {
                
                foreach (var player in clone.PlayerList)
                {
                    if (player.PlayerId != this.PlayerId)
                    {
                        evaluation -= (int)player.Score;
                    }
                    else
                    {
                        evaluation += (int)player.Score;
                    }
                }
                if (evaluation == 0)
                    evaluation = 500;
                else
                    evaluation = (evaluation < 0) ? -1000 : 1000;
            }
            else
            {


                foreach (var pair in regionsDict)
                {
                    if (pair.Key == this.PlayerId)
                    {
                        foreach (var region in pair.Value)
                        {
                            if (region == 3)
                            {
                                evaluation += 20;
                            }
                            else if (region < 3)
                            {
                                evaluation += 10;
                            }
                            else
                            {
                                //if (region > 3)
                                evaluation -= 15;
                            }
                        }

                    }
                    else
                    {
                        foreach (var region in pair.Value)
                        {
                            if (region == 3)
                            {
                                evaluation -= 10;
                            }
                            else if (region < 3)
                            {
                                evaluation -= 5;
                            }
                            else
                            {
                                evaluation += 2 * region;
                            }
                        }
                    }
                }
            }
            return evaluation;
        }

        private int Evaluate(GameState gs,int checkId)
        {
            int evaluation = 0;

            var clone = gs.Clone(true);
            if (clone.CheckGameOver())
            {
                clone.UpdateScores();


                int myScore = 0;
                foreach (var player in clone.PlayerList)
                {
                    if (player.PlayerId == checkId)
                    {
                        myScore = (int)player.Score; // temp
                        break;
                    }
                }
                foreach (var player in clone.PlayerList)
                {
                    if (player.PlayerId != checkId)
                    {
                        if (myScore > player.Score)
                        {
                            evaluation = 1000;
                        }
                        else
                        {
                            evaluation = - 1000;
                        }
                        break;
                    }
                }
            }
            else
            {
                var regionPoints= clone.UpdateScores();
                int myScore = 1, oppScore = 1;
                foreach (var pair in regionPoints)
                {
                    if (pair.Key.holderId == checkId)
                    {
                        if (pair.Value == 3)
                        {
                            evaluation += 10;
                        }
                        else if (pair.Value == 2)
                        {
                            evaluation += 8;
                        }
                        else if (pair.Value == 1)
                        {
                            evaluation += 3;
                        }
                        else
                        {
                            evaluation -= 2;
                        }
                    }
                    else
                    {
                        if (pair.Value == 3)
                        {
                            evaluation -= 10;
                        }
                        else if (pair.Value == 2)
                        {
                            evaluation -= 8;
                        }
                        else if (pair.Value == 1)
                        {
                            evaluation -= 5;
                        }
                        else
                        {
                            evaluation -= 2;
                        }
                    }
                }

                //var regionPoints= clone.UpdateScores();
                //foreach (var pair in regionPoints)
                //{
                //    if (pair.Key.holderId == checkId)
                //    {
                //        if(pair.Value == 3)
                //        {
                //            evaluation += 10;
                //        }
                //        else if(pair.Value == 2)
                //        {
                //            evaluation += 8;
                //        }
                //        else if(pair.Value == 1)
                //        {
                //            evaluation += 1;
                //        }
                //        else
                //        {
                //            evaluation += 5;
                //        }
                //    }
                //    else
                //    {
                //        if (pair.Value == 3)
                //        {
                //            evaluation -= 10;
                //        }
                //        else if (pair.Value == 2)
                //        {
                //            evaluation -= 8;
                //        }
                //        else if (pair.Value == 1)
                //        {
                //            evaluation -= 1;
                //        }
                //        else
                //        {
                //            evaluation -= 5;
                //        }
                //    }
                //}
            }

            return evaluation;
        }
        

    }
}
