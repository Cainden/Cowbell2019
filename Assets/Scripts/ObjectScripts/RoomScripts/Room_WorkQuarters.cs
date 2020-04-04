using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;

public abstract class Room_WorkQuarters : RoomScript
{
    public abstract System.Action<ManScript_Worker> GetRoleFunction { get; }

    [Tooltip("Purely for UI display purposes, this is to be set to return any/all BaseStatTypes used by the work quarters from workers to do their jobs.")]
    /// <summary>
    /// Purely for UI display purposes, this is to be set to return any/all BaseStatTypes used by the work quarters from workers to do their jobs.
    /// </summary>
    public ManScript_Worker.SpecialtyStat.StatType[] specialStatsUsed;

    public override void AssignManToRoomSlot(System.Guid manId, int slotIndex, bool assignedByPlayer)
    {
        base.AssignManToRoomSlot(manId, slotIndex, assignedByPlayer);
        if (!assignedByPlayer)
            return;
        if (ManManager.Ref.GetManData(manId).ManScript.ManType == Enums.ManTypes.Worker)
        {
            ManManager.Ref.GetManData(manId).ManScript.Add_Action_ToList(new ActionData((ManManager.Ref.GetManData(manId).ManScript as ManScript_Worker).AssignRole));
            (ManManager.Ref.GetManData(manId).ManScript as ManScript_Worker).AddToDic(RoomRole, GetRoleFunction);
        }
    }
}
