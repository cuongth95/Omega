using Omega.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.Evaluation
{
    public class EvaluateFunction
    {
        public int checkId;
        public EvaluateFunction(int checkId)
        {
            this.checkId = checkId;
        }
        public virtual int Evaluate(GameState gs) { return -1; }

        public virtual void UpdateResults(int winnerId) { }
    }

    public class LinearEvaluateFunction:EvaluateFunction
    {
        public const int INDEX_GROUP_3 = 0;
        public const int INDEX_GROUP_2 = 1;
        public const int INDEX_GROUP_1 = 2;
        public const int INDEX_GROUP_O3 = 3;

        public const int RESULT_WIN = 0;
        public const int RESULT_DRAW = 1;
        public const int RESULT_LOSE = 2;

        private int[] myGroups;
        private int[] opGroups;

        private int[] myRates;
        private int[] opRates;

        public int[] results;

        public LinearEvaluateFunction(int checkId, int[] myRates, int[] opRates) : base(checkId)
        {
            this.myRates = new int[4];
            this.opRates = new int[4];
            Array.Copy(myRates, this.myRates,myRates.Length);
            Array.Copy(opRates, this.opRates, opRates.Length);
            results = new int[3];

        }

        public override int Evaluate(GameState gs)
        {
            var clone = gs.Clone(true);
            Dictionary<int, List<int>> regionsDict = new Dictionary<int, List<int>>();
            foreach (var player in clone.PlayerList)
            {
                List<int> playerRegions = null;
                player.Score = player.UnionFinder.GetScore(out playerRegions);
                regionsDict.Add(player.PlayerId, playerRegions);
            }
            int evaluation = 0;
            if (clone.CheckGameOver())
            {

                foreach (var player in clone.PlayerList)
                {
                    if (player.PlayerId != this.checkId)
                    {
                        evaluation -= (int)player.Score;
                    }
                    else
                    {
                        evaluation += (int)player.Score;
                    }
                }
                if (evaluation == 0)
                    evaluation = -500;
                else
                    evaluation = (evaluation < 0) ? -1000 : 1000;

                
            }
            else
            {
                myGroups = new int[4];
                opGroups = new int[4];
                foreach (var pair in regionsDict)
                {
                    if (pair.Key == this.checkId)
                    {
                        foreach (var region in pair.Value)
                        {
                            if (region == 3)
                            {
                                myGroups[INDEX_GROUP_3]++;
                            }
                            else if (region == 2)
                            {
                                myGroups[INDEX_GROUP_2]++;
                            }
                            else if(region == 1)
                            {
                                myGroups[INDEX_GROUP_1]++;
                            }
                            else
                            {
                                myGroups[INDEX_GROUP_O3]++;
                            }
                        }

                    }
                    else
                    {
                        foreach (var region in pair.Value)
                        {
                            if (region == 3)
                            {
                                opGroups[INDEX_GROUP_3]++;
                            }
                            else if (region == 2)
                            {
                                opGroups[INDEX_GROUP_2]++;
                            }
                            else if (region == 1)
                            {
                                opGroups[INDEX_GROUP_1]++;
                            }
                            else
                            {
                                opGroups[INDEX_GROUP_O3]++;
                            }
                        }
                    }
                }
                evaluation = Utils.Dot(myGroups, myRates) - Utils.Dot(opGroups, opRates);
            }

            //var count =  clone.CommandList.Count;
            //var lastCmd = clone.CommandList[count - 1];
            //if (lastCmd.PresentId == checkId)
            //{
            //    Unit unit = clone.Board[lastCmd.Position];
            //    var numOfFreeSlot = unit.GetNumberOfFreeSlots(checkId);

            //    evaluation -= numOfFreeSlot*2;
            //}

            return evaluation;
        }
        public override void UpdateResults(int winnerId)
        {
            if (winnerId ==0)
            {
                results[RESULT_DRAW]++;
            }
            else if (winnerId != checkId)
            {
                results[RESULT_LOSE]++;
            }
            else
            {
                results[RESULT_WIN]++;
            }
        }
    }
}
