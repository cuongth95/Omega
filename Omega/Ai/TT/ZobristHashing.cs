using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.Ai.TT
{
    public class ZobristHashing
    {
        //https://en.wikipedia.org/wiki/Zobrist_hashing
        
        ulong[,] table;
        public void Init(GameState gs)
        {
            var playerList = gs.PlayerList;
            var board = gs.Board;
            table = new ulong[board.Count,playerList.Count];
            for (int i = 0; i < board.Count; i++)
            {
                for (int j = 0; j < playerList.Count; j++)
                {
                    table[i,j] = Utils.GetRandomBitString();
                }
            }
        }

        public ulong GetHashKey(GameState gs)
        {
            ulong h=0;
            var board = gs.Board;
            int i = 0;
            foreach (var pair in board)
            {
                if (pair.Value.IsHold)
                {
                    int j = pair.Value.Holder - 1;

                    h = h ^ table[i, j];
                }
                i++;
            }

            return h;
        }
    }
}
