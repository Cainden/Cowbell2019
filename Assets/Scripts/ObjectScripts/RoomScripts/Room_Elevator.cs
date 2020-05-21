using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MySpace;

public class Room_Elevator : RoomScript
{
    private Animator _Animator;

    private const float boxSpeed = 1.5f;
    [SerializeField] private Transform eBox;

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

    private ElevatorArrow arrow;

    public override void OnInitialization()
    {
        base.OnInitialization();
        CheckReferences();

        boxPos = eBox.position;

        Guid above = GridManager.Ref.GetGridTileRoomGuid(RoomData.CoveredIndizes[0].GetAbove()), below = GridManager.Ref.GetGridTileRoomGuid(RoomData.CoveredIndizes[0].GetBelow());

        if (eTower != null) return;

        if (RoomManager.Ref.GetRoomData(below)?.RoomScript.RoomData.RoomType == Enums.RoomTypes.Elevator)
        {
            eTower = (RoomManager.Ref.GetRoomData(below).RoomScript as Room_Elevator).eTower;
            ResetBox((RoomManager.Ref.GetRoomData(below).RoomScript as Room_Elevator).eBox);
            if (!eTower.Contains(this))
                eTower.Add(this);

            //if both above and below
            if (RoomManager.Ref.GetRoomData(above)?.RoomScript.RoomData.RoomType == Enums.RoomTypes.Elevator)
            {
                foreach (Room_Elevator room in (RoomManager.Ref.GetRoomData(above).RoomScript as Room_Elevator).eTower)
                {
                    //Add that room to this elevator's elevator list
                    if (!eTower.Contains(room))
                        eTower.Add(room);

                    //Have all of those rooms' lists become the total list
                    room.eTower = eTower;
                    room.ResetBox(eBox);
                }
            }
        }
        else if (RoomManager.Ref.GetRoomData(above)?.RoomScript.RoomData.RoomType == Enums.RoomTypes.Elevator)
        {
            eTower = (RoomManager.Ref.GetRoomData(above).RoomScript as Room_Elevator).eTower;
            if (!eTower.Contains(this))
                eTower.Add(this);
            ResetBox((RoomManager.Ref.GetRoomData(above).RoomScript as Room_Elevator).eBox);
        }
        else if (RoomData.CoveredIndizes[0].Y > Constants.GridSurfaceY && below == Guid.Empty)
        {
            eTower = new List<Room_Elevator>() { this };
            Guid temp = Guid.NewGuid();
            RoomManager.Ref.CreateRoom(temp, Enums.RoomTypes.Elevator, RoomData.CoveredIndizes[0].GetBelow());
            //eTower.Add(RoomManager.Ref.GetRoomData(temp).RoomScript as Room_Elevator);
        }
        else if (above == Guid.Empty)
        {
            eTower = new List<Room_Elevator>() { this };
            Guid temp = Guid.NewGuid();
            RoomManager.Ref.CreateRoom(temp, Enums.RoomTypes.Elevator, RoomData.CoveredIndizes[0].GetAbove());
            //eTower.Add(RoomManager.Ref.GetRoomData(temp).RoomScript as Room_Elevator);
        }

        //this needs to happen last since it uses the eTower to set it's position
        arrow.SetArrowAngle(GetBoxTIndex(eBox.position));
    }

    private void CheckReferences()
    {
        _Animator = GetComponentInChildren<Animator>();
        arrow = GetComponentInChildren<ElevatorArrow>();
        Debug.Assert(_Animator != null);
        Debug.Assert(eBox != null);
        Debug.Assert(arrow != null);
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

    public void SetTowerMoving(bool moving)
    {
        foreach (Room_Elevator r in eTower)
        {
            r.BoxMoving = moving;
        }
    }

    public override bool GetAccessRequest(ManScript man)
    {
        if (manHere == null)
            return !BoxMoving;
        else
            return man == manHere && !BoxMoving;
    }

    private Vector3 boxPos;
    public void ResetBox(Transform box)
    {
        if (eBox != null && eBox != box)
        {
            Destroy(eBox.gameObject);
        }
        eBox = box;
    }

    public void MoveBoxToFloor(int floorYGridIndex)
    {
        foreach (Room_Elevator room in eTower)
        {
            if (room.RoomData.CoveredIndizes[0].Y == floorYGridIndex)
            {
                StartCoroutine(BoxMove(room));
            }
        }
    }

    public void MoveBoxToFloor(int floorYGridIndex, ManScript manToMove)
    {
        foreach (Room_Elevator room in eTower)
        {
            if (room.RoomData.CoveredIndizes[0].Y == floorYGridIndex)
            {
                StartCoroutine(BoxMove(room, manToMove));
            }
        }
    }

    private float GetBoxTIndex(Vector3 boxPos)
    {
        int numLower = 0;
        for (int i = 0; i < eTower.Count; i++)
        {
            if (eTower[i].boxPos.y < boxPos.y)
                numLower++;
        }
        if (eTower.Count <= 1)
        {
            //This will happen when the first elevator of a tower is built
            return 0;
        }
        return numLower / (float)(eTower.Count - 1);
    }

    private void SetTowerArrowAngle(float t)
    {
        foreach (Room_Elevator room in eTower)
        {
            room.arrow.SetArrowAngle(t);
        }
    }

    public bool BoxMoving { get; private set; }
    private IEnumerator BoxMove(Room_Elevator moveTo, ManScript manToMove = null)
    {
        if (!eTower.Contains(moveTo))
        {
            yield break;
        }
        SetTowerMoving(true);
        if (!(moveTo.eBox.position == moveTo.boxPos))
        {
            Vector3 start = eBox.position, manStart = manToMove?.transform.position ?? Vector3.zero, manEnd = manStart + (moveTo.boxPos - moveTo.eBox.position);
            float t = 0, speed = (moveTo.boxPos - moveTo.eBox.position).magnitude / boxSpeed;
            float aStartT = GetBoxTIndex(start), aEndT = GetBoxTIndex(moveTo.boxPos);
            while (t < 1)
            {
                t += Time.deltaTime / speed;
                if (t > 1)
                    t = 1;
                eBox.transform.position = Vector3.Lerp(start, moveTo.boxPos, t);
                SetTowerArrowAngle(Mathf.Lerp(aStartT, aEndT, t));
                if (manToMove != null)
                {
                    manToMove.transform.position = Vector3.Lerp(manStart, manEnd, t);
                }
                yield return null;
            }
        }
        SetTowerMoving(false);
    }
}
