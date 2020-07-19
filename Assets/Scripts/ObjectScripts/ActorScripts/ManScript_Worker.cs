using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;
using System;
using MySpace.Stats;

public class ManScript_Worker : ManScript
{
    #region Variables
    public override Enums.ManTypes ManType { get { return Enums.ManTypes.Worker; } }

    #region Public Variables

    public const float tirednessMax = 60; //tiredness always increases by 1 per second, so a default of 1 minute seems decent.
    public float currentTiredness = 0;

    #region Base Stats
    //public float physicality = 1;
    //public float intelligence = 1;
    //public float professionalism = 1;

    public SpecialtyStat[] specialStats;

    public override float GetNetRevenueCalculation
    {
        get
        {
            if (role == Enums.ManRole.None)
                return 0;
            else
            {
                return -GameManager.GetRoleSalary(role, genStats.GetGeneralStat(GeneralStat.StatType.Loyalty).value);
            }
        }
    }

    public override RevenueInfo.RevenueType RevenueType => RevenueInfo.RevenueType.Worker;

    #endregion
    //public float loyalty = 1;

    //public float speed = 1;

    //public GeneralStat[] genStats; //This was moved to the base ManScript class.
    //public float delayTimer;

    #endregion

    #region Private Variables

    private static Dictionary<Enums.ManRole, Action<ManScript_Worker>> States = new Dictionary<Enums.ManRole, Action<ManScript_Worker>>();

    #endregion

    #endregion

    protected override void Start()
    {
        base.Start();
        if (!States.ContainsKey(Enums.ManRole.None))
            States.Add(Enums.ManRole.None, Idle);
        if (!States.ContainsKey(Enums.ManRole.Guest))
            States.Add(Enums.ManRole.Guest, Idle);
        currentTiredness = 60;
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
    private static void Idle(ManScript_Worker man)
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
            PayWorkerInHoots("Worker Payment", GameManager.GetRoleSalary(role, genStats.GetGeneralStat(GeneralStat.StatType.Loyalty).value));
            hasBeenPaid = true;
        }
        else if (TimeManager.Ref.worldTimeHour != 8)
        {
            hasBeenPaid = false;
        }
    }
    #endregion


    
    #region Stat Helper Functions

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
}

#region stats
namespace MySpace
{
    namespace Stats
    {
        public abstract class Stat
        {
            public const float StatMax = 10;

            public abstract string Name { get; }
            public float value;
            public abstract Sprite GetSprite { get; }
        }

        [Serializable]
        public class GeneralStat : Stat
        {
            public enum StatType : byte
            {
                Loyalty,
                Speed,
                Dirtiness
            }

            public StatType statType;

            public string Desc
            {
                get
                {
                    switch (statType)
                    {
                        case StatType.Loyalty:
                            return "This value affects the salary of the worker. A higher value means he doesn't need to be paid as much.";
                        case StatType.Speed:
                            return "How quickly this worker moves.";
                        case StatType.Dirtiness:
                            return "How quickly this guest dirties any room they stay in.";
                        default:
                            Debug.LogWarning("There is no description written for GeneralStat.StatType of type '" + statType + "'!");
                            return "";
                    }
                }
            }

            public override string Name { get { return statType.ToString(); } }

            public override Sprite GetSprite
            {
                get
                {
                    Debug.LogWarning("General stats still automatically return null when asked for their sprite!");
                    //Will want to use resources here to load specific sprites for each stat type
                    switch (statType)
                    {
                        case StatType.Loyalty:
                            return null;
                        case StatType.Speed:
                            return null;
                        case StatType.Dirtiness:
                            return null;
                        default:
                            Debug.LogWarning("There is no sprite referenced in resources for GeneralStat.StatType of type '" + statType + "'!");
                            return null;
                    }
                }
            }
        }

        [Serializable]
        public class SpecialtyStat : Stat
        {
            public enum StatType : byte
            {
                Professionalism,
                Physicality,
                Intelligence
            }

            public StatType statType;

            public string Desc
            {
                get
                {
                    switch (statType)
                    {
                        case StatType.Professionalism:
                            return "How effective the worker is at being interactable with guests.";
                        case StatType.Physicality:
                            return "How good the worker is at physical tasks.";
                        case StatType.Intelligence:
                            return "How good the worker is at non-physical tasks.";
                        default:
                            Debug.LogWarning("There is no description written for SpecialtyStat.StatType of type '" + statType + "'!");
                            return "";
                    }
                }
            }

            public float experience;

            public float GetCurrentExp { get { return IsStatMaxed ? -1 : experience; } }

            public int changedValue = 0;

            public override string Name { get { return statType.ToString(); } }

            public bool IsStatMaxed { get { return (changedValue + base.value) >= StatMax; } }

            //Hide the base value, since we want the modified value to be used for specialty stats
            public new float value { get { return changedValue + base.value; } }
            //Allow the hidden base value to be accessable to make sure you're getting the number you want and not the modified value
            public float BaseValue { get { return base.value; } set { base.value = value; } }

            public override Sprite GetSprite
            {
                get
                {
                    Debug.LogWarning("Specialty stats still automatically return null when asked for their sprite!");
                    //Will want to use resources here to load specific sprites for each stat type
                    switch (statType)
                    {
                        case StatType.Professionalism:
                            return null;
                        case StatType.Physicality:
                            return null;
                        case StatType.Intelligence:
                            return null;
                        default:
                            Debug.LogWarning("There is no sprite referenced in resources for SpecialtyStat.StatType of type '" + statType + "'!");
                            return null;
                    }
                }
            }
        }
    }
    
}
#endregion