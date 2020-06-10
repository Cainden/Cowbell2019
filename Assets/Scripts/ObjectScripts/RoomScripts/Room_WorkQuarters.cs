using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;

public abstract class Room_WorkQuarters : RoomScript
{
    public abstract System.Action<ManScript_Worker> GetRoleFunction { get; }

    public abstract MySpace.Stats.SpecialtyStat.StatType[] specialStatsUsed { get; }

    public override void AssignManToRoomSlot(System.Guid manId, int slotIndex, bool assignedByPlayer)
    {
        base.AssignManToRoomSlot(manId, slotIndex, assignedByPlayer);
        if (!assignedByPlayer)
            return;
        if (ManManager.Ref.IsManTypeOf<ManScript_Worker>(manId))
        {
            ManManager.Ref.GetManData(manId).ManScript.AddActionToEndOfMovement((ManManager.Ref.GetManData(manId).ManScript as ManScript_Worker).AssignRole);
            (ManManager.Ref.GetManData(manId).ManScript as ManScript_Worker).AddToDic(RoomRole, GetRoleFunction);
        }
    }
}
