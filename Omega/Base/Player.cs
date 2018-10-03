using Omega.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega
{
    public class Player:GameObject
    {
        protected GameState gs;
        public void SetGameState(GameState state)
        {
            this.gs = state;
        }
        public long Score { get; set; }
        protected Command nextCommand;
        public int PlayerId { get; }
        public UnionFind UnionFinder { get; set; }

        public override void Init()
        {
            base.Init();
            nextHandler = null;
        }

        public override void Reset(bool isActive = false)
        {
            nextHandler = null;
            Score = 1;
            UnionFinder = new UnionFind();
        }

        public virtual Command GetCommand() {
            var ret = nextCommand;
            nextCommand = null;
            return ret;
        }
        
        public Color PresentedColor;
        public Stone DefaultStone { get; set; }
        EventHandler nextHandler;
        public void NextHandler(EventHandler eventHandler)
        {
            nextHandler = eventHandler;
            //eventQueue.Enqueue(eventHandler);

        }
        public EventHandler GetHumanCommand()
        {
            var ret = nextHandler;
            nextHandler = null;
            return ret;
            //if (eventQueue.Count > 0)
            //    return eventQueue.Dequeue();
            //else
            //    return null;
        }
        public EventHandler GetPrevCommand()
        {
            return nextHandler;
        }

        public Player(int playerId,GameState gs)
        {
            this.PlayerId = playerId;
            this.gs = gs;
            this.UnionFinder = new UnionFind();
        }
        public Player(Player p)
        {
            this.PlayerId = p.PlayerId;
            this.Id = p.Id;
            this.Score = p.Score;
            this.gs = p.gs;
            this.UnionFinder = p.UnionFinder.Clone();
        }
        public virtual void GameOver(List<int> winnerList)
        {

        }
        /// <summary>
        /// -1: LOSE,
        ///  0: DRAW,
        ///  1: PLAYER 1,
        ///  2: PLAYER 2
        /// </summary>
        /// <param name="winner"></param>
        public virtual void GameOver(int winner) { }
    }
}
