﻿// Managing the rooms' list.

using MySpace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using System.Linq;

public class RoomManager : MonoBehaviour
{
    #region Serialized Variables
    [Tooltip("A ratio applied to all calculations related to cleaning speed")]
    [Range(0.001f, 100)]
    [SerializeField] float cleanSpeedRatio = 0.2f;
    [Tooltip("A ratio applied to all calculations related to room dirtiness speed")]
    [Range(0.001f, 100)]
    [SerializeField] float dirtinessSpeedRatio = 0.1f;
    /// <summary>
    /// A ratio applied to all calculations related to cleaning speed
    /// </summary>
    public float CleanSpeedRatio { get { return cleanSpeedRatio; } }

    /// <summary>
    /// A ratio applied to all calculations related to room dirtiness speed
    /// </summary>
    public float DirtinessSpeedRatio { get { return dirtinessSpeedRatio; } }

    [Header("===============================================================================================================================================")]
    [Header("Room Highlights & Selectors")]
    public GameObject RoomSelectorSz1; // To be set by editor
    public GameObject RoomSelectorSz2;
    public GameObject RoomSelectorSz4;
    public GameObject RoomSelectorSz6;
    public GameObject RoomHighlighterSz2;
    public GameObject RoomHighlighterSz4;
    public GameObject RoomHighlighterSz6;
    public GameObject RoomHighlighter_Bedroom;
    [Header("===============================================================================================================================================")]
    [SerializeField] GameObject[] roomDefinitions;

    public static RoomDefData[] RoomDefinitions { get; private set; }

    public static Dictionary<Enums.RoomTypes, Action> UnlockEvents = new Dictionary<Enums.RoomTypes, Action>();
    #endregion

    #region Static Variables

    public static RoomManager Ref { get; private set; } // For external access of script

    public static Guid lobbyId, underLobbyId;

    #endregion

    //private float elapsedTime = 0;
    private Dictionary<Guid, RoomRef> _RoomList = new Dictionary<Guid, RoomRef>();

    #region Monobehavior Methods

    void Awake()
    {
        if (Ref == null) Ref = GetComponent<RoomManager>();

        #region Defining RoomDefinitions Array
        if (roomDefinitions.Length == 0)
            Debug.LogError("No Rooms filled in inspector for RoomManager!");
        RoomDefinitions = new RoomDefData[roomDefinitions.Length];
        for (int i = 0; i < roomDefinitions.Length; i++)
        {
            RoomScript room = roomDefinitions[i].GetComponent<RoomScript>();
            RoomDefinitions[i] = new RoomDefData(
                room.RoomStats.RoomName,
                roomDefinitions[i],
                room.RoomStats.RoomSize,
                room.RoomStats.RoomType,
                room.RoomStats.RoomCategory,
                room.RoomStats.ManSlotCount,
                CreateNewArray(room.RoomStats.ManSlotCount),
                room.RoomStats.RoomDescription,
                room.RoomStats.RoomCost,
                room.RoomStats.RoomOverUnder,
                room.RoomStats.locked,
                room.RoomStats.roomSprite
                );
        }

        Enums.ManStates[] CreateNewArray(int length)
        {
            var ar = new Enums.ManStates[length];
            for (int i = 0; i < length; i++)
            {
                ar[i] = Enums.ManStates.Idle;
            }
            return ar;
        }
        #endregion
    }

    void Start()
    {
        #region Checking inspector object fields are properly filled
        Debug.Assert(RoomSelectorSz1 != null);
        Debug.Assert(RoomSelectorSz2 != null);
        Debug.Assert(RoomSelectorSz4 != null);
        Debug.Assert(RoomSelectorSz6 != null);
        Debug.Assert(RoomHighlighterSz2 != null);
        Debug.Assert(RoomHighlighterSz4 != null);
        Debug.Assert(RoomHighlighterSz6 != null);
        Debug.Assert(RoomHighlighter_Bedroom != null);
        #endregion
    }

    #endregion

    #region CreateRoom Methods

    public void CreateRoom(Guid roomId, Enums.RoomTypes roomType, GridIndex leftMostIndex)
    {
        RoomDefData RoomDefData = GetRoomDefData(roomType);

        RoomInstanceData RoomData = new RoomInstanceData();
        RoomData.RoomId = roomId;
        RoomData.RoomName = RoomDefData.RoomName;
        RoomData.RoomSize = RoomDefData.RoomSize;
        RoomData.RoomCategory = RoomDefData.RoomCategory;
        RoomData.RoomType = RoomDefData.RoomType;
        RoomData.RoomOverUnder = RoomDefData.RoomOverUnder;
        RoomData.ManSlotCount = RoomDefData.ManSlotCount;
        RoomData.ManSlotsPositions = new Vector3[RoomDefData.ManSlotCount]; // Data will be set by object script on Start()
        RoomData.ManSlotsRotations = new Quaternion[RoomDefData.ManSlotCount]; // Data will be set by object script on Start()
        RoomData.ManSlotsAssignments = new Guid[RoomDefData.ManSlotCount];
        RoomData.OwnerSlotsAssignments = new Guid[RoomDefData.ManSlotCount];
        RoomData.ReservedSlots = new Container<Guid, bool>[RoomDefData.ManSlotCount];
        RoomData.RoomDescription = RoomDefData.RoomDescription;

        for (int i = 0; i < RoomData.ManSlotCount; i++) RoomData.ManSlotsAssignments[i] = Guid.Empty;
        for (int i = 0; i < RoomData.ManSlotCount; i++) RoomData.ReservedSlots[i] = new Container<Guid, bool>() { object1 = Guid.Empty, object2 = false };
        RoomData.ManWorkingStates = RoomDefData.ManWorkingStates;
        RoomData.CoveredIndizes = GridManager.Ref.GetOccupiedindizes(RoomDefData.RoomSize, leftMostIndex);

        for (int i = 0; i < RoomData.ManSlotCount; i++) RoomData.OwnerSlotsAssignments[i] = Guid.Empty;

        CreateRoom(RoomData);
    }

    public void CreateRoom(RoomInstanceData roomData)
    {
        if (roomData == null) return;

        GameObject RoomObject = InstantiateRoom(roomData.RoomType);
        RoomScript RoomScript = RoomObject.GetComponent<RoomScript>();
        RoomScript.RoomData = roomData;
        roomData.RoomScript = RoomScript;

        _RoomList[roomData.RoomId] = new RoomRef(RoomObject, RoomScript);
        RoomObject.transform.position = GridManager.Ref.GetWorldPositionFromGridIndex(roomData.GetLeftMostIndex());
        GridManager.Ref.RegisterAtGrid(roomData.RoomSize, roomData.RoomId, roomData.GetLeftMostIndex());

        //Let the room know that it has initialized
        roomData.RoomScript.OnInitialization();
    }
    #endregion

    #region HelperFunction Library
    public void AddRoom(Guid roomId, RoomRef room)
    {
        if (!_RoomList.ContainsKey(roomId))
            _RoomList.Add(roomId, room);
    }

    public bool IsRoomExisting(Guid roomId)
    {
        return (_RoomList.ContainsKey(roomId));
    }

    public void RemoveRoom(Guid roomId)
    {
        RoomScript RoomScript = _RoomList[roomId].RoomScript;
        GridManager.Ref.DeregisterFromGrid(RoomScript.RoomData.GetLeftMostIndex(), RoomScript.RoomData.RoomSize);
        _RoomList[roomId].RoomScript.RemoveAllOwners();
        Destroy(_RoomList[roomId].RoomObject);
        _RoomList[roomId].RoomObject = null;
        _RoomList.Remove(roomId);
    }

    public RoomRef GetRoomData(Guid roomId)
    {
        if (IsRoomExisting(roomId))
            return (_RoomList[roomId]);
        else
            return null;
    }

    public RoomRef GetRoomData(GridIndex gridIndex)
    {
        if (!gridIndex.IsValid())
            return null;
        return GetRoomData(GridManager.Ref.GetGridTileRoomGuid(gridIndex));
    }

    public void SelectRoom(Guid roomId)
    {
        GameObject Selector = null;

        switch (_RoomList[roomId].RoomScript.RoomData.RoomSize)
        {
            case Enums.RoomSizes.Size1:
                Selector = RoomSelectorSz1;
                RoomSelectorSz2.SetActive(false);
                RoomSelectorSz4.SetActive(false);
                RoomSelectorSz6.SetActive(false);
                break;
            case Enums.RoomSizes.Size2:
                Selector = RoomSelectorSz2;
                RoomSelectorSz1.SetActive(false);
                RoomSelectorSz4.SetActive(false);
                RoomSelectorSz6.SetActive(false);
                break;
            case Enums.RoomSizes.Size4:
                Selector = RoomSelectorSz4;
                RoomSelectorSz1.SetActive(false);
                RoomSelectorSz2.SetActive(false);
                RoomSelectorSz6.SetActive(false);
                break;
            case Enums.RoomSizes.Size6:
                Selector = RoomSelectorSz6;
                RoomSelectorSz1.SetActive(false);
                RoomSelectorSz2.SetActive(false);
                RoomSelectorSz4.SetActive(false);
                break;
        }

        Debug.Assert(Selector != null);
        Selector.transform.position = _RoomList[roomId].RoomObject.transform.position;
        Selector.SetActive(true);
    }

    public void DeselectRooms()
    {
        RoomSelectorSz1.SetActive(false);
        RoomSelectorSz2.SetActive(false);
        RoomSelectorSz4.SetActive(false);
        RoomSelectorSz6.SetActive(false);
    }

    //NEED TO ADD A CASE HERE TO ALLOW HALLWAY // BEDROOMS TO BE REMOVEABLE, WITH THIS CLAUSE HALLWAYS CANNOT EVER BE REMOVED
    private int CountExternalRoomLinks(Guid roomId)
    {
        RoomScript RoomScript = _RoomList[roomId].RoomScript;

        int Count = 0;

        // Count all links, external and internal
        foreach (GridIndex Index in RoomScript.RoomData.CoveredIndizes)
        {
            Count += GridManager.Ref.CountLinksFromGridTile(Index);
        }

        // Subtract internal links
        switch (RoomScript.RoomData.RoomSize)
        {
            case Enums.RoomSizes.Size1: Count -= 2; break;
            case Enums.RoomSizes.Size2: Count -= 2; break;
            case Enums.RoomSizes.Size4: Count -= 6; break;
            case Enums.RoomSizes.Size6: Count -= 8; break;
        }

        return (Count);
    }

    public bool CanRemoveRoom(Guid roomId)
    {
        RoomScript RoomScript = _RoomList[roomId].RoomScript;

        if (IsEntranceRoom(roomId)) return (false);
        //if (RoomScript.AllOwnerSlotsAreEmpty() == false) return (false);
        if (RoomScript.AllManSlotsAreEmpty() == false) return (false);

        // Check if it is linked to only one other room. Then, we can always safely remove
        if (CountExternalRoomLinks(roomId) == 1) return (true);

        // The room can be removed if any linked tiles still have access to entrance
        if (GridManager.Ref.CanTilesBeRemovedSafely(RoomScript.RoomData.CoveredIndizes) == false)
        {
            return (false);
        }

        return (true);
    }

    public void HighlightRoom(Guid roomId)
    {
        if (StateManager.Ref.GetHighlightedRoom() == roomId) return;

        //GameObject Selector = null;
        RoomHighlighterSz2.SetActive(false);
        RoomHighlighterSz4.SetActive(false);
        RoomHighlighterSz6.SetActive(false);
        RoomHighlighter_Bedroom.SetActive(false);
        switch (_RoomList[roomId].RoomScript.RoomData.RoomSize)
        {
            case Enums.RoomSizes.Size1:
                HighlightNoRoom();
                return;
            case Enums.RoomSizes.Size2:
                StateManager.Ref.SetCurrentHoveredRoom(roomId);
                RoomHighlighterSz2.transform.position = _RoomList[roomId].RoomObject.transform.position;
                RoomHighlighterSz2.SetActive(true);
                break;
            case Enums.RoomSizes.Size4:
                StateManager.Ref.SetCurrentHoveredRoom(roomId);
                RoomHighlighterSz4.transform.position = _RoomList[roomId].RoomObject.transform.position;
                RoomHighlighterSz4.SetActive(true);
                break;
            case Enums.RoomSizes.Size6:
                StateManager.Ref.SetCurrentHoveredRoom(roomId);
                RoomHighlighterSz6.transform.position = _RoomList[roomId].RoomObject.transform.position;
                RoomHighlighterSz6.SetActive(true);
                break;
        }

        //StateManager.Ref.SetHighlightedRoom(roomId);
        //Selector.transform.position = _RoomList[roomId].RoomObject.transform.position;
        //Selector.SetActive(true);
    }

    public void HighlightNoRoom()
    {
        RoomHighlighterSz2.SetActive(false);
        RoomHighlighterSz4.SetActive(false);
        RoomHighlighterSz6.SetActive(false);
        RoomHighlighter_Bedroom.SetActive(false);
        StateManager.Ref.SetCurrentHoveredRoom(Guid.Empty);
    }

    public void HighlightBedroom(Guid hallwayId, Vector3 highlightPosition)
    {
        Debug.Assert(RoomHighlighter_Bedroom != null);

        if (RoomHighlighter_Bedroom.activeInHierarchy && RoomHighlighter_Bedroom.transform.position == highlightPosition)
            return;

        RoomHighlighterSz2.SetActive(false);
        RoomHighlighterSz4.SetActive(false);
        RoomHighlighterSz6.SetActive(false);

        StateManager.Ref.SetCurrentHoveredRoom(hallwayId);
        RoomHighlighter_Bedroom.transform.position = highlightPosition;
        RoomHighlighter_Bedroom.SetActive(true);
    }

    public bool IsEntranceRoom(Guid roomId)
    {
        RoomScript RoomScript = _RoomList[roomId].RoomScript;

        
        return (RoomScript.RoomData.GetLeftMostIndex() == Constants.EntranceRoomIndex || RoomScript.RoomData.GetLeftMostIndex() == Constants.UWEntranceRoomIndex);
    }

    public void RemoveAllRooms()
    {
        foreach (KeyValuePair<Guid, RoomRef> Entry in _RoomList)
        {
            Destroy(Entry.Value.RoomObject);
            Entry.Value.RoomObject = null;
        }

        _RoomList.Clear();

        GridManager.Ref.EmptyGrid();
    }

    public void SaveRoomList(string fileName)
    {
        XmlSerializer Serializer = new XmlSerializer(typeof(RoomInstanceData[]));

        RoomInstanceData[] tmpOutput = new RoomInstanceData[_RoomList.Count];

        // Saving as an array. Key GUID is also in the data
        int Count = 0;
        foreach (KeyValuePair<Guid, RoomRef> Entry in _RoomList)
        {
            tmpOutput[Count] = Entry.Value.RoomScript.RoomData;
            Count++;
        }

        using (StreamWriter Writer = new StreamWriter(fileName))
        {
            Serializer.Serialize(Writer, tmpOutput);
        }
    }

    public void LoadRoomList(string fileName)
    {
        RemoveAllRooms();

        XmlSerializer Serializer = new XmlSerializer(typeof(RoomInstanceData[]));
        using (FileStream FileStream = new FileStream(fileName, FileMode.Open))
        {
            RoomInstanceData[] tmpInput = (RoomInstanceData[])Serializer.Deserialize(FileStream);
            foreach (RoomInstanceData RoomData in tmpInput)
            {
                CreateRoom(RoomData);
            }
        }
    }

    public GameObject InstantiateRoom(Enums.RoomTypes roomType)
    {
        RoomDefData DefData = GetRoomDefData(roomType);
        GameObject RoomObject = Instantiate(DefData.RoomPrefab);
        RoomObject.SetActive(true);
        return (RoomObject);
    }

    public static RoomDefData GetRoomDefData(Enums.RoomTypes roomType)
    {
        foreach (RoomDefData DefData in RoomDefinitions)
        {
            if ((DefData.RoomType == roomType))
            {
                return (DefData);
            }
        }

        Debug.Assert(1 == 0);
        return (RoomDefinitions[0]);
    }

    public static RoomDefData[] GetAllRoomsofCategory(Enums.RoomCategories category, bool excludeUnbuildables = false)
    {
        List<RoomDefData> rooms = new List<RoomDefData>();

        for (int i = 0; i < RoomDefinitions.Length; i++)
        {
            if (excludeUnbuildables && RoomDefinitions[i].RoomCost == 0)
                continue;
            if (RoomDefinitions[i].RoomCategory == category)
                rooms.Add(RoomDefinitions[i]);
        }

        return rooms.ToArray();
    }

    public static RoomDefData[] GetAllRoomsofCategory(Enums.RoomCategories[] categories, bool excludeUnbuildables = false)
    {
        List<RoomDefData> rooms = new List<RoomDefData>();

        for (int i = 0; i < RoomDefinitions.Length; i++)
        {
            if (excludeUnbuildables && RoomDefinitions[i].RoomCost == 0)
                continue;
            if (categories.Contains(RoomDefinitions[i].RoomCategory))
                rooms.Add(RoomDefinitions[i]);
        }

        return rooms.ToArray();
    }

    public Room_WorkQuarters[] GetAllActiveWorkQuartersOfCategory(Room_WorkQuarters.WorkQuartersType type)
    {
        return (from r in GetAllActiveRoomsofType<Room_WorkQuarters>() where r.WorkQuarterType == type select r).ToArray();
    }

    public RoomRef[] GetAllActiveRoomsofType(params Enums.RoomTypes[] roomTypes)
    {
        return (from r in _RoomList where roomTypes.Contains(r.Value.RoomScript.RoomData.RoomType) select r.Value).ToArray();
    }

    public T[] GetAllActiveRoomsofType<T>() where T : RoomScript
    {
        return (from r in _RoomList where r.Value.RoomScript is T select r.Value.RoomScript as T).ToArray();
    }

    public T GetClosestRoom<T>(IEnumerable<T> rooms, GridIndex pos) where T : RoomScript
    {
        int shortest = int.MaxValue;
        T ret = null;
        foreach (T r in rooms)
        {
            int l = GridManager.Ref.GetIndexPath(r.RoomData.GetLeftMostIndex(), pos).Length;
            if (l < shortest)
                shortest = l;
            ret = r;
        }
        return ret;
    }

    public static bool IsRoomOfType<T>(Guid id) where T : RoomScript
    {
        return IsRoomOfType<T>(Ref.GetRoomData(id).RoomScript);
    }

    public static bool IsRoomOfType<T>(RoomScript room) where T : RoomScript
    {
        return room is T;
    }
    #endregion

    #region RoomInfo Helper Functions
    public int GetCostByRoomType(Enums.RoomTypes roomType)
    {
        for (int i = 0; i < roomDefinitions.Length; i++)
        {
            if (roomDefinitions[i].GetComponent<RoomScript>().RoomStats.RoomType == roomType)
                return roomDefinitions[i].GetComponent<RoomScript>().RoomStats.RoomCost;
        }
        Debug.LogWarning("RoomManager.cs 'GetCostByRoomType'/n No Room of the given RoomType: " + roomType + " was found in the roomDefinitions.");
        return 0;
    }

    public Enums.RoomSizes GetRoomSizeByRoomType(Enums.RoomTypes roomType)
    {
        for (int i = 0; i < roomDefinitions.Length; i++)
        {
            if (roomDefinitions[i].GetComponent<RoomScript>().RoomStats.RoomType == roomType)
                return roomDefinitions[i].GetComponent<RoomScript>().RoomStats.RoomSize;
        }
        Debug.LogWarning("RoomManager.cs 'GetRoomSizeByRoomType'/n No Room of the given RoomType: " + roomType + " was found in the roomDefinitions.");
        return 0;
    }

    public Enums.RoomOverUnder GetOverUnderByRoomType(Enums.RoomTypes roomType)
    {
        for (int i = 0; i < roomDefinitions.Length; i++)
        {
            if (roomDefinitions[i].GetComponent<RoomScript>().RoomStats.RoomType == roomType)
                return roomDefinitions[i].GetComponent<RoomScript>().RoomStats.RoomOverUnder;
        }
        Debug.LogWarning("RoomManager.cs 'GetOverUnderByRoomType'/n No Room of the given RoomType: " + roomType + " was found in the roomDefinitions.");
        return 0;
    }
    #endregion

    public static void UnlockRoomsByType(Enums.RoomTypes type)
    {
        if (!UnlockEvents.ContainsKey(type))
            return;
        UnlockEvents[type]?.Invoke();
    }

    public void CreateStartRooms()
    {
        //Lobby
        lobbyId = Guid.NewGuid();
        CreateRoom(lobbyId, Enums.RoomTypes.Lobby, Constants.EntranceRoomIndex);

        //Underworld Lobby
        underLobbyId = Guid.NewGuid();
        CreateRoom(underLobbyId, Enums.RoomTypes.UnderLobby, Constants.UWEntranceRoomIndex);
    }

    #region Serializable Struct (for inspector readability/clarity)
    [Serializable]
    public struct Room
    {
        public string RoomName;

        public Enums.RoomSizes RoomSize;

        [Tooltip("What category the room falls under (What tab it will show up under the build options when selecting a room to build)")]
        public Enums.RoomCategories RoomCategory;

        public Enums.RoomTypes RoomType;

        [Tooltip("The maximum amount of people that can be in this room at any given point")]
        public int ManSlotCount;

        public string RoomDescription;

        [Tooltip("Whether or not the room can be built above or below ground, or both")]
        public Enums.RoomOverUnder RoomOverUnder;

        [Tooltip("The cost of the room in Hoots")]
        public int RoomCost;

        [Tooltip("If set to true, the room will not be purchaseable until the player has unlocked it.")]
        public bool locked;

        public Sprite roomSprite;
    }
    #endregion
}