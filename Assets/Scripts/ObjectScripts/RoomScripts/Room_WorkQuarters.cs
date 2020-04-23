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
            ManManager.Ref.GetManData(manId).ManScript.Add_Action_ToList(new ActionData((ManManager.Ref.GetManData(manId).ManScript as ManScript_Worker).AssignRole, ActionData.ActionType.RoleAssignment));
            (ManManager.Ref.GetManData(manId).ManScript as ManScript_Worker).AddToDic(RoomRole, GetRoleFunction);
        }
    }
}
