using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.Test
{
    public class ManualPlayer : Player
    {
        public ManualPlayer(int playerId,GameState gs):base(playerId,gs)
        {
        }
        public ManualPlayer(Player p) : base(p)
        {
        }

        public void NextCommand(Command cmd)
        {
            if (cmd != null)
            {
                this.nextCommand = cmd;
                this.nextCommand.PlayerId = this.PlayerId;
            }
        }
    }
}
