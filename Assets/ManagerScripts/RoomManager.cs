// Managing the rooms' list.

using MySpace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    #region Serialized Variables
    [Tooltip("A ratio applied to all calculations related to cleaning speed")]
    [Range(0.001f, 100)]
    [SerializeField] float cleanSpeedRatio = 0.1f;
    /// <summary>
    /// A ratio applied to all calculations related to cleaning speed
    /// </summary>
    public float CleanSpeedRatio { get { return cleanSpeedRatio; } }

    [Header("===============================================================================================================================================")]
    [Header("Room Highlights & Selectors")]
    public GameObject RoomSelectorSz1; // To be set by editor
    public GameObject RoomSelectorSz2;
    public GameObject RoomSelectorSz4;
    public GameObject RoomSelectorSz6;
    public GameObject RoomHighlighterSz2;
    public GameObject RoomHighlighterSz4;
    public GameObject RoomHighlighterSz6;
    [Header("===============================================================================================================================================")]
    [SerializeField] Room[] roomDefinitions;

    public static RoomDefData[] RoomDefinitions { get; private set; }
    #endregion

    #region Static Variables

    public static RoomManager Ref { get; private set; } // For external access of script

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
            RoomDefinitions[i] = new RoomDefData(
                roomDefinitions[i].RoomName,
                roomDefinitions[i].RoomModelPrefab,
                roomDefinitions[i].RoomSize,
                roomDefinitions[i].RoomType,
                roomDefinitions[i].RoomCategory,
                roomDefinitions[i].ManSlotCount,
                CreateNewArray(roomDefinitions[i].ManSlotCount),
                roomDefinitions[i].RoomDescription,
                roomDefinitions[i].RoomCost,
                roomDefinitions[i].RoomOverUnder
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

        for (int i = 0; i < RoomData.ManSlotCount; i++) RoomData.ManSlotsAssignments[i] = Guid.Empty;
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

        _RoomList[roomData.RoomId] = new RoomRef(RoomObject, RoomScript);
        RoomObject.transform.position = GridManager.Ref.GetWorldPositionFromGridIndex(roomData.GetLeftMostIndex());
        GridManager.Ref.RegisterAtGrid(roomData.RoomSize, roomData.RoomId, roomData.GetLeftMostIndex());
    }

    #region Old CreateRoom
    //public void CreateRoom(Guid roomId, Enums.RoomTypes roomType, Enums.RoomSizes roomSize, Enums.RoomOverUnder roomOverUnder, GridIndex leftMostIndex)
    //{
    //    RoomDefData RoomDefData = GetRoomDefData(roomType, roomSize, roomOverUnder);

    //    RoomInstanceData RoomData = new RoomInstanceData();
    //    RoomData.RoomId = roomId;
    //    RoomData.RoomName = RoomDefData.RoomName;
    //    RoomData.RoomSize = RoomDefData.RoomSize;
    //    RoomData.RoomCategory = RoomDefData.RoomCategory;
    //    RoomData.RoomType = RoomDefData.RoomType;
    //    RoomData.RoomOverUnder = RoomDefData.RoomOverUnder;
    //    RoomData.ManSlotCount = RoomDefData.ManSlotCount;
    //    RoomData.ManSlotsPositions = new Vector3[RoomDefData.ManSlotCount]; // Data will be set by object script on Start()
    //    RoomData.ManSlotsRotations = new Quaternion[RoomDefData.ManSlotCount]; // Data will be set by object script on Start()
    //    RoomData.ManSlotsAssignments = new Guid[RoomDefData.ManSlotCount];
    //    RoomData.OwnerSlotsAssignments = new Guid[RoomDefData.ManSlotCount];

    //    for (int i = 0; i < RoomData.ManSlotCount; i++) RoomData.ManSlotsAssignments[i] = Guid.Empty;
    //    RoomData.ManWorkingStates = RoomDefData.ManWorkingStates;
    //    RoomData.CoveredIndizes = GridManager.Ref.GetOccupiedindizes(roomSize, leftMostIndex);

    //    for (int i = 0; i < RoomData.ManSlotCount; i++) RoomData.OwnerSlotsAssignments[i] = Guid.Empty;

    //    CreateRoom(RoomData);
    //}

    //public void CreateRoom(RoomInstanceData roomData)
    //{
    //    if (roomData == null) return;

    //    GameObject RoomObject = InstantiateRoom(roomData.RoomType, roomData.RoomSize, roomData.RoomOverUnder);
    //    RoomScript RoomScript = RoomObject.GetComponent<RoomScript>();
    //    RoomScript.RoomData = roomData;

    //    _RoomList[roomData.RoomId] = new RoomRef(RoomObject, RoomScript);
    //    RoomObject.transform.position = GridManager.Ref.GetWorldPositionFromGridIndex(roomData.GetLeftMostIndex());
    //    GridManager.Ref.RegisterAtGrid(roomData.RoomSize, roomData.RoomId, roomData.GetLeftMostIndex());
    //}
    #endregion

    #endregion

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
        Debug.Assert(IsRoomExisting(roomId));
        return (_RoomList[roomId]);
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

        GameObject Selector = null;

        switch (_RoomList[roomId].RoomScript.RoomData.RoomSize)
        {
            case Enums.RoomSizes.Size1:
                HighlightNoRoom();
                return;
            case Enums.RoomSizes.Size2:
                Selector = RoomHighlighterSz2;
                RoomHighlighterSz4.SetActive(false);
                RoomHighlighterSz6.SetActive(false);
                break;
            case Enums.RoomSizes.Size4:
                Selector = RoomHighlighterSz4;
                RoomHighlighterSz2.SetActive(false);
                RoomHighlighterSz6.SetActive(false);
                break;
            case Enums.RoomSizes.Size6:
                Selector = RoomHighlighterSz6;
                RoomHighlighterSz2.SetActive(false);
                RoomHighlighterSz4.SetActive(false);
                break;
        }

        Debug.Assert(Selector != null);
        StateManager.Ref.SetHighlightedRoom(roomId);
        Selector.transform.position = _RoomList[roomId].RoomObject.transform.position;
        Selector.SetActive(true);
    }

    public void HighlightNoRoom()
    {
        RoomHighlighterSz2.SetActive(false);
        RoomHighlighterSz4.SetActive(false);
        RoomHighlighterSz6.SetActive(false);
        StateManager.Ref.SetHighlightedRoom(Guid.Empty);
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

    #region RoomInfo Helper Functions
    public int GetCostByRoomType(Enums.RoomTypes roomType)
    {
        for (int i = 0; i < roomDefinitions.Length; i++)
        {
            if (roomDefinitions[i].RoomType == roomType)
                return roomDefinitions[i].RoomCost;
        }
        Debug.LogWarning("RoomManager.cs 'GetCostByRoomType'/n No Room of the given RoomType: " + roomType + " was found in the roomDefinitions.");
        return 0;
    }

    public Enums.RoomSizes GetRoomSizeByRoomType(Enums.RoomTypes roomType)
    {
        for (int i = 0; i < roomDefinitions.Length; i++)
        {
            if (roomDefinitions[i].RoomType == roomType)
                return roomDefinitions[i].RoomSize;
        }
        Debug.LogWarning("RoomManager.cs 'GetRoomSizeByRoomType'/n No Room of the given RoomType: " + roomType + " was found in the roomDefinitions.");
        return 0;
    }

    public Enums.RoomOverUnder GetOverUnderByRoomType(Enums.RoomTypes roomType)
    {
        for (int i = 0; i < roomDefinitions.Length; i++)
        {
            if (roomDefinitions[i].RoomType == roomType)
                return roomDefinitions[i].RoomOverUnder;
        }
        Debug.LogWarning("RoomManager.cs 'GetOverUnderByRoomType'/n No Room of the given RoomType: " + roomType + " was found in the roomDefinitions.");
        return 0;
    }
    #endregion

    public void CreateStartRooms()
    {
        //Lobby
        CreateRoom(Guid.NewGuid(), Enums.RoomTypes.Lobby, Constants.EntranceRoomIndex);

        //Underworld Lobby
        CreateRoom(Guid.NewGuid(), Enums.RoomTypes.UnderLobby, Constants.UWEntranceRoomIndex);
    }

    #region Serializable Struct (for inspector readability/clarity)
    [Serializable]
    private struct Room
    {
        public string RoomName;
        public GameObject RoomModelPrefab;
        public Enums.RoomSizes RoomSize;
        public Enums.RoomCategories RoomCategory;
        public Enums.RoomTypes RoomType;
        public int ManSlotCount;
        public string RoomDescription;
        public Enums.RoomOverUnder RoomOverUnder;
        public int RoomCost;
    }
    #endregion
}