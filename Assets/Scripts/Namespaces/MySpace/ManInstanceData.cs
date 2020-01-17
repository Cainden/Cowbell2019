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
        public Guid AssignedRoom { get; set; }
        public int AssignedRoomSlot { get; set; }

        public ManInstanceData()
        {

        }

        public string GetManFullName()
        {
            return (ManFirstName + " " + ManLastName);
        }
    }
}
