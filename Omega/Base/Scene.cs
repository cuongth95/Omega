using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega
{
    public class Scene : GameObject
    {
        //public static System.Drawing.Graphics Graphics
        //{
        //    get; set;
        //}
        //public static Bitmap BackBuffer { get; set; }
        public static int Width
        {
            get; set;
        }
        public static int Height
        {
            get; set;
        }
        
        public virtual void Bind()
        {

        }
        public virtual void Unbind()
        {

        }

        public virtual void OnGUI()
        {

        }
    }
}
