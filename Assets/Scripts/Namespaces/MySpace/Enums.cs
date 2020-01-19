﻿using System;

namespace MySpace
{
    public static class Enums
    {
        // Game states
        public enum GameStates { None, Normal, GuiBlocking, BuildRoom, RoomSelected, ManPressed, ManSelected, ManDragging, ChangeOwnedRoom };

        // Cursor states
        public enum CursorStates { None, Normal, CamDrag, ManDrag, GuiBlocking };

        // Movement/Grid-Link directions, showing possible movement directions (bitfield). 
        [Flags] public enum MoveDirections : int { Left = 1, Right = 2, Top = 4, Bottom = 8, Front = 16, Back = 32 };

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
        {
            Miscellaneous,
            Utility,
            Overworld,
            Underworld
        }

        public enum RoomTypes
        {
            None,
            Elevator,
            UnderLobby,
            Lobby,
            Bedroom_Size2,
            Bedroom_Size4,
            Bedroom_Size6,
            Hallway_Size2,
            Hallway_Size4,
            Hallway_Size6,
            UnderHallway_Size2,
            UnderHallway_Size4,
            UnderHallway_Size6,
            Common_Size2,
            Common_Size4,
            Common_Size6

        };

        /// <summary>
        /// Use if you want to have a specific display string return instead of just the RoomType enum converted directly into a string. 
        /// </summary>
        /// <param name="type"></param>
        /// <returns>The overidden string name hard-coded, otherwise it will return the type converted into a string.</returns>
        public static string RoomTypeToRoomDisplayName(this RoomTypes type)
        {
            switch (type)
            {
                case RoomTypes.UnderLobby:
                    return "Under Lobby";
                case RoomTypes.Bedroom_Size2:
                    return "Small Bedroom";
                case RoomTypes.Bedroom_Size4:
                    return "Bedroom";
                case RoomTypes.Bedroom_Size6:
                    return "Large Bedroom";
                case RoomTypes.Hallway_Size2:
                    return "Small Hallway";
                case RoomTypes.Hallway_Size4:
                    return "Hallway";
                case RoomTypes.Hallway_Size6:
                    return "Large Hallway";
                case RoomTypes.UnderHallway_Size2:
                    return "Small Under Hallway";
                case RoomTypes.UnderHallway_Size4:
                    return "Under Hallway";
                case RoomTypes.UnderHallway_Size6:
                    return "Large Under Hallway";
                default:
                    return type.ToString();
            }

        }

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