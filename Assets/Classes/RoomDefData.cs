// Data container class for a room TYPE definition.

using System;
using UnityEngine;

namespace MySpace
{
    public class RoomDefData
    {

        public string RoomName { get; set; }

        public GameObject RoomPrefab { get; set; }

        public Enums.RoomSizes RoomSize { get; set; }

        public Enums.RoomCategories RoomCategory { get; set; }

        public Enums.RoomTypes RoomType { get; set; }

        public int ManSlotCount { get; set; }

        public Enums.ManStates[] ManWorkingStates { get; set; }

        public string RoomDescription { get; set; }

        public Enums.RoomOverUnder RoomOverUnder { get; set; }

        public int RoomCost { get; set; }

        public bool Locked { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomCost">This defaults to 100</param>
        /// <param name="overUnder">OverUnder is defaulted to NEUTRAL, meaning the room will be buildable above and below ground unless otherwise specified!!!</param>
        public RoomDefData
            (
            string roomName, 
            GameObject roomPrefab,
            Enums.RoomSizes roomSize, 
            Enums.RoomTypes roomType, 
            Enums.RoomCategories roomCategory,
            int manSlotCount, 
            Enums.ManStates[] manWorkingStates,
            string roomDescription,
            int roomCost,
            Enums.RoomOverUnder overUnder,
            bool locked
            )
            //end parameters
        {
            RoomName = roomName;
            RoomPrefab = roomPrefab;
            RoomSize = roomSize;
            RoomType = roomType;
            RoomCategory = roomCategory;
            ManSlotCount = manSlotCount;
            ManWorkingStates = manWorkingStates;
            RoomDescription = roomDescription;
            RoomOverUnder = overUnder;
            RoomCost = roomCost;
            Locked = locked;
        }
    }
}
