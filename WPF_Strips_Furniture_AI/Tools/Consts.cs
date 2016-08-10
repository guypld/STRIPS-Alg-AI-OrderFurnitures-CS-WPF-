using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Strips_Furniture_AI
{
    public class Consts
    {
        public const int BOARD_FREE_SPOT = 0;
        public const int BOARD_WALL_SPOT = -1;

        public const int UPPER_DOOR_SPOT = 2;
        public const int BOTTOM_DOOR_SPOT = 4;
        public const int DOOR_X_POS = 12;
        

        public const int ROOM_1_Y1 = 1;
        public const int ROOM_1_Y2 = 11;
        public const int ROOM_1_X1 = 1;
        public const int ROOM_1_X2 = 11;

        public const int ROOM_2_Y1 = 1;
        public const int ROOM_2_Y2 = 5;
        public const int ROOM_2_X1 = 13;
        public const int ROOM_2_X2 = 23;


        public const int TRY_COUNT = 100;
        public const int MAX_MOVES = 300;

        public const int MAX_STACK_COUNT = 10000;

        public const int MAX_MOVES_TO_FAIL_COUNT = 50000;
    }
}
