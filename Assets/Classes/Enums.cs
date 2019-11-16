using System;

namespace MySpace
{
    public static class Enums
    {
        // Game states
        public enum GameStates { None, Normal, GuiBlocking, BuildRoom, RoomSelected, ManPressed, ManSelected, ManDragging, ChangeOwnedRoom };

        // Cursor states
        public enum CursorStates { None, Normal, CamDrag, ManDrag, GuiBlocking };

        // Movement/Grid-Link directions, showing possible movement directions (bitfield). 
        [Flags] public enum MoveDirections : Int32 { Left = 1, Right = 2, Top = 4, Bottom = 8, Front = 16, Back = 32 };

        // Room related enums
        public enum RoomSizes
        { None = 0, Size1 = 1, Size2 = 2, Size4 = 4, Size6 = 6 };

        //Represents whether the room can be built in the Underworld only, Overworld only, or Both
        //Under = -1
        //Both  =  0
        //Over  =  1
        public enum RoomOverUnder
        { Under = -1, Neutral, Over}

        public enum RoomCategories
        { None, Standard, Other }

        public enum RoomTypes
        { None, Common, Elevator, OverLobby, UnderLobby, Bedroom };

        // Avatar/man related enums
        // Negative values are allowed in the Underworld
        // Positive values are allowed in the Overworld
        // 0 is allowed in both.
        public enum ManTypes
        { Max=-2, Monster, None, StandardMan, Cleaner, Guest, MC, }

        public enum ManStates
        { None, Idle, Running, RotatingToPlayer, Rotating, Waiting };
    }
}
