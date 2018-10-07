using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.Ai.TT
{
    public class TTProperty
    {
        public List<Vector2> bestPoses;
        public int score;
        public TFlag flag;
        public int depth;

        public TTProperty(List<Vector2> bestPoses, int score, TFlag flag, int depth)
        {
            this.bestPoses = Utils.Clone(bestPoses);
            this.score = score;
            this.flag = flag;
            this.depth = depth;
        }
        public TTProperty Clone()
        {
            return new TTProperty(this.bestPoses,this.score,this.flag,this.depth);
        }
    }

    public class TransitionalTable
    {
        public Dictionary<ulong, TTProperty> dict;
        private ZobristHashing zoHash;

        public TransitionalTable()
        {
            dict = new Dictionary<ulong, TTProperty>();
            zoHash = new ZobristHashing();
        }
        public void InitZobrishTable(GameState initGameState)
        {
            zoHash.Init(initGameState);
        }

        public bool TryToStore(GameState gs, List<Vector2> bestPoses, int score, TFlag flag, int depth)
        {
            ulong hashKey = zoHash.GetHashKey(gs);
            

            if (!dict.ContainsKey(hashKey))
            {
                //Console.WriteLine("TT stored with hash key = " + hashKey);
                dict[hashKey] = new TTProperty(bestPoses, score, flag, depth);
                return true;
            }
            else
                return false;
        }

        public TTProperty Retrieve(GameState gs)
        {
            TTProperty ret = null;
            ulong hashKey = zoHash.GetHashKey(gs);
            

            if (dict.TryGetValue(hashKey, out ret))
                return ret;
            else
            {
                var invalidPoses = new List<Vector2>();
                invalidPoses.Add(new Vector2(Constants.INVALID_VECTOR2));
                ret = new TTProperty(invalidPoses, 0, TFlag.EXACT_VALUE, -1);
                return ret;
            }
        }
    }
}
