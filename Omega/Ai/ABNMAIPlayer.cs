using Omega.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.Ai
{
    public class ABNMAIPlayer : Player
    {
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

        public ABNMAIPlayer(int playerId, GameState gs,int maxDepth, int[]myRates,int[]opRates):this(playerId,gs,maxDepth)
        {
            ef = new LinearEvaluateFunction(this.PlayerId, myRates, opRates);
        }

        public override void Reset(bool isActive = false)
        {
            base.Reset(isActive);
        }
        private int turn = 0;
        private int beginCmdCount;
        public override Command GetCommand()
        {
            if (gs.CurrentPlayerId != this.PlayerId)
                return null;
            if (bestPoses.Count == 0 || bestPoses[0] == Constants.INVALID_VECTOR2)
            {
                int alpha = -10000;
                int beta = 10000;
                int maxPlayer = this.PlayerId;
                int minPlayer = this.PlayerId - 1;
                //var bestMove = GetBestMove(gs, maxPlayer,minPlayer,alpha,beta);
                // Console.WriteLine("bestMove = " + bestMove);
                //this.nextCommand = bestMove;
                //AB(gs, maxDepth,maxPlayer, minPlayer, alpha, beta);
                beginCmdCount = gs.CommandList.Count - 1;
                bestPos = new Vector2(-1000, -1000);
                //MiniMax(gs,2,MAX, maxPlayer, minPlayer);
                //maxDepth = 2;
                bestPoses.Clear();
                bestPoses.Add(new Vector2(Constants.INVALID_VECTOR2));
                bestPoses.Add(new Vector2(Constants.INVALID_VECTOR2));
                Console.WriteLine("turn " + turn);
                turn++;
                int sco = AB2(gs, maxDepth, alpha, beta,this.PlayerId,1);
                Console.WriteLine("final score = " + sco);
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
            return base.GetCommand();
        }

        List<Vector2> bestPoses;
        private Vector2 bestPos;
        private Command GetBestMove(GameState initGameState, int maxPlayer, int minPlayer, int alpha, int beta)
        {
            Command best = null;

            ABNMNode head = null;

            head = new ABNMNode(ABNMNode.TYPE_MAX_NODE, maxDepth, initGameState, alpha, beta);

            
            

            return best;

        }
        const int  MAX = 1;
        const int  MIN = 2;
        private int MiniMax(GameState gs, int depth,int type,int maxPlayerId,int minPLayerId)
        {
            int oldScore = 0;
            int score = 0;
            int value = 0;
            bool gameOver = gs.CheckGameOver();
            if (depth == 0 || gameOver)
            {
                return Evaluate(gs, this.PlayerId);
            }
            if (type == MAX)//max
            {
                score = -10000;
                List<GameState> successorList = Generate(gs, this.PlayerId);
                for (int child = 0; child < successorList.Count; child++)
                {
                    if (successorList[child].RoundQueue.Count == gs.PlayerList.Count)
                        value = MiniMax(successorList[child], depth - 1, MIN,  minPLayerId, maxPlayerId);
                    else
                        value = MiniMax(successorList[child], depth - 1, MAX,maxPlayerId,minPLayerId);
                    if (value > score )
                    {
                        score = value;

                        var numOfCommands = successorList[child].CommandList.Count;
                        bestPos = successorList[child].CommandList[beginCmdCount + 1].Position;
                        
                    }
                }
            }
            else if(type == MIN)//min
            {
                score = 10000;
                List<GameState> successorList = Generate(gs, this.PlayerId-1);
                for (int child = 0; child < successorList.Count; child++)
                {
                    if (gs.RoundQueue.Count <= 0 || gs.RoundQueue.Count == gs.PlayerList.Count)
                        value = MiniMax(successorList[child], depth - 1, MAX, minPLayerId, maxPlayerId);
                    else
                        value = MiniMax(successorList[child], depth - 1, MIN,maxPlayerId,minPLayerId);
                    if (value < score)
                    {
                        score = value;
                    }
                }
            }
            return score;
        }
        private int AB2(GameState gs, int depth, int alpha, int beta,int checkId,int uncheckId)
        {
            int score = 0;
            int value = 0;
            bool gameOver = gs.CheckGameOver();
            if (depth == 0 || gameOver)
            {
                ef.checkId = checkId;
                return ef.Evaluate(gs); //- TestEvaluate(gs);//Evaluate(gs,this.PlayerId);//
            }
            score = -10000;
            List<GameState> successorList = GenerateForTwo(gs, 2);
            Utils.Shuffle<GameState>(successorList);
            for (int child = 0; child < successorList.Count; child++)
            {
                value = -AB2(successorList[child], depth - 1, -beta, -alpha,uncheckId,checkId);
                
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
                    break;
                }

            }

            return score;
        }
        
        

        private int AB(GameState gs,int depth,  int alpha,int beta)
        {
            int score=0;
            int value = 0;
            bool gameOver = gs.CheckGameOver();
            if(depth == 0 || gameOver)
            {
                return TestEvaluate(gs);//Evaluate(gs,this.PlayerId);//
            }
            score = -10000;
            List<GameState> successorList =  Generate(gs,2);
            for (int child = 0; child < successorList.Count; child++)
            {
                if (successorList[child].RoundQueue.Count == gs.PlayerList.Count)
                    value = AB(successorList[child], depth - 1, alpha, beta);
                else
                    value = -AB(successorList[child], depth - 1, -beta, -alpha);

                if (value > score)
                    score = value;
                if (score > alpha)
                {
                    alpha = score;
                    bestPos = successorList[child].CommandList[beginCmdCount + 1].Position;

                }
                if (score >= beta)
                    break;

            }

            return score;
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

        //private int ABNM(ABNMNode node)
        //{
        //    int score = 0;
        //    int value = 0;

        //    if(node.depth == 0)
        //    {
        //        return Evaluate(node.gs);
        //    }

        //    node.childNodeList = Generate(node);

        //    if(node.childNodeList.Count <=0)
        //    {
        //        return Evaluate(node.gs);
        //    }
        //    score = -10000;
        //    for (int i = 0; i < node.childNodeList.Count; i++)
        //    {
        //        var childNode = node.childNodeList[i];
        //        Make(childNode);// execute move
        //        childNode.alpha = -node.beta;
        //        childNode.beta = -Math.Max(node.alpha,score);
        //        childNode.depth = node.depth - 1;
        //        value = -ABNM(childNode);

        //        if(value > score)
        //        {
        //            score = value;
        //        }
        //        Undo(childNode);
        //        if(score >= node.beta)
        //        {
        //            //prunning
        //            return score;
        //        }
        //    }

        //    return score;
        //}

        //private void Undo(ABNMNode node)
        //{
        //    node.gs= node.gs.Undo();
        //}

        //private void Make(ABNMNode node)
        //{

        //}

        //private List<ABNMNode> Generate(ABNMNode node)
        //{
        //    List<ABNMNode> childList = new List<ABNMNode>();
        //    var board = node.gs.Board;
        //    var posList = node.gs.GetAllPositions();

        //    for (int i = 0; i < posList.Count; i++)
        //    {
        //        if (!board[posList[i]].IsHold)
        //        {
        //            for (int j = 0; j < posList.Count; j++)
        //            {
        //                if(j!= i && !board[posList[j]].IsHold)
        //                {
        //                    ABNMNode newNode = new ABNMNode(ABNMNode.TYPE_MIN_NODE,node.depth-1,)
        //                }
        //            }
        //        }
        //    }


        //    return childList;
        //}
        


    }
}
