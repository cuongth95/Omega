using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega
{
    public class Constants
    {

        public const bool DEBUG_DRAW_POSITION_IN_HEX = false;
        public const bool DEBUG_DRAW_LABEL = true;
        public const bool DEBUG_AI_PER_TURN = true;


        public readonly static PointF GRID_ORIGIN = new PointF(Constants.WINDOW_WIDTH / 2, Constants.WINDOW_HEIGHT / 2);
        public readonly static float GRID_HEX_RADIUS = 20;

        public readonly static Color COLOR_BACKGROUND = Color.White;
        public readonly static Color COLOR_DECORATION_RECT = Color.PaleVioletRed;

        internal static readonly int NUM_OF_PLAYERS = 2;
        public const int PLAYSCOPE_HEXS_PER_SIDE = 2;

        public const int MIN_HEXS_PER_SIDE = 10;
        public const int MAX_HEXS_PER_SIDE = 15;

        public const float STONE_RADIUS = 32;

        public const int WINDOW_WIDTH = 1280;
        public const int WINDOW_HEIGHT = 800;

        public const float SPACING = 1.1f;

    }
}
