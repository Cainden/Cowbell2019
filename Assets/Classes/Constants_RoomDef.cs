// Static class for all constants and pre-defs (partial).

namespace MySpace
{
    public static partial class Constants
    {
        // Room definitions
        public static readonly GridIndex EntranceRoomIndex = new GridIndex(0, GridSurfaceY, 0);
        public static readonly Enums.RoomSizes EntranceRoomSize = Enums.RoomSizes.Size4;
        public static readonly Enums.RoomTypes EntranceRoomType = Enums.RoomTypes.OverLobby;
       // public static readonly Enums.RoomOverUnder EntranceRoomOverUnder = Enums.RoomOverUnder.Over;
        public static readonly GridIndex UWEntranceRoomIndex = new GridIndex(0, GridSurfaceY - 1, 0);
        public static readonly Enums.RoomSizes UWEntranceRoomSize = Enums.RoomSizes.Size4;
        public static readonly Enums.RoomTypes UWEntranceRoomType = Enums.RoomTypes.UnderLobby;
        //public static readonly Enums.RoomOverUnder UWEntranceRoomOverUnder = Enums.RoomOverUnder.Under;

        // Room data - might be moved to a file or database or so later. Unique index is combo size+type
        public static readonly RoomDefData[] RoomDefinitions = new RoomDefData[]
        {
        //////////////////////////////////////////////////////////////
        //NEUTRAL ROOMS
        /////////////////////////////////////////////////////////////
        new RoomDefData (
            "Common Room Size 2",
            "Room_Prefabs/Room_All_Standard_Sz2",
            Enums.RoomSizes.Size2,
            Enums.RoomTypes.Common,
            Enums.RoomCategories.Standard,
            2,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle },
            "A standard room of small size. Can hold 2 men."
            ),

        new RoomDefData (
            "Common Room Size 4",
            "Room_Prefabs/Room_All_Standard_Sz4",
            Enums.RoomSizes.Size4,
            Enums.RoomTypes.Common,
            Enums.RoomCategories.Standard,
            4,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle },
            "A standard room of medium size. Can hold 4 men."
            ),

        new RoomDefData (
            "Common Room Size 6",
            "Room_Prefabs/Room_All_Standard_Sz6",
            Enums.RoomSizes.Size6,
            Enums.RoomTypes.Common,
            Enums.RoomCategories.Standard,
            6,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle },
            "A standard room of large size. Can hold 6 men."
            ),

        new RoomDefData (
            "Elevator",
            "Room_Prefabs/Room_Other_Elevator_Sz1",
            Enums.RoomSizes.Size1,
            Enums.RoomTypes.Elevator,
            Enums.RoomCategories.Other,
            0,
            new Enums.ManStates[] {  },
            "The elevator room is of size 1. It can not hold any men."
            ),
        //////////////////////////////////////////////////////////////
        // OVERWORLD ROOMS
        /////////////////////////////////////////////////////////////
        new RoomDefData (
            "Overworld Lobby",
            "Room_Prefabs/Room_All_Standard_Sz4",
            Enums.RoomSizes.Size4,
            Enums.RoomTypes.OverLobby,
            Enums.RoomCategories.Standard,
            4,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle },
            "The Overworld Hootel's Lobby. Our main Owl can be found here.",
            Enums.RoomOverUnder.Over
            ),
         new RoomDefData (
            "Overworld Room Size 2",
            "Room_Prefabs/Room_All_Standard_Sz2",
            Enums.RoomSizes.Size2,
            Enums.RoomTypes.Common,
            Enums.RoomCategories.Standard,
            2,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle },
            "A Overworld room of small size. Can hold 2 men.",
            Enums.RoomOverUnder.Over
            ),

        new RoomDefData (
            "Overworld Room Size 4",
            "Room_Prefabs/Room_All_Standard_Sz4",
            Enums.RoomSizes.Size4,
            Enums.RoomTypes.Common,
            Enums.RoomCategories.Standard,
            4,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle },
            "A Overworld room of medium size. Can hold 4 men.",
            Enums.RoomOverUnder.Over
            ),

        new RoomDefData (
            "Overworld Room Size 6",
            "Room_Prefabs/Room_All_Standard_Sz6",
            Enums.RoomSizes.Size6,
            Enums.RoomTypes.Common,
            Enums.RoomCategories.Standard,
            6,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle },
            "A Overworld room of large size. Can hold 6 men.",
            Enums.RoomOverUnder.Over
            ),
                new RoomDefData (
            "Bedroom Size 2",
            "Room_Prefabs/Room_All_Standard_Sz2",
            Enums.RoomSizes.Size2,
            Enums.RoomTypes.Bedroom,
            Enums.RoomCategories.Standard,
            2,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle },
            "A Bedroom of small size. Can hold 2 men.",
            Enums.RoomOverUnder.Over
            ),

        new RoomDefData (
            "Bedroom Size 4",
            "Room_Prefabs/Room_All_Standard_Sz4",
            Enums.RoomSizes.Size4,
            Enums.RoomTypes.Bedroom,
            Enums.RoomCategories.Standard,
            4,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle },
            "A bedroom of medium size. Can hold 4 men.",
            Enums.RoomOverUnder.Over
            ),

        new RoomDefData (
            "Bedroom Size 6",
            "Room_Prefabs/Room_All_Standard_Sz6",
            Enums.RoomSizes.Size6,
            Enums.RoomTypes.Bedroom,
            Enums.RoomCategories.Standard,
            6,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle },
            "A Bedroom of large size. Can hold 6 men.",
            Enums.RoomOverUnder.Over
            ),
        //////////////////////////////////////////////////////////////
        //UNDERWORLD ROOMS
        /////////////////////////////////////////////////////////////
        new RoomDefData (
            "Underworld Lobby",
            "Room_Prefabs/Room_All_Standard_Sz4",
            Enums.RoomSizes.Size4,
            Enums.RoomTypes.UnderLobby,
            Enums.RoomCategories.Standard,
            4,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle },
            "The Underworld Hootel's Lobby. Max can be found here.",
            Enums.RoomOverUnder.Under
            ),
        new RoomDefData (
            "Underworld Room Size 2",
            "Room_Prefabs/Room_All_Standard_Sz2",
            Enums.RoomSizes.Size2,
            Enums.RoomTypes.Common,
            Enums.RoomCategories.Standard,
            2,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle },
            "A Underworld room of small size. Can hold 2 men.",
            Enums.RoomOverUnder.Under
            ),

        new RoomDefData (
            "Underworld Room Size 4",
            "Room_Prefabs/Room_All_Standard_Sz4",
            Enums.RoomSizes.Size4,
            Enums.RoomTypes.Common,
            Enums.RoomCategories.Standard,
            4,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle },
            "A Underworld room of medium size. Can hold 4 men.",
            Enums.RoomOverUnder.Under
            ),

        new RoomDefData (
            "Underworld Room Size 6",
            "Room_Prefabs/Room_All_Standard_Sz6",
            Enums.RoomSizes.Size6,
            Enums.RoomTypes.Common,
            Enums.RoomCategories.Standard,
            6,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle },
            "A Underworld room of large size. Can hold 6 men.",
            Enums.RoomOverUnder.Under
            ),
        };
    }
}
