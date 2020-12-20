// Container class for avatar/man data, stored in the man object script
using System;
using System.Xml;
using System.Linq;

namespace MySpace
{
    using Stats;
    [Serializable]
    public class ManInstanceData
    {
        // Identification
        public Guid ManId { get; set; }
        public Enums.ManTypes ManType { get; set; }
        public string ManFirstName { get; set; }
        public string ManLastName { get; set; }
        public GridIndex CurrIndex { get; set; }

        public RoomInstanceData OwnedRoomRef { get; set; } = null;

        // Location
        public RoomScript AssignedRoom { get; set; }//Made this a reference variable since it will be being called much more often in order to have the men communicate with the rooms better
        public int AssignedRoomSlot { get; set; }

        public CharacterSwaper.CharLabel CharSprite { get; set; }

        public ManInstanceData()
        {

        }

        public string GetManFullName()
        {
            return (ManFirstName + " " + ManLastName);
        }
    }

    [Serializable]
    public struct WorkerConstructionData
    {
        public Guid manId;
        public Enums.ManTypes manType;
        public string manFirstName;
        public string manLastName;
        //public GridIndex currIndex; //Will probably need this later once we implement a save system.

        //For stats display on the hire list
        //public float physicality, professionalism, intelligence; //base stats
        //public float speed, loyalty; //specialty stats
        public SpecialtyStat[] specialtyStats;
        public GeneralStat[] generalStats;

        public CharacterSwaper.CharLabel sprite;

        public CharacterSwaper.CharLabel GetRandomizedSprite()
        {
            //Left this here instead of making it stat if we ever want to use information from the character that is going to be created
            int max = 0;
            var types = (from CharacterSwaper.CharLabel c in Enum.GetValues(typeof(CharacterSwaper.CharLabel)) where CheckLabelName(c) select c).ToArray();

            int rand = UnityEngine.Random.Range(0, max); //number from 0-(the number of possible sprites)
            return types[rand];

            bool CheckLabelName(CharacterSwaper.CharLabel c)
            {
                string[] split = c.ToString().Split('_');

                //These are the three strings that workers will use
                if (split[0] == "Jupiter" || split[0] == "Mercury" || split[0] == "Neptune")
                {
                    //Need another check here since workers will have additional sprites for jobs. Need to make sure it's casual clothing.
                    if (split[1] == "Tank" || split[1] == "Red" || split[1] == "Hawiian" || split[1] == "Hawaiin" /*check for a spelling error as well*/)
                    {
                        max++;
                        return true;
                    }
                }
                return false;
            }
        }
    }

    [Serializable]
    public struct GuestConstructionData
    {
        public Guid manId;
        public Enums.ManTypes manType;
        public string manFirstName;
        public string manLastName;
        //public GridIndex currIndex; //Will probably need this later once we implement a save system.
        
        //public float dirtiness;
        public GeneralStat[] generalStats;

        public CharacterSwaper.CharLabel sprite;

        public CharacterSwaper.CharLabel GetRandomizedSprite()
        {
            //Left this here instead of making it stat if we ever want to use information from the character that is going to be created
            int max = 0;
            var types = (from CharacterSwaper.CharLabel c in Enum.GetValues(typeof(CharacterSwaper.CharLabel)) where CheckLabelName(c) select c).ToArray();

            int rand = UnityEngine.Random.Range(0, max); //number from 0-(the number of possible sprites)
            return types[rand];

            bool CheckLabelName(CharacterSwaper.CharLabel c)
            {
                string[] split = c.ToString().Split('_');

                //These are the three strings that guests will use
                if (split[0] == "Mars" || split[0] == "Venus" || split[0] == "Saturn")
                {
                    max++;
                    return true;
                }
                return false;
            }
        }
    }
}
