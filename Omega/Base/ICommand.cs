using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega
{
    public enum CommandType
    {
        MoveStone,
        Undo
    }
    public interface ICommand
    {
        CommandType CmdType { get; }

        void Execute();
        void Undo();
    }

    public class Command : ICommand
    {
        protected CommandType cmdType;
        public int PresentId { get; set; }
        public int PlayerId { get; set; }
        public Vector2 Position { get; set; }

        public CommandType CmdType
        {
            get { return cmdType; }
        }


        public Command(CommandType type,int PresentId, Vector2 position)
        {
            cmdType = type;
            this.PresentId = PresentId;
            this.Position = position;
        }
        public Command(CommandType type, Vector2 position)
        {
            cmdType = type;
            this.Position = position;
        }

        public virtual void Execute()
        {
        }

        public virtual void Undo()
        {
        }
        public Command Clone()
        {
            Command cmd = new Command(this.cmdType, this.PresentId, this.Position);
            cmd.PlayerId = this.PlayerId;
            return cmd;
        }
    }

}
