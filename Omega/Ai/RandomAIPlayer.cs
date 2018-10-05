using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.Ai
{
    class RandomAIPlayer : Player
    {
        Random ran;
        private Command bestMove;

        public RandomAIPlayer(int playerId, GameState gs) : base(playerId,gs)
        {
        }

        public RandomAIPlayer(Player p) : base(p)
        {
        }
        public override void Init()
        {
            ran = new Random();
        }
        
        public override Command GetCommand()
        {
            if (gs.CurrentPlayerId != this.PlayerId)
                return null;

            int posX = 0;
            int scopeZ1 =0;
            int scopeZ2 =0;
            int posY = 0;
            Vector2 ranPos = new Vector2();
            do
            {
                posX = ran.Next(0, gs.PlayRad+1);
                posX = ran.Next(0, 2) == 0 ? - posX : posX;
                posY = ran.Next(0, gs.PlayRad +1);
                posY = ran.Next(0, 2) == 0 ? -posY : posY;




                //scopeZ1 = Math.Max(-gs.PlayRad, -posX - gs.PlayRad);
                //scopeZ2 = Math.Min(gs.PlayRad, -posX + gs.PlayRad);
                ranPos.X = posX;
                ranPos.Y = posY;


            } while (!gs.Board.ContainsKey(ranPos)|| gs.Board[ranPos].IsHold);

            this.nextCommand = gs.GetNextStone(CommandType.MoveStone,ranPos);
            this.nextCommand.PlayerId = this.PlayerId;
            return base.GetCommand();
        }
        public override void GameOver(List<int> winnerList)
        {
            base.GameOver(winnerList);
        }
        
    }
}
