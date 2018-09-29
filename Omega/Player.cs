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
        //Queue<EventHandler> eventQueue;
        public long Score { get; set; }
        EventHandler nextHandler;
        Command nextCommand;
        public Color PresentedColor;
        public Stone DefaultStone { get; set; }
        public int PlayerId { get; }
        public override void Init()
        {
            base.Init();
            nextHandler = null;
        }

        public override void Reset(bool isActive = false)
        {
            nextHandler = null;
            Score = 1;
        }

        public Command GetCommand() {
            var ret = nextCommand;
            nextCommand = null;
            return ret;
        }

        public void NextCommand(Command cmd)
        {
            this.nextCommand = cmd;
            this.nextCommand.PlayerId = this.PlayerId;
        }

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

        //private  ICommand nextCommand;

        //public void NextCommand(ICommand cmd)
        //{
        //    nextCommand = cmd;
        //}
        //public ICommand GetCommand()
        //{
        //    var ret = nextCommand;
        //    nextCommand = null;
        //    return ret;
        //}


        public Player(Color playerId)
        {
            this.PresentedColor = playerId;
        }
        public Player(int playerId)
        {
            this.PlayerId = playerId;
        }
        public Player(Player p)
        {
            this.PlayerId = p.PlayerId;
            this.Id = p.Id;
            this.Score = p.Score;
            this.Tag = p.Tag.ToString();
        }
        public void GameOver(List<int> winnerList)
        {

        }
    }
}
