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

    public float physicality = 1;
    public float intelligence = 1;
    public float professionalism = 1;

    #endregion
    /// <summary>
    /// Affects the salary of the worker for their given role. Also makes them more likely to stay if they are unhappy.
    /// </summary>
    public float loyalty = 1;

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
        loyalty = 1;
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
            PayWorkerInHoots("Worker Payment", Mathf.FloorToInt(loyalty * GameManager.GetRoleInfo(role).income));
            hasBeenPaid = true;
        }
        else if (TimeManager.Ref.worldTimeHour != 8)
        {
            hasBeenPaid = false;
        }
    }
    #endregion
}
