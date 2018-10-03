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
        
        public ABNMAIPlayer(int playerId, GameState gs,int maxDepth) : base(playerId, gs)
        {
            this.maxDepth = maxDepth;
            this.childNodes = 0;
            this.leafNodes = 0;

        }
        public ABNMAIPlayer(ABNMAIPlayer p) : base(p)
        {
            this.maxDepth = p.maxDepth;
            this.childNodes = p.childNodes;
            this.leafNodes = p.leafNodes;
        }

        public override void Reset(bool isActive = false)
        {
            base.Reset(isActive);
        }

        public override Command GetCommand()
        {
            if (gs.CurrentPlayerId != this.PlayerId)
                return null;
            int alpha= -10000;
            int beta = 10000;
            int maxPlayer = this.PlayerId;
            int minPlayer = this.PlayerId - 1;
            //var bestMove = GetBestMove(gs, maxPlayer,minPlayer,alpha,beta);
            // Console.WriteLine("bestMove = " + bestMove);
            //this.nextCommand = bestMove;
            AB(gs, maxDepth,maxPlayer, minPlayer, alpha, beta);

            return base.GetCommand();
        }

        private Command GetBestMove(GameState initGameState, int maxPlayer, int minPlayer, int alpha, int beta)
        {
            Command best = null;

            ABNMNode head = null;

            head = new ABNMNode(ABNMNode.TYPE_MAX_NODE, maxDepth, initGameState, alpha, beta);

            
            

            return best;

        }

        private int AB(GameState gs,int depth, int maxPlayer, int minPlayer, int alpha,int beta)
        {
            int score=0;
            int value = 0;
            bool gameOver = gs.CheckGameOver();
            if(depth == 0 || gameOver)
            {
                return Evaluate(gs);
            }
            score = -10000;
            List<GameState> successorList =  GenerateForTwo(gs,maxPlayer);
            for (int child = 0; child < successorList.Count; child++)
            {
                value = -AB(successorList[child],depth-1,  maxPlayer, minPlayer, -beta,-alpha);
                if (value > score)
                    score = value;
                if (score > alpha)
                {
                    alpha = score;
                    var numOfCommands = successorList[child].CommandList.Count;
                    var delta = numOfCommands - gs.CommandList.Count;
                    if (delta > 1)
                        nextCommand = successorList[child].CommandList[numOfCommands - 2];
                    else
                        nextCommand = successorList[child].CommandList[numOfCommands - 1];
                    
                }
                if (score >= beta)
                    break;

            }

            return score;
        }

        private List<GameState> GenerateForTwo(GameState gs,int playerId)
        {
            List<GameState> childList = new List<GameState>();
            var board = gs.Board;
            var posList = gs.GetAllPositions();

            
            for (int i = 0; i < posList.Count; i++)
            {
                if (!board[posList[i]].IsHold)
                {
                    var nextState = gs.Clone(true);
                    nextState.SimulateCommand(new Command(CommandType.MoveStone, posList[i]), playerId);
                    for (int j = 0; j < posList.Count; j++)
                    {
                        if (i != j && !board[posList[j]].IsHold)
                        {
                            var childState = nextState.Clone(true);
                            childState.SimulateCommand(new Command(CommandType.MoveStone, posList[j]),playerId);
                            childList.Add(childState);
                        }
                    }
                }
            }


            return childList;
        }

        private int Evaluate(GameState gs)
        {
            int evaluation = 0;

            var clone = gs.Clone(true);
            if (clone.CheckGameOver())
            {
                clone.UpdateScores();


                int myScore = 0;
                foreach (var player in clone.PlayerList)
                {
                    if (player.PlayerId == this.PlayerId)
                    {
                        myScore = (int)player.Score; // temp
                        break;
                    }
                }
                foreach (var player in clone.PlayerList)
                {
                    if (player.PlayerId != this.PlayerId)
                    {
                        if (myScore > player.Score)
                        {
                            evaluation = myScore;
                        }
                        else
                        {
                            evaluation = - (int)player.Score;
                        }
                        break;
                    }
                }
            }
            else
            {
                var regionPoints= clone.UpdateScores();
                foreach (var pair in regionPoints)
                {
                    if (pair.Key.holderId == this.PlayerId)
                    {
                        if(pair.Value == 3)
                        {
                            evaluation += 100;
                        }
                        else if(pair.Value == 2)
                        {
                            evaluation += 80;
                        }
                        else if(pair.Value == 1)
                        {
                            evaluation += 10;
                        }
                        else
                        {
                            evaluation += 50;
                        }
                    }
                }
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

        public override void GameOver(int winner)
        {
            base.GameOver(winner);
        }


    }
}
