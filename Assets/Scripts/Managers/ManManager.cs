// Handling all avatar related actions

using MySpace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class ManManager : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField] GameObject[] ManPrefabs;

    #endregion

    #region Variables
    [HideInInspector]
    public static ManManager Ref { get; private set; } // For external access of script

    private Dictionary<Guid, ManRef> _ManList = new Dictionary<Guid, ManRef>();

    [HideInInspector]
    public List<ManInstanceData> hireList = new List<ManInstanceData>();
    [HideInInspector]
    public List<ManInstanceData> bookingList = new List<ManInstanceData>();
    #endregion

    void Awake()
    {
        if (Ref == null) Ref = GetComponent<ManManager>();
    }

    public bool IsManExisting(Guid manId)
    {
        return (_ManList.ContainsKey(manId));
    }

    public void CreateMan(Guid manId, Enums.ManTypes manType)
    {
        ManInstanceData ManData = new ManInstanceData();
        ManData.ManId = manId;
        ManData.ManType = manType;
        ManData.ManFirstName = NameFactory.GetNewFirstName();
        ManData.ManLastName = NameFactory.GetNewLastName();

        CreateMan(ManData);
    }

    public void CreateMan(ManInstanceData manData)
    {
        if (manData == null) return;

        GameObject ManObject = InstantiateMan(manData.ManType);
        ManScript ManScript = ManObject.GetComponent<ManScript>();
        ManScript.ManData = manData;

        _ManList[manData.ManId] = new ManRef(ManObject, ManScript);
        ManObject.transform.position = Constants.NewManIncomingPath[0];
        AddIncomingPath(ManScript);
        GuiManager.Ref.UpdateManCount(_ManList.Count);
    }

    private void AddIncomingPath(ManScript manScript)
    {
        foreach (Vector3 WayPoint in Constants.NewManIncomingPath)
        {
            manScript.Add_RunAction_ToList(WayPoint);
        }
        
        manScript.Add_FacePlayerAction_ToList();
        manScript.Add_IdleAction_ToList();
    }

    private void AddOutgoingPath(ManScript manScript)
    {
        foreach (Vector3 WayPoint in Constants.NewManOutgoingPath)
        {
            manScript.Add_RunAction_ToList(WayPoint);
        }
    }

    public void RemoveManFromList(Guid manId)
    {
        Debug.Assert(IsManExisting(manId));
        _ManList[manId].ManObject = null;
        _ManList.Remove(manId);
        GuiManager.Ref.UpdateManCount(_ManList.Count);
    }

    public void MakeManLeave(Guid manId)
    {
        ManScript ManScript = _ManList[manId].ManScript;

        // Disable the raycast option
        ManScript.SetGhostState();

        // Give path to entrance (if assigned to any room. Otherwise, it is the one waiting at the entrance)
        if (ManScript.IsAssignedToAnyRoom())
        {
            RoomScript RoomScript = RoomManager.Ref.GetRoomData(ManScript.ManData.AssignedRoom).RoomScript;
            GridIndex[] Pathindizes = GridManager.Ref.GetIndexPath(RoomScript.RoomData.CoveredIndizes[ManScript.ManData.AssignedRoomSlot],
                                                                   Constants.EntranceRoomIndex);

            if (Pathindizes.Length > 1) AddIndexPathToManScript(Pathindizes, ManScript);
        }
        else
        {
            StateManager.Ref.SetWaitingMan(Guid.Empty);
        }

        // Give path to outside
        AddOutgoingPath(ManScript);

        // Submit self-destruction of object
        ManScript.Add_SelfDestruction_ToList();
    }

    public ManRef GetManData(Guid manId)
    {
        Debug.Assert(IsManExisting(manId));
        return (_ManList[manId]);
    }

    public void MoveManToNewRoom(Guid manId, Guid newRoomId)
    {
        if (RoomManager.Ref.IsRoomExisting(newRoomId) == false) return;
        if (IsManExisting(manId) == false) return;

        ManScript ManScript = _ManList[manId].ManScript;
        RoomScript NewRoomScript = RoomManager.Ref.GetRoomData(newRoomId).RoomScript;

        //NPC is Overworld only trying to travel to Underworld
        if (ManScript.ManData.ManType > 0 && NewRoomScript.RoomData.RoomOverUnder < 0)
        {
            GuiManager.Ref.Initiate_UserInfoSmall("Sorry, can't assign to this room!");
            return;
        }

        //NPC is Underworld only trying to travel to Overworld
        if (ManScript.ManData.ManType < 0 && NewRoomScript.RoomData.RoomOverUnder > 0)
        {
            GuiManager.Ref.Initiate_UserInfoSmall("Sorry, can't assign to this room!");
            return;
        }


        if (NewRoomScript.RoomContainsMan(manId))
        {
            GuiManager.Ref.Initiate_UserInfoSmall("Already assigned to this room!");
            return;
        }

        if ((ManScript.IsAssignedToAnyRoom() == false) && (NewRoomScript.RoomHasFreeManSlots() == false))
        {
            GuiManager.Ref.Initiate_UserInfoSmall("Sorry, can't assign to this room!");
            return;
        }

        if ((ManScript.IsAssignedToAnyRoom() == false) && (NewRoomScript.RoomHasFreeManSlots() == true))
        {
            int ManSlotIndex = NewRoomScript.GetFreeManSlotIndex();
            ManScript.AssignToRoom(newRoomId, ManSlotIndex);
            NewRoomScript.AssignManToRoomSlot(manId, ManSlotIndex);
            SetManPathFromEntrance(manId, newRoomId, ManSlotIndex);
            StateManager.Ref.SetWaitingMan(Guid.Empty);
            return;
        }

        Guid OldRoomGuid = ManScript.ManData.AssignedRoom;
        RoomScript OldRoomScript = RoomManager.Ref.GetRoomData(OldRoomGuid).RoomScript;

        if (NewRoomScript.RoomHasFreeManSlots() == true)
        {            
            int NewManSlotIndex = NewRoomScript.GetFreeManSlotIndex();
            int OldManSlotIndex = ManScript.ManData.AssignedRoomSlot;
            ManScript.AssignToRoom(newRoomId, NewManSlotIndex);
            OldRoomScript.RemoveManFromRoomSlot(manId);
            NewRoomScript.AssignManToRoomSlot(manId, NewManSlotIndex);
            SetManPath(manId, OldRoomGuid, OldManSlotIndex, newRoomId, NewManSlotIndex);
        }
        else
        {
            Guid OtherManGuid = NewRoomScript.RoomData.ManSlotsAssignments[0];
            ManScript OtherManScript = _ManList[OtherManGuid].ManScript;
            OldRoomScript.RemoveManFromRoomSlot(manId);
            NewRoomScript.RemoveManFromRoomSlot(OtherManGuid);

            int NewManSlotIndex1 = NewRoomScript.GetFreeManSlotIndex();
            int OldManSlotIndex1 = ManScript.ManData.AssignedRoomSlot;
            ManScript.AssignToRoom(newRoomId, NewManSlotIndex1);
            NewRoomScript.AssignManToRoomSlot(manId, NewManSlotIndex1);
            SetManPath(manId, OldRoomGuid, OldManSlotIndex1, newRoomId, NewManSlotIndex1);

            int NewManSlotIndex2 = OldRoomScript.GetFreeManSlotIndex();
            int OldManSlotIndex2 = OtherManScript.ManData.AssignedRoomSlot;
            OtherManScript.AssignToRoom(OldRoomGuid, NewManSlotIndex2);
            OldRoomScript.AssignManToRoomSlot(OtherManGuid, NewManSlotIndex2);
            SetManPath(OtherManGuid, newRoomId, OldManSlotIndex2, OldRoomGuid, NewManSlotIndex2);
        }
    }

    public void RemoveManFromRoom(Guid manId)
    {
        ManScript ManScript = _ManList[manId].ManScript;
        if (RoomManager.Ref.IsRoomExisting(ManScript.ManData.AssignedRoom) == false) return;

        RoomScript RoomScript = RoomManager.Ref.GetRoomData(ManScript.ManData.AssignedRoom).RoomScript;
        RoomScript.RemoveManFromRoomSlot(manId);        
        ManScript.AssignToRoom(Guid.Empty, 0);
    }

    public void RemoveManOwnershipFromRoom(Guid manId)
    {
        ManScript ManScript = _ManList[manId].ManScript;
        if (RoomManager.Ref.IsRoomExisting(ManScript.ManData.OwnedRoomRef.RoomId) == false) return;

        RoomScript RoomScript = RoomManager.Ref.GetRoomData(ManScript.ManData.OwnedRoomRef.RoomId).RoomScript;
        RoomScript.RemoveOwnerFromRoomSlot(manId);
        ManScript.SetOwnerOfRoom(Guid.Empty);
    }

    public void TransferOwnershipToRoom(Guid manId, Guid roomId)
    {
        ManScript ManScript = _ManList[manId].ManScript;
        ManScript.TransferOwnershipToNewRoom(roomId);
    }

    private void SetManPathFromEntrance(Guid manId, Guid newRoomGuid, int newSlotIndex)
    {
        ManScript ManScript = _ManList[manId].ManScript;

        Vector3 PathPosition = GridManager.Ref.GetWorldPositionFromGridIndexZOffset(Constants.EntranceRoomIndex, Constants.GridPositionWalkZOffset);
        ManScript.Add_RunAction_ToList(PathPosition);

        Guid OldRoomGuid = GridManager.Ref.GetGridTileRoomGuid(Constants.EntranceRoomIndex);
        SetManPath(manId, OldRoomGuid, 0, newRoomGuid, newSlotIndex);
    }

    private void SetManPath(Guid manId, Guid oldRoomGuid, int oldSlotIndex, Guid newRoomGuid, int newSlotIndex)
    {
        ManScript ManScript = _ManList[manId].ManScript;
        RoomScript OldRoomScript = RoomManager.Ref.GetRoomData(oldRoomGuid).RoomScript;
        RoomScript NewRoomScript = RoomManager.Ref.GetRoomData(newRoomGuid).RoomScript;

        GridIndex[] Pathindizes  = GridManager.Ref.GetIndexPath(OldRoomScript.RoomData.CoveredIndizes[oldSlotIndex],
                                                                NewRoomScript.RoomData.CoveredIndizes[newSlotIndex]);

        if (Pathindizes.Length > 1) AddIndexPathToManScript(Pathindizes, ManScript);

        
        ManScript.Add_RunAction_ToList(NewRoomScript.RoomData.ManSlotsPositions[newSlotIndex]);
        ManScript.Add_RotateAction_ToList(NewRoomScript.RoomData.ManSlotsRotations[newSlotIndex]);
        ManScript.Add_WorkingAction_ToList(NewRoomScript.RoomData.ManWorkingStates[newSlotIndex]);
    }

    public void AddIndexPathToManScript(GridIndex[] pathIndizes, ManScript manScript)
    {
        if (pathIndizes == null) return;
        if (manScript == null) return;

        Vector3 WorldPos;

        for (int i = 0; i < (pathIndizes.Length - 1); i++)
        {
            WorldPos = GridManager.Ref.GetWorldPositionFromGridIndexZOffset(pathIndizes[i], Constants.GridPositionWalkZOffset);
            manScript.Add_RunAction_ToList(WorldPos);

            if (pathIndizes[i].Z != pathIndizes[i + 1].Z) // Going to pass elevator door
            {
                Guid RoomID = GridManager.Ref.GetGridTileRoomGuid(pathIndizes[i]);
                if (RoomManager.Ref.GetRoomData(RoomID).RoomScript.RoomData.RoomType == Enums.RoomTypes.Elevator)
                    manScript.Add_DoorOpenAction_ToList(RoomID);
                WorldPos = GridManager.Ref.GetWorldPositionFromGridIndexZOffset(pathIndizes[i + 1], Constants.GridPositionWalkZOffset);
                manScript.Add_RunAction_ToList(WorldPos);
                if (RoomManager.Ref.GetRoomData(RoomID).RoomScript.RoomData.RoomType == Enums.RoomTypes.Elevator)
                    manScript.Add_DoorCloseAction_ToList(RoomID);
                i++;
            }
        }

        WorldPos = GridManager.Ref.GetWorldPositionFromGridIndexZOffset(pathIndizes[pathIndizes.Length - 1], Constants.GridPositionWalkZOffset);
        manScript.Add_RunAction_ToList(WorldPos);
    }

    public void SetHighlightedMan(Guid manId)
    {
        Debug.Assert(IsManExisting(manId));
        _ManList[manId].ManScript.SetSelectedState(true);
    }

    public void ResetHighlightedMan(Guid manId)
    {
        if(!IsManExisting(manId)) return;
        _ManList[manId].ManScript.SetSelectedState(false);
    }

    public void RemoveAllAvatars()
    {
        foreach (KeyValuePair<Guid, ManRef> Entry in _ManList)
        {
            Destroy(Entry.Value.ManObject);
            Entry.Value.ManObject = null;
        }

        _ManList.Clear();
    }

    public int GetManCount()
    {
        return (_ManList.Count);
    }

    public void SaveManList(string fileName)
    {
        XmlSerializer Serializer = new XmlSerializer(typeof(ManInstanceData[]));

        ManInstanceData[] tmpOutput = new ManInstanceData[_ManList.Count];
        
        // Saving as an array. Key GUID is also in the data
        int Count = 0;
        foreach (KeyValuePair<Guid, ManRef> Entry in _ManList)
        {
            tmpOutput[Count] = Entry.Value.ManScript.ManData;
            Count++;
        }

        using (StreamWriter Writer = new StreamWriter(fileName))
        { 
            Serializer.Serialize(Writer, tmpOutput);
        }
    }

    public void LoadManList(string fileName)
    {
        RemoveAllAvatars();

        XmlSerializer Serializer = new XmlSerializer(typeof(ManInstanceData[]));
        using (FileStream FileStream = new FileStream(fileName, FileMode.Open))
        {
            ManInstanceData[] tmpInput = (ManInstanceData[])Serializer.Deserialize(FileStream);

            foreach (ManInstanceData ManData in tmpInput)
            {
                CreateMan(ManData); // Gets walking path to entrance automatically

                if (ManData.AssignedRoom == Guid.Empty)
                {
                    StateManager.Ref.SetWaitingMan(ManData.ManId);
                }
                else
                {
                    RoomScript RoomScript = RoomManager.Ref.GetRoomData(ManData.AssignedRoom).RoomScript;
                    RoomScript.AssignManToRoomSlot(ManData.ManId, ManData.AssignedRoomSlot);
                    SetManPathFromEntrance(ManData.ManId, ManData.AssignedRoom, ManData.AssignedRoomSlot);
                }
            }
        }        
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
}
