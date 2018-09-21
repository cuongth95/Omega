using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega
{
    public class GameObject:IDisposable
    {
        public int Id { get; set; }
        public Vector2 Position { get; set; }
        public string Tag { get; set; }
        /// <summary>
        /// Init parameters
        /// </summary>
        public virtual void Init() {
        }
        /// <summary>
        /// Call when restart new game
        /// </summary>
        public virtual void Reset(bool isActive = false)
        {

        }
        /// <summary>
        /// Load resources (sound,img...)
        /// </summary>
        public virtual void Load() { }
        /// <summary>
        /// update method call in duration
        /// </summary>
        public virtual void Update(float dt) { }
        /// <summary>
        /// Render function
        /// </summary>
        public virtual void Draw() { }

        public virtual void Dispose()
        {
        }
    }
}
