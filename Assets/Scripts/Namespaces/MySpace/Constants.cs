﻿// Static class for all constants and pre-defs (partial).

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace MySpace
{
    public static partial class Constants
    {
        // Grid definitions
        public static readonly GridIndex EntranceRoomIndex = new GridIndex(16, 8, 0);

        public static Vector3 GridCenterPos { get { return new Vector3(EntranceRoomIndex.X * GridElementWidth, 0, 0); } }

        public static readonly int GridSizeX = 32;
        public static readonly int GridSizeY = 20;
        public static readonly int GridSizeZ = 2;
        public static readonly int GridSurfaceY = 8; // First row of 'AboveSurface'. Here, 0..7 = below, 8..11 is above

        public static readonly float GridElementWidth = 2.5f;  // X-Size
        public static readonly float GridElementHeight = 4.0f; // Y-Size
        public static readonly float GridElementDepth = 4.0f;  // Z-Size
        
        // Offset the underwold
        public static readonly float UnderWorldOffset = 10.5f; 

        // Selection definitions
        public static readonly float MouseDragInvokeDownTime = 0.5f;

        // Avatar movements
        public static readonly float GridPositionWalkZOffset = 1.4f; // Z-Offset for the 'walking lane' in rooms
        public static readonly float ManRunSpeed = 2.0f;
        public static readonly float ManWalkSpeed = 1.0f;
        public static readonly float ManWalkLaneZOffset = 1.5f;

        // In/Out fixed movement paths. Overdoing it here a bit, trying to follow Microsoft's design rule for constant arrays
        //private static readonly GridIndex[] _NewManIncomingPath = 
        //{
        //    new GridIndex(-5, 8, 1),
        //    new GridIndex(-2, 8, 1),
        //    new GridIndex(-1, 8, 1),
        //    new GridIndex(-1, 8, 0)
        //    //new Vector3(-18f,  (GridSurfaceY + 0.5f) * GridElementHeight - 0.2f, 6.0f),
        //    //new Vector3(-5.5f, (GridSurfaceY + 0.5f) * GridElementHeight - 0.2f, 6.0f),
        //    //new Vector3(-3.0f, (GridSurfaceY + 0.5f) * GridElementHeight - 0.2f, 4.5f),
        //    //new Vector3(-3.5f, (GridSurfaceY + 0.5f) * GridElementHeight - 0.2f, 1.5f)
        //};
        //public static readonly ReadOnlyCollection<GridIndex> NewManIncomingPath = new ReadOnlyCollection<GridIndex>(_NewManIncomingPath);

        //private static readonly GridIndex[] _ManOutgoingPath = 
        //{
        //    new GridIndex(-1, 8, 0),
        //    new GridIndex(-1, 8, 1),
        //    new GridIndex(-2, 8, 1),
        //    new GridIndex(-5, 8, 1)
        //    //new Vector3(-2.5f, (GridSurfaceY + 0.5f) * GridElementHeight - 0.2f, 1.5f),
        //    //new Vector3(-3.0f, (GridSurfaceY + 0.5f) * GridElementHeight - 0.2f, 4.5f),
        //    //new Vector3(-5.5f, (GridSurfaceY + 0.5f) * GridElementHeight - 0.2f, 6.0f),
        //    //new Vector3(-18f,  (GridSurfaceY + 0.5f) * GridElementHeight - 0.2f, 6.0f)
        //};
        //public static readonly ReadOnlyCollection<GridIndex> ManOutgoingPath = new ReadOnlyCollection<GridIndex>(_ManOutgoingPath);

        // Selector definitions
        //public static readonly Dictionary<Enums.RoomSizes, string> RoomBuildSelectorModels = new Dictionary<Enums.RoomSizes, string>
        //{
        //    { Enums.RoomSizes.Size1, "RoomSelectors/BuildPosSelector_Size1" },
        //    { Enums.RoomSizes.Size2, "RoomSelectors/BuildPosSelector_Size2" },
        //    { Enums.RoomSizes.Size4, "RoomSelectors/BuildPosSelector_Size4" },
        //    { Enums.RoomSizes.Size6, "RoomSelectors/BuildPosSelector_Size6" }
        //};

        // Room cost definitions based on size for now
        /*
        public static readonly Dictionary<Enums.RoomSizes, int> RoomCostDefinitions = new Dictionary<Enums.RoomSizes, int>
        {
            { Enums.RoomSizes.None, 0 },
            { Enums.RoomSizes.Size1, 500 },
            { Enums.RoomSizes.Size2, 1000 },
            { Enums.RoomSizes.Size4, 2000 },
            { Enums.RoomSizes.Size6, 4000 }
        };
        */

        // Man avatar definitions
        public static readonly string ManSelectedMaterial = "Avatars/UnlitYellowMaterial";
        public static readonly string ManGhostMaterial = "Avatars/UnlitGreyMaterial";

        //public static readonly ManDefData[] ManDefinitions = new ManDefData[]
        //{
        //    new ManDefData ("StandardMan", "Avatars/Man_pref", Enums.ManTypes.StandardMan),
        //    new ManDefData ("Cleaner", "Avatars/Man_pref", Enums.ManTypes.Cleaner),
        //    new ManDefData ("Guest", "Avatars/Man_pref", Enums.ManTypes.Guest)
        //};
    }
}