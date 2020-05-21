// Container class for avatar/man data, stored in the room object script
using System;
using UnityEngine;

namespace MySpace
{
    [Serializable]
    public class RoomInstanceData
    {
        // Room Characterisation
        public Guid RoomId { get; set; }
        public string RoomName { get; set; }
        public Enums.RoomSizes RoomSize { get; set; }
        public Enums.RoomCategories RoomCategory { get; set; }
        public Enums.RoomTypes RoomType { get; set; }
        public Enums.RoomOverUnder RoomOverUnder{ get; set; }
        public int RoomCost { get; set; }
        public RoomScript RoomScript { get; set; }
        public string RoomDescription { get; set; }

       

        // Room Positioning
        public GridIndex[] CoveredIndizes { get; set; }

        // Avatar Management
        public int ManSlotCount { get; set; } // Number of avatars that are currently in this room '
        
        //Not necessary, since the amount that can own are the amount that can be inside, for now
        //public int OwnerSlotCount { get; set; } // Max Number of avatars that can own this room   

        public Vector3[] ManSlotsPositions { get; set; } // World positions of the avatar slots
        public Quaternion[] ManSlotsRotations { get; set; } // World rotations of the avatar slots
        public Guid[] ManSlotsAssignments { get; set; } // Guids of avatars currently assigned to be in this room
        public Guid[] OwnerSlotsAssignments { get; set; } // Guids of avatars assigned as owners to this room
        public Enums.ManStates[] ManWorkingStates { get; set; }

        public RoomInstanceData()
        {
            
        }

        public GridIndex GetLeftMostIndex()
        {
            return (CoveredIndizes[0]);
        }

        public GridIndex GetRightMostIndex()
        {
            GridIndex index = CoveredIndizes[0];
            foreach (GridIndex i in CoveredIndizes)
            {
                if (i.X > index.X)
                    index = i;
            }
            return index;
        }
    }
}
