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

    public float cleaningEfficieny = 1;

    public float raiseValue = 1;

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
        raiseValue = 1;
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
            role = (ManData.AssignedRoom as Room_WorkerQuarters)?.RoomRole ?? Enums.ManRole.None;
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
        WalletManager.Ref.Hoots -= amount;
    }

    private void CheckIfRentTime()
    {
        if (role != Enums.ManRole.None && !hasBeenPaid && TimeManager.Ref.worldTimeHour == 8)
        {
            PayWorkerInHoots("Worker Payment", Mathf.FloorToInt(raiseValue * GameManager.GetRoleInfo(role).income));
            hasBeenPaid = true;
        }
        else if (TimeManager.Ref.worldTimeHour != 8)
        {
            hasBeenPaid = false;
        }
    }
    #endregion
}
