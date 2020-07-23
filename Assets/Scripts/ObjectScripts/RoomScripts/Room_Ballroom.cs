﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;
using System;
using MySpace.Stats;
using System.Linq;

public class Room_Ballroom : Room_WorkQuarters
{
    private Dictionary<ManScript_Worker, ManScript_Guest> workerAssignments;

    public override WorkQuartersType WorkQuarterType => WorkQuartersType.Entertainment;

    public override Action<ManScript_Worker> GetRoleFunction => DanceDirector;

    public override Enums.ManRole RoomRole => Enums.ManRole.DanceDirector;

    public override SpecialtyStat.StatType[] specialStatsUsed => new SpecialtyStat.StatType[1] { SpecialtyStat.StatType.Physicality };

    public override void AssignManToRoomSlot(Guid manId, int slotIndex, bool assignedByPlayer)
    {
        base.AssignManToRoomSlot(manId, slotIndex, assignedByPlayer);

        Enums.ManTypes manType = ManManager.Ref.GetManData(manId).ManScript.ManType;

        if (manType == Enums.ManTypes.Worker)
        {
            workerAssignments.Add((ManManager.Ref.GetManData(manId).ManScript as ManScript_Worker), GetGuestFromWorkerSlot(slotIndex));
        }
        else if (manType == Enums.ManTypes.Guest)
        {
            if (GetWorkerFromGuestSlot(slotIndex) != null)
            {
                workerAssignments[GetWorkerFromGuestSlot(slotIndex)] = ManManager.Ref.GetManData(manId).ManScript as ManScript_Guest;
            }
        }

        
    }

    public override bool RoomHasFreeManSlots(Guid manId)
    {
        if (ManManager.Ref.GetManData(manId).ManScript.ManType == Enums.ManTypes.Guest)
            return CheckFreeGuestSlot();
        else if (ManManager.Ref.GetManData(manId).ManScript.ManType == Enums.ManTypes.Worker)
            return CheckFreeWorkerSlot();
        return base.RoomHasFreeManSlots(manId);
    }

    public override bool RoomHasFreeManSlots(ManScript man)
    {
        if (man.ManType == Enums.ManTypes.Guest)
            return CheckFreeGuestSlot();
        else if (man.ManType == Enums.ManTypes.Worker)
            return CheckFreeWorkerSlot();
        return base.RoomHasFreeManSlots(man);
    }

    public override int GetFreeManSlotIndex(ManScript man)
    {
        if (man.ManType == Enums.ManTypes.Guest)
        {
            for (int i = 0; i < RoomData.CoveredIndizes.Length; i++)
            {
                if (RoomData.CoveredIndizes[i].Z != 0)
                    continue;
                if (RoomData.ManSlotsAssignments[i] == Guid.Empty)
                    return i;
            }
        }
        else if (man.ManType == Enums.ManTypes.Worker)
        {
            for (int i = 0; i < RoomData.ManSlotsAssignments.Length; i++)
            {
                if (RoomData.CoveredIndizes[i].Z == 0)
                    continue;
                if (RoomData.ManSlotsAssignments[i] == Guid.Empty)
                    return i;
            }
        }
        return base.GetFreeManSlotIndex(man);
    }

    public override void GuestBehavior(ManScript_Guest guest)
    {
        guest.delayTimer += Time.deltaTime;
        if (guest.delayTimer < 1)
            return;
        guest.delayTimer = 0;
        if (GetWorkerFromGuestSlot(guest.ManData.AssignedRoomSlot) != null)
        {
            guest.SetAnimation(Enums.ManStates.Dancing, 0);
            guest.ChangeHappiness((3 + GetWorkerFromGuestSlot(guest.ManData.AssignedRoomSlot).GetSpecialtyStatValue(SpecialtyStat.StatType.Physicality)));
            if ((int)guest.GetMood() >= 80)
            {
                if (UnityEngine.Random.Range(0, 100) >= 70 /*Want to use a stat from the guest here once we get something for it, rather than a hard coded number*/)
                    ManManager.Ref.MoveManToNewRoom(guest.ManData.ManId, guest.ManData.OwnedRoomRef.RoomId);
            }
        }
        else
        {
            guest.SetMood(Enums.ManMood.Angry, true);
            guest.ChangeHappiness(-1 /*Could maybe add in a general stat here for guests that makes them get angrier faster?*/);
        }
    }

    protected override void Start()
    {
        base.Start();
        workerAssignments = new Dictionary<ManScript_Worker, ManScript_Guest>();
        AddRearIndizes();
    }

    private void DanceDirector(ManScript_Worker man)
    {
        Vector3 target;
        if (workerAssignments[man] != null)
        {                                                                                                                              //Have them a bit to the left of the guest.
            target = GridManager.Ref.GetWorldPositionFromGridIndex(RoomData.CoveredIndizes[man.ManData.AssignedRoomSlot].GetFront()) + (Vector3.left * 0.5f);
            if (man.transform.position != target)
            {
                man.SetAnimation(Enums.ManStates.Running, 1, target);
                man.DoMovement(target);
            }
            else
                man.SetAnimation(Enums.ManStates.None, 4);
            return;
        }

        target = GridManager.Ref.GetWorldPositionFromGridIndex(RoomData.CoveredIndizes[man.ManData.AssignedRoomSlot]);
        if (man.transform.position != target)
        {
            man.SetAnimation(Enums.ManStates.Running, 2, target);
            man.DoMovement(target);
        }
        else
            man.SetAnimation(Enums.ManStates.None, 0);
    }

    private void AddRearIndizes()
    {
        GridIndex[] newIndizes = new GridIndex[RoomData.CoveredIndizes.Length * 2];
        Guid[] newSlots = new Guid[newIndizes.Length];
        for (int i = 0; i < RoomData.CoveredIndizes.Length; i++)
        {
            newSlots[i * 2] = Guid.Empty;
            newSlots[(i * 2) + 1] = Guid.Empty;
            newIndizes[i * 2] = RoomData.CoveredIndizes[i];
            newIndizes[(i * 2) + 1] = RoomData.CoveredIndizes[i].GetBack();
            GridManager.Ref.AddMovementDirectionToGridIndex(newIndizes[i * 2], Enums.MoveDirections.Back);
            GridManager.Ref.AddMovementDirectionToGridIndex(newIndizes[(i * 2) + 1], Enums.MoveDirections.Front);
            if (i > 0)
                GridManager.Ref.AddMovementDirectionToGridIndex(newIndizes[(i * 2) + 1], Enums.MoveDirections.Left);
            if (i < RoomData.CoveredIndizes.Length - 1)
                GridManager.Ref.AddMovementDirectionToGridIndex(newIndizes[(i * 2) + 1], Enums.MoveDirections.Right);
            if (RoomData.CoveredIndizes.Length > 1)
            {
                if (i > 0)
                {
                    GridManager.Ref.AddMovementDirectionToGridIndex(newIndizes[i * 2], Enums.MoveDirections.D_BackLeft);
                    GridManager.Ref.AddMovementDirectionToGridIndex(newIndizes[(i * 2) + 1], Enums.MoveDirections.D_FrontLeft);
                }
                    
                if (i < RoomData.CoveredIndizes.Length - 1)
                {
                    GridManager.Ref.AddMovementDirectionToGridIndex(newIndizes[i * 2], Enums.MoveDirections.D_BackRight);
                    GridManager.Ref.AddMovementDirectionToGridIndex(newIndizes[(i * 2) + 1], Enums.MoveDirections.D_FrontRight);

                }

            }
        }
        RoomData.CoveredIndizes = newIndizes;
    }

    private bool CheckFreeGuestSlot()
    {
        for(int i = 0; i < RoomData.ManSlotsAssignments.Length; i++)
        {
            if (RoomData.CoveredIndizes[i].Z != 0)
                continue;
            if (RoomData.ManSlotsAssignments[i] == Guid.Empty)
                return true;
        }
        return false;
    }

    private bool CheckFreeWorkerSlot()
    {
        for (int i = 0; i < RoomData.ManSlotsAssignments.Length; i++)
        {
            if (RoomData.CoveredIndizes[i].Z == 0)
                continue;
            if (RoomData.ManSlotsAssignments[i] == Guid.Empty)
                return true;
        }
        return false;
    }

    private ManScript_Guest GetGuestFromWorkerSlot(int s)
    {
        GridIndex index = RoomData.CoveredIndizes[s].GetFront();
        for (int i = 0; i < RoomData.CoveredIndizes.Length; i++)
        {
            if (RoomData.CoveredIndizes[i] == index)
                if (RoomData.ManSlotsAssignments[i] != Guid.Empty)
                    return ManManager.Ref.GetManData(RoomData.ManSlotsAssignments[i]).ManScript as ManScript_Guest;
        }
        return null;
    }

    private ManScript_Worker GetWorkerFromGuestSlot(int s)
    {
        GridIndex index = RoomData.CoveredIndizes[s].GetBack();
        for (int i = 0; i < RoomData.CoveredIndizes.Length; i++)
        {
            if (RoomData.CoveredIndizes[i] == index)
                if (RoomData.ManSlotsAssignments[i] != Guid.Empty)
                    return ManManager.Ref.GetManData(RoomData.ManSlotsAssignments[i]).ManScript as ManScript_Worker;
        }
        return null;
    }
}