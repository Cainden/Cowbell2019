using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;
using System;

public class ManScript_Worker : ManScript
{
    #region Variables
    public override Enums.ManTypes ManType { get { return Enums.ManTypes.Worker; } }

    #region Public Variables

    public float tirednessThreshhold; //tiredness always increases by 1 per second, so a default of 1 minute seems decent.
    public float currentTiredness = 0;

    #region Base Stats
    //public float physicality = 1;
    //public float intelligence = 1;
    //public float professionalism = 1;

    public SpecialtyStat[] specialStats;



    #endregion
    /// <summary>
    /// Affects the salary of the worker for their given role. Also makes them more likely to stay if they are unhappy.
    /// </summary>
    //public float loyalty = 1;

    //public float speed = 1;

    //public GeneralStat[] genStats; //This was moved to the base ManScript class.
    public float delayTimer;

    #endregion

    #region Private Variables

    private Dictionary<Enums.ManRole, Action<ManScript_Worker>> States = new Dictionary<Enums.ManRole, Action<ManScript_Worker>>();

    #endregion

    #endregion

    protected override void Start()
    {
        base.Start();
        States.Add(Enums.ManRole.None, Idle);
        States.Add(Enums.ManRole.Guest, Idle);
        tirednessThreshhold = 60;
        currentTiredness = 60;
        delayTimer = 0;
    }

    protected override void Update()
    {
        base.Update();
        if (State == Enums.ManStates.None) //Might need to fix this later somehow, only doing the states when the State is none seems dangerous for future development
        {
            States[role]?.Invoke(this);
        }
        
    }

    public void AssignRole()
    {
        //role = RoomManager.Ref.GetRoomData(ManData.AssignedRoom).RoomScript.RoomRole;

        if (ManData.AssignedRoom != null)
            role = ManData.AssignedRoom.RoomRole;
    }

    #region Role State Functions
    private void Idle(ManScript_Worker man)
    {
        //Do nothing! (for now. Add some animations of some sort here for idling later)
    }
    #endregion

    #region HelperFunctions

    public void AddToDic(Enums.ManRole role, Action<ManScript_Worker> function)
    {
        if (!States.ContainsKey(role))
            States.Add(role, function);
    }

    #endregion

    #region Pay Methods

    private bool hasBeenPaid = false;

    public void PayWorkerInHoots(string reason, int amount)
    {
        OverheadTextManager.Ref.OverheadHoots(amount.ToString(), transform.position);
        if (WalletManager.SubtractHoots(amount))
        {
            //Wallet had the amount to pay
        }
        else
        {
            //Wallet did NOT have the amount to pay
        }
    }

    private void CheckIfRentTime()
    {
        if (role != Enums.ManRole.None && !hasBeenPaid && TimeManager.Ref.worldTimeHour == 8)
        {
            PayWorkerInHoots("Worker Payment", Mathf.FloorToInt(genStats.GetGeneralStat(GeneralStat.StatType.Loyalty).value * GameManager.GetRoleInfo(role).income));
            hasBeenPaid = true;
        }
        else if (TimeManager.Ref.worldTimeHour != 8)
        {
            hasBeenPaid = false;
        }
    }
    #endregion


    #region Stats

    #region Stat Helper Functions

    public float GetGeneralStatValue(GeneralStat.StatType type)
    {
        foreach (GeneralStat s in genStats)
        {
            if (s.statType == type)
                return s.value;
        }
        Debug.LogWarning("False Value returned. The type '" + type + "' was not found in the genStats Array.");
        return -1;
    }

    public GeneralStat GetGeneralStat(GeneralStat.StatType type)
    {
        foreach (GeneralStat s in genStats)
        {
            if (s.statType == type)
                return s;
        }
        Debug.LogWarning("Null Stat returned. The type '" + type + "' was not found in the genStats Array.");
        return null;
    }

    public float GetSpecialtyStatValue(SpecialtyStat.StatType type)
    {
        foreach (SpecialtyStat s in specialStats)
        {
            if (s.statType == type)
                return s.value;
        }
        Debug.LogWarning("False Value returned. The type '" + type + "' was not found in the specialStats Array.");
        return -1;
    }

    public SpecialtyStat GetSpecialtyStat(SpecialtyStat.StatType type)
    {
        foreach (SpecialtyStat s in specialStats)
        {
            if (s.statType == type)
                return s;
        }
        Debug.LogWarning("Null Stat returned. The type '" + type + "' was not found in the specialStats Array.");
        return null;
    }

    #endregion

    public abstract class Stat
    {
        public const float StatMax = 10;

        public string name;
        public string Description;
        public float value;
    }

    public class GeneralStat : Stat
    {
        public enum StatType : byte
        {
            Loyalty,
            Speed,
            Dirtiness
        }

        public StatType statType;
    }

    public class SpecialtyStat : Stat
    {
        public enum StatType : byte
        {
            Professionalism,
            Physicality,
            Intelligence
        }

        public StatType statType;
    }
    #endregion
}

namespace MySpace
{
    public static class StatExtensions
    {
        public static ManScript_Worker.SpecialtyStat GetSpecialtyStat(this ManScript_Worker.SpecialtyStat[] ar, ManScript_Worker.SpecialtyStat.StatType type)
        {
            foreach (ManScript_Worker.SpecialtyStat s in ar)
            {
                if (type == s.statType)
                    return s;
            }
            Debug.LogWarning("Specialty Stat type '" + type + "', was not found in the given array!!");
            return null;
        }

        public static ManScript_Worker.GeneralStat GetGeneralStat(this ManScript_Worker.GeneralStat[] ar, ManScript_Worker.GeneralStat.StatType type)
        {
            foreach (ManScript_Worker.GeneralStat s in ar)
            {
                if (type == s.statType)
                    return s;
            }
            Debug.LogWarning("General Stat type '" + type + "', was not found in the given array!!");
            return null;
        }
    }
}
