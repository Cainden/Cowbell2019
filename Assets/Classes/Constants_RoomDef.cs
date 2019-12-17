// Static class for all constants and pre-defs (partial).

namespace MySpace
{
    public static partial class Constants
    {
        // Room definitions
        public static readonly GridIndex EntranceRoomIndex = new GridIndex(0, GridSurfaceY, 0);
        //public static readonly Enums.RoomSizes EntranceRoomSize = Enums.RoomSizes.Size4;
        //public static readonly Enums.RoomOverUnder EntranceRoomOverUnder = Enums.RoomOverUnder.Over;
        public static readonly GridIndex UWEntranceRoomIndex = new GridIndex(0, GridSurfaceY - 1, 0);
        //public static readonly Enums.RoomSizes UWEntranceRoomSize = Enums.RoomSizes.Size4;
        //public static readonly Enums.RoomOverUnder UWEntranceRoomOverUnder = Enums.RoomOverUnder.Under;

        #region Old Room Definitions
        // Room data - might be moved to a file or database or so later. Unique index is combo size+type
        /*
        public static readonly RoomDefData[] RoomDefinitions = new RoomDefData[]
        {
            #region Neutral Rooms
            //////////////////////////////////////////////////////////////
            //NEUTRAL ROOMS
            /////////////////////////////////////////////////////////////
            new RoomDefData (
            "Common Room Size 2",
            "Room_Prefabs/Room_All_Standard_Sz2",
            Enums.RoomSizes.Size2,
            Enums.RoomTypes.Common_Size2,
            Enums.RoomCategories.Standard,
            2,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle },
            "A standard room of small size. Can hold 2 men."
            ),

        new RoomDefData (
            "Common Room Size 4",
            "Room_Prefabs/Room_All_Standard_Sz4",
            Enums.RoomSizes.Size4,
            Enums.RoomTypes.Common_Size4,
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
            Enums.RoomTypes.Common_Size6,
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
            "The elevator room is of size 1. It can not hold any men.",
            175
            ),
            #endregion

            #region Overworld Rooms
            //////////////////////////////////////////////////////////////
            // OVERWORLD ROOMS
            /////////////////////////////////////////////////////////////
        new RoomDefData (
            "Overworld Lobby",
            "Room_Prefabs/Room_All_Standard_Sz4",
            Enums.RoomSizes.Size4,
            Enums.RoomTypes.Lobby,
            Enums.RoomCategories.Standard,
            4,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle },
            "The Overworld Hootel's Lobby. Our main Owl can be found here.",
            0,
            Enums.RoomOverUnder.Over
            ),

        new RoomDefData (
            "Overworld Room Size 2",
            "Room_Prefabs/Room_All_Standard_Sz2",
            Enums.RoomSizes.Size2,
            Enums.RoomTypes.Hallway_Size2,
            Enums.RoomCategories.Standard,
            2,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle },
            "An Overworld room of small size. Can hold 2 men.",
            overUnder: Enums.RoomOverUnder.Over
            ),

        new RoomDefData (
            "Overworld Room Size 4",
            "Room_Prefabs/Room_All_Standard_Sz4",
            Enums.RoomSizes.Size4,
            Enums.RoomTypes.Hallway_Size4,
            Enums.RoomCategories.Standard,
            4,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle },
            "An Overworld room of medium size. Can hold 4 men.",
            overUnder: Enums.RoomOverUnder.Over
            ),

        new RoomDefData (
            "Overworld Room Size 6",
            "Room_Prefabs/Room_All_Standard_Sz6",
            Enums.RoomSizes.Size6,
            Enums.RoomTypes.Hallway_Size6,
            Enums.RoomCategories.Standard,
            6,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle },
            "An Overworld room of large size. Can hold 6 men.",
            overUnder: Enums.RoomOverUnder.Over
            ),
                new RoomDefData (
            "Bedroom Size 2",
            "Room_Prefabs/Room_Bedroom_Sz2",
            Enums.RoomSizes.Size2,
            Enums.RoomTypes.Bedroom_Size2,
            Enums.RoomCategories.Standard,
            2,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle },
            "A Bedroom of small size. Can hold 2 men.",
            overUnder: Enums.RoomOverUnder.Over
            ),

        new RoomDefData (
            "Bedroom Size 4",
            "Room_Prefabs/Room_Bedroom_Sz4",
            Enums.RoomSizes.Size4,
            Enums.RoomTypes.Bedroom_Size4,
            Enums.RoomCategories.Standard,
            4,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle },
            "A bedroom of medium size. Can hold 4 men.",
            150,
            Enums.RoomOverUnder.Over
            ),

        new RoomDefData (
            "Bedroom Size 6",
            "Room_Prefabs/Room_Bedroom_Sz6",
            Enums.RoomSizes.Size6,
            Enums.RoomTypes.Bedroom_Size6,
            Enums.RoomCategories.Standard,
            6,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle },
            "A Bedroom of large size. Can hold 6 men.",
            200,
            Enums.RoomOverUnder.Over
            ),
            #endregion

            #region Underworld Rooms
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
            overUnder: Enums.RoomOverUnder.Under
            ),
        new RoomDefData (
            "Underworld Room Size 2",
            "Room_Prefabs/Room_All_Standard_Sz2",
            Enums.RoomSizes.Size2,
            Enums.RoomTypes.UnderHallway_Size2,
            Enums.RoomCategories.Standard,
            2,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle },
            "An Underworld room of small size. Can hold 2 men.",
            overUnder: Enums.RoomOverUnder.Under
            ),

        new RoomDefData (
            "Underworld Room Size 4",
            "Room_Prefabs/Room_All_Standard_Sz4",
            Enums.RoomSizes.Size4,
            Enums.RoomTypes.UnderHallway_Size4,
            Enums.RoomCategories.Standard,
            4,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle },
            "An Underworld room of medium size. Can hold 4 men.",
            overUnder: Enums.RoomOverUnder.Under
            ),

        new RoomDefData (
            "Underworld Room Size 6",
            "Room_Prefabs/Room_All_Standard_Sz6",
            Enums.RoomSizes.Size6,
            Enums.RoomTypes.UnderHallway_Size6,
            Enums.RoomCategories.Standard,
            6,
            new Enums.ManStates[] { Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle,
                                    Enums.ManStates.Idle, Enums.ManStates.Idle },
            "An Underworld room of large size. Can hold 6 men.",
            overUnder: Enums.RoomOverUnder.Under
            ),
            #endregion
        };
        */
        #endregion
    }
}
