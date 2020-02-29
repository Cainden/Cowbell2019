using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;
using System;

/// <summary>
/// A class to be inherented by all rooms that can have workers assigned to them.
/// </summary>
public abstract class Room_WorkerQuarters : RoomScript
{
    public abstract Enums.ManRole RoomRole { get; }

    public override void AssignManToRoomSlot(Guid manId, int slotIndex, bool assignedByPlayer)
    {
        base.AssignManToRoomSlot(manId, slotIndex, assignedByPlayer);
        if (!assignedByPlayer)
            return;
        if (ManManager.Ref.GetManData(manId).ManScript.ManType == Enums.ManTypes.Worker)
        {
            ManManager.Ref.GetManData(manId).ManScript.Add_Action_ToList(new ActionData((ManManager.Ref.GetManData(manId).ManScript as ManScript_Worker).AssignRole));
            (ManManager.Ref.GetManData(manId).ManScript as ManScript_Worker).AddToDic(RoomRole, RoleBehavior);
        }
        
    }

    protected abstract void RoleBehavior(ManScript_Worker man);
}
