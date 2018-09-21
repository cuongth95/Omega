using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega
{
    class MoveStoneCommand : ICommand
    {
        Hex lastHex;
        Hex hex;
        private Stone movingStone;
        public MoveStoneCommand(Stone stone, Hex hex)
        {
            this.movingStone = stone;
            this.hex = hex;
            lastHex = null;
        }

        public void Execute()
        {
            lastHex = hex;
            movingStone.MoveTo(hex);
        }
        public void Undo()
        {
            if(lastHex != null)
                movingStone.MoveTo(lastHex);
        }
    }
}
