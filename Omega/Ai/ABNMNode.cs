using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.Ai
{
    public class ABNMNode
    {
        public static readonly int TYPE_MAX_NODE = 0;
        public static readonly int TYPE_MIN_NODE = 1;
        public int type;
        public int depth;
        public GameState gs;
        public Dictionary<Command, float> moves;
        public float alpha;
        public float beta;
        public List<ABNMNode> childNodeList;

        public ABNMNode(int type,int depth, GameState gs, float alpha, float beta)
        {
            this.type = type;
            this.depth = depth;
            this.gs = gs;
            this.alpha = alpha;
            this.beta = beta;
            this.moves = new Dictionary<Command, float>();
            this.childNodeList = new List<ABNMNode>();
        }

    }
}
