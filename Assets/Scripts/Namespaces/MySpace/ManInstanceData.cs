// Container class for avatar/man data, stored in the man object script
using System;
using System.Xml;

namespace MySpace
{
    [Serializable]
    public class ManInstanceData
    {
        // Identification
        public Guid ManId { get; set; }
        public Enums.ManTypes ManType { get; set; }
        public string ManFirstName { get; set; }
        public string ManLastName { get; set; }

        public RoomInstanceData OwnedRoomRef { get; set; } = null;

        // Location
        public RoomScript AssignedRoom { get; set; }//Made this a reference variable since it will be being called much more often in order to have the men communicate with the rooms better
        public int AssignedRoomSlot { get; set; }

        public ManInstanceData()
        {

        }

        public string GetManFullName()
        {
            return (ManFirstName + " " + ManLastName);
        }
    }

    public struct WorkerConstructionData
    {
        public Guid manId;
        public Enums.ManTypes manType;
        public string manFirstName;
        public string manLastName;

        //For stats display on the hire list
        public float physicality, professionalism, intelligence; //base stats
        public float speed, loyalty; //specialty stats
    }

    public struct GuestConstructionData
    {
        public Guid manId;
        public Enums.ManTypes manType;
        public string manFirstName;
        public string manLastName;

        public float dirtiness;
    }
}
