// Data container class for a room TYPE definition.

using System;
using UnityEngine;

namespace MySpace
{
    public class RoomDefData
    {
        private string _RoomName;
        private GameObject _RoomPrefab;
        private Enums.RoomSizes _RoomSize;
        private Enums.RoomCategories _RoomCategory;
        private Enums.RoomTypes _RoomType;
        private int _ManSlotCount;
        private Enums.ManStates[] _ManWorkingStates;  // Per slot, we can assign different (animation) states
        private string _RoomDescription;
        private Enums.RoomOverUnder _RoomOverUnder;
        private int _RoomCost;

        public string RoomName
        {
            get { return _RoomName; }
            set { _RoomName = value; }
        }

        public GameObject RoomPrefab
        {
            get { return _RoomPrefab; }
            set { _RoomPrefab = value; }
        }

        public Enums.RoomSizes RoomSize
        {
            get { return _RoomSize; }
            set { _RoomSize = value; }
        }

        public Enums.RoomCategories RoomCategory
        {
            get { return _RoomCategory; }
            set { _RoomCategory = value; }
        }

        public Enums.RoomTypes RoomType
        {
            get { return _RoomType; }
            set { _RoomType = value; }
        }

        public int ManSlotCount
        {
            get { return _ManSlotCount; }
            set { _ManSlotCount = value; }
        }

        public Enums.ManStates[] ManWorkingStates
        {
            get { return _ManWorkingStates; }
            set { _ManWorkingStates = value; }
        }

        public string RoomDescription
        {
            get { return _RoomDescription; }
            set { _RoomDescription = value; }
        }

        public Enums.RoomOverUnder RoomOverUnder
        {
            get { return _RoomOverUnder; }
            set { _RoomOverUnder = value; }
        }

        public int RoomCost
        {
            get { return _RoomCost; }
            set { _RoomCost = value; }
        }


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
            int roomCost = 100,
            Enums.RoomOverUnder overUnder = Enums.RoomOverUnder.Neutral
            )
            //end parameters
        {
            _RoomName = roomName;
            _RoomPrefab = roomPrefab;
            _RoomSize = roomSize;
            _RoomType = roomType;
            _RoomCategory = roomCategory;
            _ManSlotCount = manSlotCount;
            _ManWorkingStates = manWorkingStates;
            _RoomDescription = roomDescription;
            _RoomOverUnder = overUnder;
            //Based on room size for now
            _RoomCost = roomCost;
        }
    }
}
