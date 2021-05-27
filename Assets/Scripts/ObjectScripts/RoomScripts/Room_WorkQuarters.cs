using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;

public abstract class Room_WorkQuarters : RoomScript
{
    public enum WorkQuartersType { Utilities, Hunger, Entertainment }

    public abstract System.Action<ManScript_Worker> GetRoleFunction { get; }

    public abstract MySpace.Stats.SpecialtyStat.StatType[] specialStatsUsed { get; }

    public abstract WorkQuartersType WorkQuarterType { get; }

    public abstract CharacterSwaper.CharLabel Outfit { get; }

    public override void AssignManToRoomSlot(System.Guid manId, int slotIndex, bool assignedByPlayer)
    {
        base.AssignManToRoomSlot(manId, slotIndex, assignedByPlayer);
        if (!assignedByPlayer)
            return;
        ManScript_Worker worker = ManManager.Instance.GetManData<ManScript_Worker>(manId).ManScript;
        if (worker != null)
        {
            //ManManager.Instance.GetManData(manId).ManScript.AddActionToEndOfMovement((ManManager.Instance.GetManData(manId).ManScript as ManScript_Worker).AssignRole);
            worker.AssignRole();
            worker.AddToDic(RoomRole, GetRoleFunction);
            worker.SetCharacterSprites(Outfit);
        }
    }
}
