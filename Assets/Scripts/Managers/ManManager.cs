﻿// Handling all avatar related actions

using MySpace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using System.Linq;

public class ManManager : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField] GameObject[] ManPrefabs;

    #endregion

    #region Variables
    [HideInInspector]
    public static ManManager Ref { get; private set; } // For external access of script

    private Dictionary<Guid, ManRef<ManScript>> _ManList = new Dictionary<Guid, ManRef<ManScript>>();

    #region Helper with dictionaries
    //Here for reference on foreach loops through dictionaries
    void Test()
    {
        int i = 0;
        int random1 = 0, random2 = 0, random3 = 0;
        int length = _ManList.Keys.Count;
        foreach (Guid key in _ManList.Keys)
        {
            if (i == random1)
            {
                //Index the dictionary using the key
                //_ManList[key].ManScript
            }
            i++;
        }
    }
    #endregion

    [HideInInspector]
    public List<WorkerConstructionData> hireList = new List<WorkerConstructionData>();
    [HideInInspector]
    public List<GuestConstructionData> bookingList = new List<GuestConstructionData>();

    public float ManRevenueCalculation
    {
        get
        {
            float t = 0;
            foreach (ManRef<ManScript> man in _ManList.Values)
            {
                t += man.ManScript.GetNetRevenueCalculation;
            }
            return t;
        }
    }

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

    public void CreateWorker(WorkerConstructionData data)
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


        _ManList[data.manId] = new ManRef<ManScript>(script.gameObject, script);
        //script.gameObject.transform.position = GridManager.Ref.GetWorldPositionFromGridIndexZOffset(Constants.NewManIncomingPath[0], Constants.GridPositionWalkZOffset);
        //AddIncomingPath(script);
        GuiManager.Ref.UpdateManCount(_ManList.Count);
    }

    public void CreateGuest(GuestConstructionData data)
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

        _ManList[data.manId] = new ManRef<ManScript>(script.gameObject, script);
        //script.gameObject.transform.position = GridManager.Ref.GetWorldPositionFromGridIndexZOffset(Constants.NewManIncomingPath[0], Constants.GridPositionWalkZOffset);
        //AddIncomingPath(script);
        GuiManager.Ref.UpdateManCount(_ManList.Count);
    }

    public void CreateMan(ManInstanceData manData)
    {
        if (manData == null) return;

        GameObject ManObject = InstantiateMan(manData.ManType);
        ManScript ManScript = ManObject.GetComponent<ManScript>();
        ManScript.ManData = manData;

        _ManList[manData.ManId] = new ManRef<ManScript>(ManObject, ManScript);
        //ManObject.transform.position = GridManager.Ref.GetWorldPositionFromGridIndexZOffset(Constants.NewManIncomingPath[0], Constants.GridPositionWalkZOffset);
        //AddIncomingPath(ManScript);
        GuiManager.Ref.UpdateManCount(_ManList.Count);
    }

    //private void AddIncomingPath(ManScript manScript)
    //{
    //    //Need to loop through the list since it is readonly it cannot be fed into a reference variable function parameter.
    //    foreach (GridIndex WayPoint in Constants.NewManIncomingPath)
    //    {
    //        manScript.AddMovementAction(WayPoint);
    //    }
    //}

    //private void AddOutgoingPath(ManScript manScript)
    //{
    //    //Need to loop through the list since it is readonly it cannot be fed into a reference variable function parameter.
    //    foreach (GridIndex WayPoint in Constants.ManOutgoingPath)
    //    {
    //        manScript.AddMovementAction(WayPoint);
    //    }
    //}

    public void RemoveManFromList(Guid manId)
    {
        Debug.Assert(IsManExisting(manId));
        //_ManList[manId].ManObject = null;
        _ManList.Remove(manId);
        GuiManager.Ref.UpdateManCount(_ManList.Count);
    }

    public void MakeManLeave(Guid manId)
    {
        ManScript manScript = _ManList[manId].ManScript;

        // Disable the raycast option
        //manScript.SetGhostState();

        // Give path to entrance (if assigned to any room. Otherwise, it is the one waiting at the entrance)
        if (manScript.IsAssignedToAnyRoom())
        {
            if (GameManager.Debug)
                Debug.Log("Guest had room and is being removed from it");
            //RoomScript RoomScript = RoomManager.Ref.GetRoomData(ManScript.ManData.AssignedRoom).RoomScript;
            RoomScript RoomScript = manScript.ManData.AssignedRoom;

            //Find the path from their current location to the entrance point of the hootel.
            GridIndex[] Pathindizes = GridManager.Ref.GetIndexPath(RoomScript.RoomData.CoveredIndizes[manScript.ManData.AssignedRoomSlot],
                                                                   Constants.EntranceRoomIndex);

            if (Pathindizes.Length > 0) AddIndexPathToManScript(Pathindizes, manScript);

            RoomScript.RemoveManFromRoomSlot(manId);
        }
        else
        {
            StateManager.Ref.SetWaitingMan(Guid.Empty);
        }

        //Give path to outside from the entrance index
        //AddOutgoingPath(manScript); //No longer needed, the beginannihilation function does the movement as well.

        //Submit self-destruction of object once the character reaches the end point of the path.
        manScript.AddActionToEndOfMovement(manScript.BeginAnnihilation);
        
    }

    public ManRef<T>[] GetAllActiveMenOfType<T>() where T : ManScript
    {
                                                    //This might just return true every time?
        return (from ManRef<T> man in _ManList.Values where man.ManScript is T select man).ToArray();


        //int c = 0;
        //foreach (ManRef<ManScript> man in _ManList.Values)
        //{
        //    if (man.ManScript is T)
        //    {
        //        c++;
        //    }
        //}
        //ManRef<T>[] ar = new ManRef<T>[c];
        //c = 0;
        //foreach (ManRef<ManScript> man in _ManList.Values)
        //{
        //    if (man.ManScript is T)
        //    {
        //        ar[c] = new ManRef<T>(man.ManObject, man.ManScript as T);
        //        c++;
        //    }
        //}
        //return ar;
    }

    public ManRef<ManScript>[] GetAllActiveMenOfType(Enums.ManTypes type)
    {
        return (from ManRef<ManScript> man in _ManList.Values where man.ManScript.ManType == type select man).ToArray();

        //int c = 0;
        //foreach (ManRef<ManScript> man in _ManList.Values)
        //{
        //    if (man.ManScript.ManType == type)
        //    {
        //        c++;
        //    }
        //}
        //ManRef<ManScript>[] ar = new ManRef<ManScript>[c];
        //c = 0;
        //foreach (ManRef<ManScript> man in _ManList.Values)
        //{
        //    if (man.ManScript.ManType == type)
        //    {
        //        ar[c] = man;
        //        c++;
        //    }
        //}
        //return ar;
    }

    public ManRef<ManScript> GetManData(Guid manId)
    {
        Debug.Assert(IsManExisting(manId));
        return _ManList[manId];
    }

    public ManRef<T> GetManData<T>(Guid manId) where T : ManScript
    {
        Debug.Assert(IsManExisting(manId));
        if (!(_ManList[manId].ManScript is T))
        {
            Debug.LogWarning("Given manId " + manId + ", was not the given type: " + typeof(T) + "!");
            return new ManRef<T>();
        }
            
        return new ManRef<T>(_ManList[manId].ManObject, (T)_ManList[manId].ManScript);
    }

    public bool IsManTypeOf<T>(Guid manId) where T : ManScript
    {
        Debug.Assert(IsManExisting(manId));
        return _ManList[manId].ManScript is T;
    }

    public void MoveManToClosestRoomOfType<T>(ManScript man) where T : Room_CleanerCloset
    {
        Guid chosen = Guid.Empty;
        int shortest = int.MaxValue;

        foreach (T room in (from T r in RoomManager.Ref.GetAllActiveRoomsofType<T>() where r.RoomHasFreeManSlots(man.ManData.ManId) select r))
        {
            var path = GridManager.Ref.GetIndexPath(man.ManData.AssignedRoom.RoomData.CoveredIndizes[man.ManData.AssignedRoomSlot], room.RoomData.CoveredIndizes[room.CountMen()]);
            if (path.Length < shortest)
            {
                chosen = room.RoomData.RoomId;
                shortest = path.Length;
            }
        }
        if (chosen != Guid.Empty)
            MoveManToNewRoom(man.ManData.ManId, chosen);
    }

    public void MoveManToClosestRoomOfType<T>(Guid manId) where T : Room_CleanerCloset
    {
        Guid chosen = Guid.Empty;
        int shortest = int.MaxValue;

        foreach (T room in (from T r in RoomManager.Ref.GetAllActiveRoomsofType<T>() where r.RoomHasFreeManSlots(manId) select r))
        {
            var path = GridManager.Ref.GetIndexPath(GetManData(manId).ManScript.ManData.AssignedRoom.RoomData.CoveredIndizes[GetManData(manId).ManScript.ManData.AssignedRoomSlot], room.RoomData.CoveredIndizes[room.CountMen()]);
            if (path.Length < shortest)
            {
                chosen = room.RoomData.RoomId;
                shortest = path.Length;
            }
        }
        if (chosen != Guid.Empty)
            MoveManToNewRoom(manId, chosen);
    }

    public void MoveManToNewRoom(Guid manId, Guid newRoomId, bool fromPlayer = false)
    {
        if (RoomManager.Ref.IsRoomExisting(newRoomId) == false) return;
        if (IsManExisting(manId) == false) return;

        ManScript ManScript = _ManList[manId].ManScript;
        RoomScript NewRoomScript = RoomManager.Ref.GetRoomData(newRoomId).RoomScript;

        //NPC is Overworld only trying to travel to Underworld
        if (ManScript.ManData.ManType > 0 && NewRoomScript.RoomData.RoomOverUnder < 0)
        {
            goto CannotAssign;
        }

        //NPC is Underworld only trying to travel to Overworld
        if (ManScript.ManData.ManType < 0 && NewRoomScript.RoomData.RoomOverUnder > 0)
        {
            goto CannotAssign;
        }


        if (NewRoomScript.RoomContainsMan(manId))
        {
            GuiManager.Ref.Initiate_UserInfoSmall("Already assigned to this room!");
            return;
        }

        if ((ManScript.IsAssignedToAnyRoom() == false) && (NewRoomScript.RoomHasFreeManSlots(manId) == false))
        {
            goto CannotAssign;
        }

        if (fromPlayer && !RoomManager.IsRoomOfType<Room_WorkQuarters>(NewRoomScript) && ManScript.ManType == Enums.ManTypes.Worker)
        {
            goto CannotAssign;
        }

        if ((ManScript.IsAssignedToAnyRoom() == false) && (NewRoomScript.RoomHasFreeManSlots(manId) == true))
        {
            int ManSlotIndex = NewRoomScript.GetFreeManSlotIndex(ManScript);
            SetManPathFromEntrance(manId, newRoomId, ManSlotIndex);
            //ManScript.AssignToRoom(newRoomId, ManSlotIndex);
            //NewRoomScript.AssignManToRoomSlot(manId, ManSlotIndex, fromPlayer);
            NewRoomScript.ReserveSlotForMan(manId, ManSlotIndex, fromPlayer);
            ManScript.AddActionToEndOfMovement(ManScript.AssignToCurrentRoomReservation);
            //ManScript.rum = NewRoomScript;
            
            StateManager.Ref.SetWaitingMan(Guid.Empty);
            return;
        }

        //Guid OldRoomGuid = ManScript.ManData.AssignedRoom;
        Guid OldRoomGuid = ManScript.ManData.AssignedRoom.RoomData.RoomId;
        RoomScript OldRoomScript = RoomManager.Ref.GetRoomData(OldRoomGuid).RoomScript;

        if (NewRoomScript.RoomHasFreeManSlots(manId) == true)
        {
            int NewManSlotIndex = NewRoomScript.GetFreeManSlotIndex(ManScript);
            int OldManSlotIndex = ManScript.ManData.AssignedRoomSlot;
            SetManPath(manId, OldRoomGuid, OldManSlotIndex, newRoomId, NewManSlotIndex);
            //ManScript.AssignToRoom(newRoomId, NewManSlotIndex);
            OldRoomScript.RemoveManFromRoomSlot(manId);
            //NewRoomScript.AssignManToRoomSlot(manId, NewManSlotIndex, fromPlayer);
            NewRoomScript.ReserveSlotForMan(manId, NewManSlotIndex, fromPlayer);
            ManScript.AddActionToEndOfMovement(ManScript.AssignToCurrentRoomReservation);
            //ManScript.rum = NewRoomScript;
        }
        else if (ManScript.ManData.ManType == Enums.ManTypes.Worker)//make sure this doesnt happen to guests
        {
            Guid OtherManGuid = NewRoomScript.RoomData.ManSlotsAssignments[0];
            ManScript OtherManScript = _ManList[OtherManGuid].ManScript;
            OldRoomScript.RemoveManFromRoomSlot(manId);
            NewRoomScript.RemoveManFromRoomSlot(OtherManGuid);

            int NewManSlotIndex1 = NewRoomScript.GetFreeManSlotIndex(ManScript);
            int OldManSlotIndex1 = ManScript.ManData.AssignedRoomSlot;
            SetManPath(manId, OldRoomGuid, OldManSlotIndex1, newRoomId, NewManSlotIndex1);
            //ManScript.AssignToRoom(newRoomId, NewManSlotIndex1);
            //NewRoomScript.AssignManToRoomSlot(manId, NewManSlotIndex1, fromPlayer);
            NewRoomScript.ReserveSlotForMan(manId, NewManSlotIndex1, fromPlayer);
            ManScript.AddActionToEndOfMovement(ManScript.AssignToCurrentRoomReservation);
            //ManScript.rum = NewRoomScript;

            int NewManSlotIndex2 = OldRoomScript.GetFreeManSlotIndex(OtherManScript);
            int OldManSlotIndex2 = OtherManScript.ManData.AssignedRoomSlot;
            SetManPath(OtherManGuid, newRoomId, OldManSlotIndex2, OldRoomGuid, NewManSlotIndex2);
            //OtherManScript.AssignToRoom(OldRoomGuid, NewManSlotIndex2);
            //OldRoomScript.AssignManToRoomSlot(OtherManGuid, NewManSlotIndex2, fromPlayer);
            OldRoomScript.ReserveSlotForMan(OtherManGuid, NewManSlotIndex2, fromPlayer);
            OtherManScript.AddActionToEndOfMovement(OtherManScript.AssignToCurrentRoomReservation);
            //OtherManScript.rum = OldRoomScript;
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

    public void RemoveManFromRoom(Guid manId)
    {
        ManScript ManScript = _ManList[manId].ManScript;
        //if (RoomManager.Ref.IsRoomExisting(ManScript.ManData.AssignedRoom) == false) return;

        if (ManScript.ManData.AssignedRoom == null) return;
        if (RoomManager.Ref.IsRoomExisting(ManScript.ManData.AssignedRoom.RoomData.RoomId) == false) return;

        //RoomScript RoomScript = RoomManager.Ref.GetRoomData(ManScript.ManData.AssignedRoom).RoomScript;
        RoomScript RoomScript = ManScript.ManData.AssignedRoom;
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

    public void TransferOwnershipToRoom(ManScript ManScript, Guid roomId)
    {
        ManScript.TransferOwnershipToNewRoom(roomId);
    }

    private void SetManPathFromEntrance(Guid manId, Guid newRoomGuid, int newSlotIndex)
    {
        ManScript ManScript = _ManList[manId].ManScript;

        //Vector3 PathPosition = GridManager.Ref.GetWorldPositionFromGridIndexZOffset(Constants.EntranceRoomIndex, Constants.GridPositionWalkZOffset);
        //Use the man's object position instead of the "start index" position
        GridIndex index = GridManager.Ref.GetXYGridIndexFromWorldPosition(GetManData(manId).ManObject.transform.position);
        ManScript.AddMovementAction(index);

        Guid OldRoomGuid = GridManager.Ref.GetGridTileRoomGuid(Constants.EntranceRoomIndex);
        if (OldRoomGuid == newRoomGuid)
        {
            GridIndex[] Pathindizes = GridManager.Ref.GetIndexPath(index, RoomManager.Ref.GetRoomData(newRoomGuid).RoomScript.RoomData.CoveredIndizes[newSlotIndex]);
            if (Pathindizes.Length > 0) AddIndexPathToManScript(Pathindizes, ManScript);
        }
        else
            SetManPath(manId, OldRoomGuid, 0, newRoomGuid, newSlotIndex);
    }

    private void SetManPath(Guid manId, Guid oldRoomGuid, int oldSlotIndex, Guid newRoomGuid, int newSlotIndex)
    {
        ManScript ManScript = _ManList[manId].ManScript;
        RoomScript OldRoomScript = RoomManager.Ref.GetRoomData(oldRoomGuid).RoomScript;
        RoomScript NewRoomScript = RoomManager.Ref.GetRoomData(newRoomGuid).RoomScript;

        GridIndex[] Pathindizes = GridManager.Ref.GetIndexPath(OldRoomScript.RoomData.CoveredIndizes[oldSlotIndex],
                                                                NewRoomScript.RoomData.CoveredIndizes[newSlotIndex]);

        if (Pathindizes.Length > 0) AddIndexPathToManScript(Pathindizes, ManScript);

        
        //ManScript.Add_RunAction_ToList(NewRoomScript.RoomData.ManSlotsPositions[newSlotIndex]);
        //ManScript.Add_RotateAction_ToList(NewRoomScript.RoomData.ManSlotsRotations[newSlotIndex]);
        //ManScript.Add_WorkingAction_ToList(NewRoomScript.RoomData.ManWorkingStates[newSlotIndex]);
    }

    public void AddIndexPathToManScript(GridIndex[] pathIndizes, ManScript manScript)
    {
        if (pathIndizes == null) return;
        if (manScript == null) return;

        manScript.AddMovementActions(pathIndizes);
        //Vector3 WorldPos;
        //Guid prevRoom = Guid.Empty;

        //for (int i = 0; i < pathIndizes.Length; i++)
        //{
        //    WorldPos = GridManager.Ref.GetWorldPositionFromGridIndexZOffset(pathIndizes[i], Constants.GridPositionWalkZOffset);
        //    Guid RoomID = GridManager.Ref.GetGridTileRoomGuid(pathIndizes[i]);

        //    if (i > 0 && pathIndizes[i - 1].Y != pathIndizes[i].Y) // Going up an elevator
        //    {
        //        if (RoomManager.Ref.GetRoomData(RoomID).RoomScript.RoomData.RoomType == Enums.RoomTypes.Elevator)
        //        {
        //            manScript.Add_ElevatorMovementAction_ToList(RoomManager.Ref.GetRoomData(RoomID).RoomScript as Room_Elevator, pathIndizes[i].Y, true);

        //            //Dont need the movement action since the elevator will move the character with the elevator box
        //            continue;
        //        }
        //    }

        //    if (i > 0 && pathIndizes[i - 1].Z != pathIndizes[i].Z && (RoomManager.Ref.GetRoomData(RoomID).RoomScript.RoomData.RoomType == Enums.RoomTypes.Elevator || RoomManager.Ref.GetRoomData(prevRoom).RoomScript.RoomData.RoomType == Enums.RoomTypes.Elevator)) // Going to pass elevator door
        //    {
        //        if (RoomManager.Ref.GetRoomData(RoomID).RoomScript.RoomData.RoomType == Enums.RoomTypes.Elevator)
        //        {
        //            manScript.Add_AccessAction_ToList(RoomID);
        //            manScript.Add_ElevatorMovementAction_ToList(RoomManager.Ref.GetRoomData(RoomID).RoomScript as Room_Elevator, pathIndizes[i].Y, false);
        //            manScript.Add_DoorOpenAction_ToList(RoomID);
        //        }
        //        else
        //            manScript.Add_AccessAction_ToList(RoomID);

        //        WorldPos = GridManager.Ref.GetWorldPositionFromGridIndexZOffset(pathIndizes[i], Constants.GridPositionWalkZOffset);
        //        manScript.Add_RunAction_ToList(WorldPos);
        //        if (RoomManager.Ref.GetRoomData(RoomID).RoomScript.RoomData.RoomType == Enums.RoomTypes.Elevator)
        //            manScript.Add_DoorCloseAction_ToList(RoomID);
        //        if (prevRoom != RoomID)
        //        {
        //            prevRoom = RoomID;
        //        }
        //        continue;
        //    }
        //    else if (prevRoom != RoomID)
        //    {
        //        prevRoom = RoomID;
        //        if (RoomManager.Ref.GetRoomData(RoomID).RoomScript.RoomData.RoomType != Enums.RoomTypes.Elevator)
        //            manScript.Add_AccessAction_ToList(RoomID);
        //    }
        //    manScript.Add_RunAction_ToList(WorldPos);
        //}
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
        //foreach (KeyValuePair<Guid, ManRef<ManScript>> Entry in _ManList)
        //{
        //    Destroy(Entry.Value.ManObject);
        //    //Entry.Value.ManObject = null;
        //}

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
        foreach (KeyValuePair<Guid, ManRef<ManScript>> Entry in _ManList)
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

                //if (ManData.AssignedRoom == Guid.Empty)
                if (ManData.AssignedRoom == null)
                {
                    StateManager.Ref.SetWaitingMan(ManData.ManId);
                }
                else
                {
                    //This local reference wasn't necessary
                    //RoomScript RoomScript = RoomManager.Ref.GetRoomData(ManData.AssignedRoom).RoomScript;
                    //SetManPathFromEntrance(ManData.ManId, ManData.AssignedRoom, ManData.AssignedRoomSlot);
                    SetManPathFromEntrance(ManData.ManId, ManData.AssignedRoom.RoomData.RoomId, ManData.AssignedRoomSlot);
                    //RoomManager.Ref.GetRoomData(ManData.AssignedRoom).RoomScript.AssignManToRoomSlot(ManData.ManId, ManData.AssignedRoomSlot);
                    ManData.AssignedRoom.AssignManToRoomSlot(ManData.ManId, ManData.AssignedRoomSlot, true);
                    
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
                Ref.MoveManToNewRoom(man.ManData.ManId, ar[i].RoomData.RoomId);
                Ref.TransferOwnershipToRoom(man.ManData.ManId, ar[i].RoomData.RoomId);
                return true;
            }
        }

        GuiManager.Ref.Initiate_UserInfoSmall("New Guests are waiting on a bedroom!");
        return false;
    }
}
