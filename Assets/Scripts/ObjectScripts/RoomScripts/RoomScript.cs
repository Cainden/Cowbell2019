// This script is attached to the rooms

using MySpace;
using System;
using UnityEngine;
using System.Collections.Generic;

public class RoomScript : MonoBehaviour
{
    [SerializeField]protected GameObject MeshBothOpen = null, MeshROpen = null, MeshLOpen = null;

    // Instance specific data. Kept separate for easier serialization
    [HideInInspector] public RoomInstanceData RoomData;

    // Used by the room manager
    public RoomManager.Room RoomStats;

    // Internals
    public bool RoomIsActive { get; private set; } // True if at least one man working here
    
    public virtual Enums.ManRole RoomRole { get { return Enums.ManRole.None; } }

    protected ManManager manManRef;

    public Guid adjacentRight, adjacentLeft;

    public GridIndex leftMost;

    protected virtual void Start()
    {
        AssignManSlotPositions();
        CheckReferences();
        SetRoomText();
        
        manManRef = ManManager.Ref;
        leftMost = RoomData.GetLeftMostIndex();
    }


    protected virtual void Update()
    {
        //Left this here for now, it may need to be used later
    }

    private void CheckReferences()
    {
        Debug.Assert(RoomData != null);
        Debug.Assert(RoomData.ManSlotCount == RoomData.ManSlotsPositions.Length);
        Debug.Assert(RoomData.ManSlotCount == RoomData.ManSlotsRotations.Length);
        Debug.Assert(RoomData.ManSlotCount == RoomData.ManSlotsAssignments.Length);
        Debug.Assert(MeshBothOpen != null);
        //dont need to assert the other two, not all rooms will have all options (like the lobby)
    }

    private void AssignManSlotPositions() // Get men positions from model
    {
        Transform[] Children = GetComponentsInChildren<Transform>();

        for (int i = 0; i < Children.Length; i++)
        {
            string SlotName = "SlotPos" + (i + 1).ToString();
            foreach (Transform Child in Children)
            {
                if (Child.name == SlotName)
                {
                    RoomData.ManSlotsPositions[i] = Child.position;
                    RoomData.ManSlotsRotations[i] = Child.rotation;
                }
            }
        }
    }

    /// <summary>
    /// Use this if you want guests to behave in certain ways while inside of the room.
    /// </summary>
    /// <param name="guest"></param>
    public virtual void GuestBehavior(ManScript_Guest guest) { }

    #region Helper Functions

    public virtual void OnInitialization()
    {
        ReCheckAdjacency();
        HaveAdjacentsCheckAdjacency();
    }

    public virtual void ManHasEntered(ManScript man) { }

    public bool HasOwner()
    {
        for (int i = 0; i < RoomData.OwnerSlotsAssignments.Length; i++)
        {
            if (RoomData.OwnerSlotsAssignments[i] != Guid.Empty)
                return true;
        }
        return false;
    }

    public virtual bool RoomHasFreeManSlots(Guid manId)
    {
        for(int i = 0; i < RoomData.ManSlotCount; i++)
        {
            if (RoomData.ManSlotsAssignments[i] == Guid.Empty && (RoomData.ReservedSlots[i].object1 == Guid.Empty || RoomData.ReservedSlots[i].object1 == manId))
                return true;
        }
        return (false);
    }

    public virtual bool RoomHasFreeManSlots(ManScript man)
    {
        for (int i = 0; i < RoomData.ManSlotCount; i++)
        {
            if (RoomData.ManSlotsAssignments[i] == Guid.Empty && (RoomData.ReservedSlots[i].object1 == Guid.Empty || RoomData.ReservedSlots[i].object1 == man.ManData.ManId))
                return true;
        }
        return (false);
    }


    public bool AllManSlotsAreEmpty()
    {
        for (int i = 0; i < RoomData.ManSlotCount; i++)
        {
            if (RoomData.ManSlotsAssignments[i] != Guid.Empty || (RoomData.ReservedSlots[i].object1 != Guid.Empty))
                return false;
        }
        return (true);
    }

    public virtual int GetFreeManSlotIndex(ManScript man)
    {
        for (int i = 0; i < RoomData.ManSlotCount; i++)
        {
            if (RoomData.ManSlotsAssignments[i] == Guid.Empty && (RoomData.ReservedSlots[i].object1 == Guid.Empty))
            {
                return (i);
            }
        }
        return (-1);
    }

    public int GetFreeManSlotIndex(Guid ManId)
    {
        return GetFreeManSlotIndex(ManManager.Ref.GetManData(ManId).ManScript);
    }

    public int GetReservedSlotIndex(Guid manId)
    {
        for(int i = 0; i < RoomData.ManSlotCount; i++)
        {
            if (RoomData.ReservedSlots[i].object1 == manId)
                return i;
        }
        return -1;
    }

    public bool GetReservedSlotAssignedByPlayer(Guid manId)
    {
        for (int i = 0; i < RoomData.ManSlotCount; i++)
        {
            if (RoomData.ReservedSlots[i].object1 == manId)
                return RoomData.ReservedSlots[i].object2;
        }
        return false;
    }

    public bool RoomContainsMan(Guid manId)
    {
        foreach (Guid g in RoomData.ManSlotsAssignments)
        {
            if (g == manId) return (true);
        }
        return (false);
    }

    public int CountMen()
    {
        int Count = 0;
        foreach (Guid g in RoomData.ManSlotsAssignments)
        {
            if (g != Guid.Empty) Count++;
        }
        return (Count);
    }

    public virtual void AssignManToRoomSlot(Guid manId, int slotIndex, bool assignedByPlayer)
    {
        if (RoomData.ReservedSlots[slotIndex].object1 != Guid.Empty && RoomData.ReservedSlots[slotIndex].object1 != manId)
            Debug.LogError("Room Slot Assignment Attempted in a reserved slot!!!; ManType: " + ManManager.Ref.GetManData(manId).ManScript.ManData.ManType);
        RoomData.ManSlotsAssignments[slotIndex] = manId;
        RoomData.ReservedSlots[slotIndex] = new Container<Guid, bool>() { object1 = Guid.Empty, object2 = false };

        RoomIsActive = true; // General sign that there is a worker
    }

    public virtual void ReserveSlotForMan(Guid manId, int slotIndex, bool assignedByPlayer)
    {
        if (RoomData.ManSlotsAssignments[slotIndex] != Guid.Empty)
            Debug.LogError("Attempted to reserve an already occupied slot!!!; ManType: " + ManManager.Ref.GetManData(manId).ManScript.ManData.ManType);
        if (RoomData.ReservedSlots[slotIndex].object1 != Guid.Empty && RoomData.ReservedSlots[slotIndex].object1 != manId)
            Debug.LogError("Room Slot Reservation Attempted in an already reserved slot!!!; ManType: " + ManManager.Ref.GetManData(manId).ManScript.ManData.ManType);
        RoomData.ReservedSlots[slotIndex].object1 = manId;
        RoomData.ReservedSlots[slotIndex].object2 = assignedByPlayer;

    }

    public virtual void RemoveManFromRoomSlot(Guid manId)
    {
        for (int i = 0; i < RoomData.ManSlotCount; i++)
        {
            if (RoomData.ManSlotsAssignments[i] == manId) RoomData.ManSlotsAssignments[i] = Guid.Empty;
        }

        if (AllManSlotsAreEmpty()) RoomIsActive = false;
    }

    // ROOM OWNER CODE /////////////
    public virtual bool RoomHasFreeOwnerSlots()
    {
        foreach (Guid g in RoomData.OwnerSlotsAssignments)
        {
            if (g == Guid.Empty) return (true);
        }
        return (false);
    }

    public bool AllOwnerSlotsAreEmpty()
    {
        foreach (Guid g in RoomData.OwnerSlotsAssignments)
        {
            if (g != Guid.Empty) return (false);
        }
        return (true);
    }

    public int GetFreeOwnerSlotIndex()
    {
        for (int i = 0; i < RoomData.ManSlotCount; i++)
        {
            if (RoomData.OwnerSlotsAssignments[i] == Guid.Empty)
            {
                return (i);
            }
        }
        return (-1);
    }

    public void AssignOwnerToRoomSlot(Guid manId, int slotIndex)
    {
        RoomData.OwnerSlotsAssignments[slotIndex] = manId;
        //RoomIsActive = true; // General sign that there is a worker
    }

    public void RemoveOwnerFromRoomSlot(Guid manId)
    {
        for (int i = 0; i < RoomData.ManSlotCount; i++)
        {
            if (RoomData.OwnerSlotsAssignments[i] == manId) RoomData.OwnerSlotsAssignments[i] = Guid.Empty;
        }

        //if (AllOwnerSlotsAreEmpty()) RoomIsActive = false;
    }

    public void RemoveAllOwners()
    {
        for (int i = 0; i < RoomData.ManSlotCount; i++)
        {
            if(RoomData.OwnerSlotsAssignments[i] != Guid.Empty)
                manManRef.GetManData(RoomData.OwnerSlotsAssignments[i]).ManScript.RemoveRoomOwnership();
        }
    }

    private void SetRoomText()
    {
        Transform[] Children = GetComponentsInChildren<Transform>();

        foreach (Transform Child in Children)
        {
            if (Child.name == "TextPos")
            {
                GameObject RoomText = Instantiate(Resources.Load<GameObject>("RoomText"));
                //  RoomText.GetComponent<TextMesh>().text = RoomData.RoomType.ToString() + " " + RoomData.RoomSize.ToString();
                RoomText.GetComponent<TextMesh>().text = RoomData.RoomName.ToString();
                RoomText.transform.position = Child.transform.position;
                RoomText.transform.SetParent(transform);
                return;
            }
        }
    }

    public void ApplyWithAllMenOfType<T>(Action<ManRef<T>> action) where T : ManScript
    {
        int menFound = 0, c = CountMen();
        for (int i = 0; menFound < c; i++)
        {
            if (RoomData.ManSlotsAssignments[i] != Guid.Empty)
            {
                if (!manManRef.IsManTypeOf<T>(RoomData.ManSlotsAssignments[i]))
                    continue;
                menFound++;
                action(manManRef.GetManData<T>(RoomData.ManSlotsAssignments[i]));
            }
        }
    }

    public ManRef<T>[] GetAllMenOfType<T>() where T : ManScript
    {
        List<ManRef<T>> men = new List<ManRef<T>>();
        for (int i = 0; i < RoomData.ManSlotsAssignments.Length; i++)
        {
            if (RoomData.ManSlotsAssignments[i] != Guid.Empty)
            {
                if (!manManRef.IsManTypeOf<T>(RoomData.ManSlotsAssignments[i]))
                    continue;
                men.Add(manManRef.GetManData<T>(RoomData.ManSlotsAssignments[i]));
            }
        }
        return men.ToArray();
    }
    #endregion

    #region Adjacency Check

    private void HaveAdjacentsCheckAdjacency()
    {
        RoomManager.Ref.GetRoomData(GridManager.Ref.GetGridTileRoomGuid(RoomData.CoveredIndizes[0].GetLeft()))?.RoomScript?.ReCheckAdjacency();
        RoomManager.Ref.GetRoomData(GridManager.Ref.GetGridTileRoomGuid(RoomData.GetRightMostIndex().GetRight()))?.RoomScript?.ReCheckAdjacency();
    }

    public void ReCheckAdjacency()
    {
        bool left = CheckIsRoomLeft();
        bool right = CheckIsRoomRight();
        if (left && right)
        {
            MeshBothOpen.SetActive(true);
            if (MeshLOpen != null)
                MeshLOpen?.SetActive(false);
            if (MeshROpen != null)
                MeshROpen?.SetActive(false);
        }
        else if (MeshLOpen != null && left)
        {
            MeshBothOpen.SetActive(false);
            if (MeshLOpen != null)
                MeshLOpen?.SetActive(true);
            if (MeshROpen != null)
                MeshROpen?.SetActive(false);
        }
        else if (MeshROpen != null && right)
        {
            MeshBothOpen.SetActive(false);
            if (MeshLOpen != null)
                MeshLOpen?.SetActive(false);
            if (MeshROpen != null)
                MeshROpen?.SetActive(true);
        }
        else
        {
            MeshBothOpen.SetActive(true);
            if (MeshLOpen != null)
                MeshLOpen.SetActive(false);
            if (MeshROpen != null)
                MeshROpen?.SetActive(false);
        }
    }

    public bool CheckIsRoomLeft()
    {
        return RoomManager.Ref.GetRoomData(GridManager.Ref.GetGridTileRoomGuid(RoomData.CoveredIndizes[0].GetLeft())) != null;
    }

    public bool CheckIsRoomLeft(out RoomScript room)
    {
        room = RoomManager.Ref.GetRoomData(GridManager.Ref.GetGridTileRoomGuid(RoomData.CoveredIndizes[0].GetLeft())).RoomScript;
        return room != null;
    }

    public bool CheckIsRoomLeft<T>(out T room) where T : RoomScript
    {
        room = RoomManager.Ref.GetRoomData(GridManager.Ref.GetGridTileRoomGuid(RoomData.CoveredIndizes[0].GetLeft())).RoomScript as T;
        return room != null;
    }

    public bool CheckIsRoomRight()
    {
        return RoomManager.Ref.GetRoomData(GridManager.Ref.GetGridTileRoomGuid(RoomData.GetRightMostIndex().GetRight())) != null;
    }

    public bool CheckIsRoomRight(out RoomScript room)
    {
        room = RoomManager.Ref.GetRoomData(GridManager.Ref.GetGridTileRoomGuid(RoomData.GetRightMostIndex().GetRight())).RoomScript;
        return room != null;
    }

    public bool CheckIsRoomRight<T>(out T room) where T : RoomScript
    {
        room = RoomManager.Ref.GetRoomData(GridManager.Ref.GetGridTileRoomGuid(RoomData.GetRightMostIndex().GetRight())).RoomScript as T;
        return room != null;
    }

    #endregion
}
