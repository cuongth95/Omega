using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega
{
    class Player:GameObject
    {
        //Queue<EventHandler> eventQueue;
        public long Score { get; set; }
        EventHandler nextCommand;
        public Color PresentedColor;
        public Stone DefaultStone { get; set; }
        public override void Init()
        {
            base.Init();
            nextCommand = null;
        }

        public override void Reset(bool isActive = false)
        {
            nextCommand = null;
            Score = 1;
        }

        public void NextCommand(EventHandler eventHandler)
        {
            nextCommand = eventHandler;
            //eventQueue.Enqueue(eventHandler);

        }
        public EventHandler GetCommand()
        {
            var ret = nextCommand;
            nextCommand = null;
            return ret;
            //if (eventQueue.Count > 0)
            //    return eventQueue.Dequeue();
            //else
            //    return null;
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


    }
}
