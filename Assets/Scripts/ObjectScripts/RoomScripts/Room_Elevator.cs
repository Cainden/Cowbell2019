using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MySpace;

public class Room_Elevator : RoomScript
{
    private Animator _Animator;

    #region DoorUtilityFunctions
    float c = -1;
    private float clipLength
    {
        get
        {
            if (c < 0)
            {
                c = _Animator.runtimeAnimatorController.animationClips[0].length;
            }
            return c;
        }
    }
    private bool GetDoorIsOpen
    {
        get
        {
            if (_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= clipLength && !_Animator.IsInTransition(0))
            {
                return _Animator.GetBool("Ele_DoorOpen");
            }
            else
                return false;
        }
    }
    private bool GetDoorIsClosed
    {
        get
        {
            if (_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= clipLength && !_Animator.IsInTransition(0))
            {
                return !_Animator.GetBool("Ele_DoorOpen");
            }
            else
                return false;
        }
    }
    public bool CheckDoor(bool closed)
    {
        if (closed)
            return GetDoorIsClosed;
        else
            return GetDoorIsOpen;
    }
    #endregion

    private List<Room_Elevator> eTower = null;

    public override void OnInitialization()
    {
        base.OnInitialization();
        CheckReferences();

        Guid above = GridManager.Ref.GetGridTileRoomGuid(RoomData.CoveredIndizes[0].GetAbove()), below = GridManager.Ref.GetGridTileRoomGuid(RoomData.CoveredIndizes[0].GetBelow());

        if (eTower != null) return;

        if (RoomManager.Ref.GetRoomData(below)?.RoomScript.RoomData.RoomType == Enums.RoomTypes.Elevator)
        {
            eTower = (RoomManager.Ref.GetRoomData(below).RoomScript as Room_Elevator).eTower;
            eTower.Add(this);

            //if both above and below
            if (RoomManager.Ref.GetRoomData(above)?.RoomScript.RoomData.RoomType == Enums.RoomTypes.Elevator)
            {
                foreach (Room_Elevator room in (RoomManager.Ref.GetRoomData(above).RoomScript as Room_Elevator).eTower)
                {
                    //Add that room to this elevator's elevator list
                    eTower.Add(room);

                    //Have all of those rooms' lists become the total list
                    room.eTower = eTower;
                }
            }
        }
        else if (RoomManager.Ref.GetRoomData(above)?.RoomScript.RoomData.RoomType == Enums.RoomTypes.Elevator)
        {
            eTower = (RoomManager.Ref.GetRoomData(above).RoomScript as Room_Elevator).eTower;
            eTower.Add(this);
        }
        else if (RoomData.CoveredIndizes[0].Y > Constants.GridSurfaceY && below == Guid.Empty)
        {
            eTower = new List<Room_Elevator>() { this };
            Guid temp = Guid.NewGuid();
            RoomManager.Ref.CreateRoom(temp, Enums.RoomTypes.Elevator, RoomData.CoveredIndizes[0].GetBelow());
            eTower.Add(RoomManager.Ref.GetRoomData(temp).RoomScript as Room_Elevator);
        }
        else if (above == Guid.Empty)
        {
            eTower = new List<Room_Elevator>() { this };
            Guid temp = Guid.NewGuid();
            RoomManager.Ref.CreateRoom(temp, Enums.RoomTypes.Elevator, RoomData.CoveredIndizes[0].GetAbove());
            eTower.Add(RoomManager.Ref.GetRoomData(temp).RoomScript as Room_Elevator);
        }
        
    }

    private void CheckReferences()
    {
        _Animator = GetComponentInChildren<Animator>();
        Debug.Assert(_Animator != null);
    }

    private ManScript manHere = null;
    public void SetAnimation_OpenDoor()
    {
        _Animator.SetBool("Ele_DoorOpen", true);
    }

    public void SetAnimation_CloseDoor(bool manLeaving)
    {
        _Animator.SetBool("Ele_DoorOpen", false);
        if (manLeaving)
        {
            foreach (Room_Elevator r in eTower)
            {
                r.manHere = null;
            }
        }
    }

    public override void ManHasEntered(ManScript man)
    {
        foreach (Room_Elevator r in eTower)
        {
            r.manHere = man;
        }
    }

    public override bool GetAccessRequest(ManScript man)
    {
        if (manHere == null)
            return true;
        else
            return man == manHere;
    }
}
