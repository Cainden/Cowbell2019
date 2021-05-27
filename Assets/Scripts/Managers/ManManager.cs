// Handling all avatar related actions

using System;
using System.Collections.Generic;
using System.Linq;
using MySpace;
using UnityEngine;

using ManLookup = System.Collections.Generic.Dictionary<System.Guid, ManScript>;
using ManLookupByType = System.Collections.Generic.Dictionary<MySpace.Enums.ManTypes,
                         System.Collections.Generic.Dictionary<System.Guid, ManScript>>;

public class ManManager : Singleton<ManManager>
{
    #region Inspector Variables

    [SerializeField] GameObject[] ManPrefabs;

    #endregion

    #region Variables

    private ManLookupByType m_manList = new ManLookupByType();

    [HideInInspector]
    public List<ManScript> hireList = new List<ManScript>();
    [HideInInspector]
    public List<ManScript> bookingList = new List<ManScript>();

    public float ManRevenueCalculation
    {
        get
        {
            float totalRevenue = 0;
            foreach (ManLookup manLookup in m_manList.Values)
            {
                if (manLookup != null)
                {
                    foreach (ManScript man in manLookup.Values)
                    {
                        totalRevenue += man.GetNetRevenueCalculation;
                    }
                }
            }
            return totalRevenue;
        }
    }

    #endregion

    

    protected override void Awake()
    {
        base.Awake();
    }

    public bool IsManExisting(ManScript man)
    {
        if (m_manList.ContainsKey(man.ManType))
        {
            if (m_manList[man.ManType] != null)
            {
                return m_manList[man.ManType].ContainsKey(man.ManData.ManId);
            }
        }

        return false;
    }

    public int GetActiveNumberOfManType(Enums.ManTypes manType)
    {
        if (m_manList.ContainsKey(manType))
        {
            if (m_manList[manType] != null)
            {
                return m_manList[manType].Count();
            }
        }

        return 0;
    }

    private int GetNumberOfManObjects()
    {
        int total = 0;

        foreach (ManLookup manLookup in m_manList.Values)
        {
            if (manLookup != null)
            {
                total += manLookup.Count;
            }
        }

        return total;
    }

    private void AddToList(ManScript man)
    {
        if (m_manList.ContainsKey(man.ManType) == false)
        {
            m_manList.Add(man.ManType, new ManLookup());
        }

        m_manList[man.ManType].Add(man.ManData.ManId, man);
        GuiManager.Ref.UpdateManCount(GetNumberOfManObjects());
    }

    public GameObject CreateMan(Guid manId, Enums.ManTypes manType)
    {
        ManInstanceData ManData = new ManInstanceData();
        ManData.ManId = manId;
        ManData.ManType = manType;
        ManData.ManFirstName = NameFactory.GetNewFirstName();
        ManData.ManLastName = NameFactory.GetNewLastName();

        return CreateMan(ManData);
    }

    public GameObject CreateWorker(WorkerConstructionData data)
    {
        ManScript_Worker script = InstantiateMan(data.manType).GetComponent<ManScript_Worker>();

        script.ManData = new ManInstanceData()
        {
            ManId = data.manId,
            ManType = data.manType,
            ManFirstName = data.manFirstName,
            ManLastName = data.manLastName,
            CharSprite = data.sprite
        };

        //Set Stats
        script.specialStats = data.specialtyStats;
        script.genStats = data.generalStats;
        script.AssignCharacterSpriteByCharacterType();

        AddToList(script);
        return script.gameObject;
    }

    public GameObject CreateGuest(GuestConstructionData data)
    {
        ManScript_Guest script = InstantiateMan(data.manType).GetComponent<ManScript_Guest>();

        script.ManData = new ManInstanceData()
        {
            ManId = data.manId,
            ManType = data.manType,
            ManFirstName = data.manFirstName,
            ManLastName = data.manLastName,
            CharSprite = data.sprite
        };

        //Set Stats
        script.genStats = data.generalStats;
        script.AssignCharacterSpriteByCharacterType();

        AddToList(script);
        return script.gameObject;
    }

    public GameObject CreateMan(ManInstanceData manData)
    {
        if (manData == null) return null;

        GameObject ManObject = InstantiateMan(manData.ManType);
        ManScript ManScript = ManObject.GetComponent<ManScript>();
        ManScript.ManData = manData;

        AddToList(ManScript);

        return ManScript.gameObject;
    }

    public ManScript FindManByGuid(Guid manId)
    {
        ManScript man = null;

        foreach (ManLookup manLookup in m_manList.Values)
        {
            if (manLookup.TryGetValue(manId, out man))
            {
                break;
            }
        }

        return man;
    }

    public void RemoveManFromList(ManScript man)
    {
        Debug.Assert(IsManExisting(man));

        if (m_manList.ContainsKey(man.ManType))
        {
            if(m_manList[man.ManType] != null)
            {
                m_manList[man.ManType].Remove(man.ManData.ManId);
            }
        }

        GuiManager.Ref.UpdateManCount(GetNumberOfManObjects());
    }

    public void MakeManLeave(ManScript man)
    {
        // Give path to entrance (if assigned to any room. Otherwise, it is the one waiting at the entrance)
        if (man.IsAssignedToAnyRoom())
        {
            if (GameManager.Debug)
                Debug.Log("Guest had room and is being removed from it");
            //RoomScript RoomScript = RoomManager.Ref.GetRoomData(ManScript.ManData.AssignedRoom).RoomScript;
            RoomScript RoomScript = man.ManData.AssignedRoom;

            //Find the path from their current location to the entrance point of the hootel.
            GridIndex[] Pathindizes = GridManager.Ref.GetIndexPath(RoomScript.RoomData.CoveredIndizes[man.ManData.AssignedRoomSlot],
                                                                   Constants.EntranceRoomIndex);

            if (Pathindizes.Length > 0) AddIndexPathToManScript(Pathindizes, man);

            RoomScript.RemoveManFromRoomSlot(man.ManData.ManId);
        }
        else
        {
            StateManager.Ref.SetWaitingMan(null);
        }

        //Submit self-destruction of object once the character reaches the end point of the path.
        man.AddActionToEndOfMovement(man.BeginAnnihilation);
        
    }

    public ManScript[] GetAllActiveMenOfType(Enums.ManTypes manType)
    {
        if (m_manList.ContainsKey(manType))
        {
            if(m_manList[manType] != null)
            {
                return m_manList[manType].Values.ToArray<ManScript>();
            }
        }

        return null;
    }

    public void MoveManToClosestRoomOfType<T>(ManScript man) where T : Room_CleanerCloset
    {
        RoomScript chosen = null;
        int shortest = int.MaxValue;

        foreach (T room in (from T r in RoomManager.Ref.GetAllActiveRoomsofType<T>() where r.RoomHasFreeManSlots(man.ManData.ManId) select r))
        {
            var path = GridManager.Ref.GetIndexPath(man.ManData.AssignedRoom.RoomData.CoveredIndizes[man.ManData.AssignedRoomSlot], room.RoomData.CoveredIndizes[room.CountMen()]);
            if (path.Length < shortest)
            {
                chosen = room;
                shortest = path.Length;
            }
        }
        if (chosen != null)
            MoveManToNewRoom(man, chosen);
    }


    public void MoveManToNewRoom(ManScript man, RoomScript newRoom, bool fromPlayer = false)
    {
        if (RoomManager.Ref.IsRoomExisting(newRoom.RoomData.RoomId) == false) return;
        if (IsManExisting(man) == false) return;

        //NPC is Overworld only trying to travel to Underworld
        if (man.ManData.ManType > 0 && newRoom.RoomData.RoomOverUnder < 0)
        {
            goto CannotAssign;
        }

        //NPC is Underworld only trying to travel to Overworld
        if (man.ManData.ManType < 0 && newRoom.RoomData.RoomOverUnder > 0)
        {
            goto CannotAssign;
        }


        if (newRoom.RoomContainsMan(man.ManData.ManId))
        {
            GuiManager.Ref.Initiate_UserInfoSmall("Already assigned to this room!");
            return;
        }

        if ((man.IsAssignedToAnyRoom() == false) && (newRoom.RoomHasFreeManSlots(man.ManData.ManId) == false))
        {
            goto CannotAssign;
        }

        if (fromPlayer && !RoomManager.IsRoomOfType<Room_WorkQuarters>(newRoom) && man.ManType == Enums.ManTypes.Worker)
        {
            goto CannotAssign;
        }

        if ((man.IsAssignedToAnyRoom() == false) && (newRoom.RoomHasFreeManSlots(man.ManData.ManId) == true))
        {
            int ManSlotIndex = newRoom.GetFreeManSlotIndex(man);
            SetManPathFromEntrance(man, newRoom, ManSlotIndex);
            newRoom.ReserveSlotForMan(man.ManData.ManId, ManSlotIndex, fromPlayer);
            man.AddActionToEndOfMovement(man.AssignToCurrentRoomReservation);
            
            StateManager.Ref.SetWaitingMan(null);
            return;
        }

        Guid OldRoomGuid = man.ManData.AssignedRoom.RoomData.RoomId;
        RoomScript OldRoomScript = RoomManager.Ref.GetRoomData(OldRoomGuid).RoomScript;

        if (newRoom.RoomHasFreeManSlots(man.ManData.ManId) == true)
        {
            int NewManSlotIndex = newRoom.GetFreeManSlotIndex(man);
            int OldManSlotIndex = man.ManData.AssignedRoomSlot;
            SetManPath(man, OldRoomScript, OldManSlotIndex, newRoom, NewManSlotIndex);
            OldRoomScript.RemoveManFromRoomSlot(man.ManData.ManId);
            newRoom.ReserveSlotForMan(man.ManData.ManId, NewManSlotIndex, fromPlayer);
            man.AddActionToEndOfMovement(man.AssignToCurrentRoomReservation);
        }
        else if (man.ManData.ManType == Enums.ManTypes.Worker)//make sure this doesnt happen to guests
        {
            Guid OtherManGuid = newRoom.RoomData.ManSlotsAssignments[0];
            ManScript OtherManScript = FindManByGuid(OtherManGuid); // TODO : This needs null checks below!!

            OldRoomScript.RemoveManFromRoomSlot(man.ManData.ManId);
            newRoom.RemoveManFromRoomSlot(OtherManGuid);

            int NewManSlotIndex1 = newRoom.GetFreeManSlotIndex(man);
            int OldManSlotIndex1 = man.ManData.AssignedRoomSlot;
            SetManPath(man, OldRoomScript, OldManSlotIndex1, newRoom, NewManSlotIndex1);
            newRoom.ReserveSlotForMan(man.ManData.ManId, NewManSlotIndex1, fromPlayer);
            man.AddActionToEndOfMovement(man.AssignToCurrentRoomReservation);

            int NewManSlotIndex2 = OldRoomScript.GetFreeManSlotIndex(OtherManScript);
            int OldManSlotIndex2 = OtherManScript.ManData.AssignedRoomSlot;
            SetManPath(OtherManScript, newRoom, OldManSlotIndex2, OldRoomScript, NewManSlotIndex2);
            OldRoomScript.ReserveSlotForMan(OtherManGuid, NewManSlotIndex2, fromPlayer);
            OtherManScript.AddActionToEndOfMovement(OtherManScript.AssignToCurrentRoomReservation);
        }
        else
            goto CannotAssign;
        return;

        CannotAssign:
        if (fromPlayer)
            GuiManager.Ref.Initiate_UserInfoSmall("Sorry, can't assign to this room!");
        else
            Debug.LogWarning("Attempting to move a man to a room that does not have space, or the man cannot otherwise be assigned to the room.");
    }

    public void RemoveManFromRoom(ManScript man)
    {
        if (man.ManData.AssignedRoom == null) return;
        if (RoomManager.Ref.IsRoomExisting(man.ManData.AssignedRoom.RoomData.RoomId) == false) return;

        RoomScript RoomScript = man.ManData.AssignedRoom;
        RoomScript.RemoveManFromRoomSlot(man.ManData.ManId);        
        man.AssignToRoom(Guid.Empty, 0);
    }

    public void RemoveManOwnershipFromRoom(ManScript man)
    {
        if (RoomManager.Ref.IsRoomExisting(man.ManData.OwnedRoomRef.RoomId) == false) return;

        RoomScript RoomScript = RoomManager.Ref.GetRoomData(man.ManData.OwnedRoomRef.RoomId).RoomScript;
        RoomScript.RemoveOwnerFromRoomSlot(man.ManData.ManId);
        man.SetOwnerOfRoom(Guid.Empty);
    }

    public void TransferOwnershipToRoom(ManScript man, RoomScript room)
    {
        man.TransferOwnershipToNewRoom(room.RoomData.RoomId);
    }

    private void SetManPathFromEntrance(ManScript man, RoomScript newRoom, int newSlotIndex)
    {
        GridIndex index = GridManager.Ref.GetXYGridIndexFromWorldPosition(man.transform.position);
        man.AddMovementAction(index);

        Guid OldRoomGuid = GridManager.Ref.GetGridTileRoomGuid(Constants.EntranceRoomIndex);
        if (OldRoomGuid == newRoom.RoomData.RoomId)
        {
            GridIndex[] Pathindizes = GridManager.Ref.GetIndexPath(index, newRoom.RoomData.CoveredIndizes[newSlotIndex]);
            if (Pathindizes.Length > 0) AddIndexPathToManScript(Pathindizes, man);
        }
        else
            SetManPath(man, RoomManager.Ref.GetRoomData(OldRoomGuid).RoomScript, 0, newRoom, newSlotIndex);
    }

    private void SetManPath(ManScript man, RoomScript oldRoom, int oldSlotIndex, RoomScript newRoom, int newSlotIndex)
    {
        GridIndex[] Pathindizes = GridManager.Ref.GetIndexPath(oldRoom.RoomData.CoveredIndizes[oldSlotIndex],
                                                                newRoom.RoomData.CoveredIndizes[newSlotIndex]);

        if (Pathindizes.Length > 0) AddIndexPathToManScript(Pathindizes, man);
    }

    public void AddIndexPathToManScript(GridIndex[] pathIndizes, ManScript manScript)
    {
        if (pathIndizes == null) return;
        if (manScript == null) return;

        manScript.AddMovementActions(pathIndizes);
    }

    public void SetHighlightedMan(ManScript man)
    {
        Debug.Assert(IsManExisting(man));
        man.SetSelectedState(true);
    }

    public void ResetHighlightedMan(ManScript man)
    {
        if (!IsManExisting(man))
        {
            return;
        }

        man.SetSelectedState(false);
    }

    public void RemoveAllAvatars()
    {
        m_manList.Clear();
    }

    public int GetManCount() // TODO : This is incorrect
    {
        return (m_manList.Count);
    }

    public void SaveManList(string fileName)
    {
        //XmlSerializer Serializer = new XmlSerializer(typeof(ManInstanceData[]));

        //ManInstanceData[] tmpOutput = new ManInstanceData[m_manList.Count];
        
        //// Saving as an array. Key GUID is also in the data
        //int Count = 0;
        //foreach (KeyValuePair<Guid, ManRef<ManScript>> Entry in m_manList)
        //{
        //    tmpOutput[Count] = Entry.Value.ManScript.ManData;
        //    Count++;
        //}

        //using (StreamWriter Writer = new StreamWriter(fileName))
        //{ 
        //    Serializer.Serialize(Writer, tmpOutput);
        //}
    }

    public void LoadManList(string fileName)
    {
        //RemoveAllAvatars();

        //XmlSerializer Serializer = new XmlSerializer(typeof(ManInstanceData[]));
        //using (FileStream FileStream = new FileStream(fileName, FileMode.Open))
        //{
        //    ManInstanceData[] tmpInput = (ManInstanceData[])Serializer.Deserialize(FileStream);

        //    foreach (ManInstanceData ManData in tmpInput)
        //    {
        //        CreateMan(ManData); // Gets walking path to entrance automatically

        //        //if (ManData.AssignedRoom == Guid.Empty)
        //        if (ManData.AssignedRoom == null)
        //        {
        //            StateManager.Ref.SetWaitingMan(ManData.ManId);
        //        }
        //        else
        //        {
        //            //This local reference wasn't necessary
        //            //RoomScript RoomScript = RoomManager.Ref.GetRoomData(ManData.AssignedRoom).RoomScript;
        //            //SetManPathFromEntrance(ManData.ManId, ManData.AssignedRoom, ManData.AssignedRoomSlot);
        //            SetManPathFromEntrance(ManData.ManId, ManData.AssignedRoom.RoomData.RoomId, ManData.AssignedRoomSlot);
        //            //RoomManager.Ref.GetRoomData(ManData.AssignedRoom).RoomScript.AssignManToRoomSlot(ManData.ManId, ManData.AssignedRoomSlot);
        //            ManData.AssignedRoom.AssignManToRoomSlot(ManData.ManId, ManData.AssignedRoomSlot, true);
                    
        //        }
        //    }
        //}        
    }

    private GameObject InstantiateMan(Enums.ManTypes manType)
    {
        foreach (GameObject man in ManPrefabs)
        {
            if (man.GetComponent<ManScript>().ManType == manType)
                return Instantiate(man);
        }
        Debug.LogError("Man type of " + manType + " was not found in the ManPrefabs list on the ManManager.");
        return null;
    }

    public static bool FindOpenBedroomForMan(ManScript man)
    {
        //Get all bedroom rooms
        var ar = RoomManager.Ref.GetAllActiveRoomsofType<Room_Bedroom>();
        for (int i = 0; i < ar.Length; i++)
        {
            if (!ar[i].HasOwner())
            {
                if (ar[i].Cleanliness <= 0.8f)
                    continue;
                Instance.MoveManToNewRoom(man, ar[i]);
                Instance.TransferOwnershipToRoom(man, ar[i]);
                return true;
            }
        }

        GuiManager.Ref.Initiate_UserInfoSmall("New Guests are waiting on a bedroom!");
        return false;
    }
}
