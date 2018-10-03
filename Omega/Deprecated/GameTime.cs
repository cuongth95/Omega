using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega
{
    class GameTime
    {
        private Stopwatch stopwatch;

        private long prevTime;
        private long totalTime;

        public float DeltaTime
        {
            get; private set;
        }

        public GameTime()
        {
            stopwatch = new Stopwatch();
            stopwatch.Reset();
        }



        public void Start()
        {
            prevTime = 0;
            totalTime = 0;
            DeltaTime = 0;
            stopwatch.Start();
        }

        public void Tick()
        {
            var currTime = this.stopwatch.ElapsedMilliseconds;
            var elapsedTime = currTime - prevTime;

            if(elapsedTime >= (1.0f/60f))
            {
                
            }
        }


    }
}
