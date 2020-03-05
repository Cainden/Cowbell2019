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

    public float raiseValue = 1;

    public float delayTimer;

    #endregion

    #region Private Variables

    private Dictionary<Enums.ManRole, Action<ManScript_Worker>> States = new Dictionary<Enums.ManRole, Action<ManScript_Worker>>();

    #endregion

    #region Stats

    private float speed = 1;
    public float Speed
    {
        get { return speed; }
        set
        {
            //General stats won't use the specialty stat max and minimum because general stats might vary greatly in number size (it is currently equivalent but that may change)
            speed = Mathf.Clamp(value, 1, 10);
        }
    }

    #region Specialty Stats

    private int intelligence = 1;
    public int Intelligence
    {
        get { return intelligence; }
        set
        {
            intelligence = Mathf.Clamp(value, GameManager.StatMinimum, GameManager.StatMaximum);
        }
    }

    private int physicality = 1;
    public int Physicality
    {
        get { return physicality; }
        set
        {
            physicality = Mathf.Clamp(value, GameManager.StatMinimum, GameManager.StatMaximum);
        }
    }

    private int professionalism = 1;
    public int Professionalism
    {
        get { return professionalism; }
        set
        {
            professionalism = Mathf.Clamp(value, GameManager.StatMinimum, GameManager.StatMaximum);
        }
    }

    #endregion

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

    public override void InitializeStats(StatDeviation statDev)
    {
        //Gotta calculate the standard deviation based on the inputted values from the gamemanager
        //float roll = 
        Speed = 1;
    }

    #endregion

    protected override void DoMovement()
    {
        float Distance = Vector3.Distance(transform.position, _TargetPos);
        float Travel = (Constants.ManRunSpeed + (Speed * 0.1f)) * Time.deltaTime;

        if (Travel > Distance) // Target reached
        {
            transform.position = _TargetPos;
            State = Enums.ManStates.None; // Will trigger next action
            return;
        }
        else // Regular movement
        {
            Vector3 DeltaPos = (_TargetPos - transform.position);
            DeltaPos.Normalize();
            transform.position += (DeltaPos * Travel);
            FaceTowardsWaypoint(DeltaPos);
        }
    }

    protected override void FaceTowardsPlayer()
    {
        Quaternion TargetRotation = Quaternion.Euler(0, 180, 0);

        transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, 0.10f);

        if (Mathf.Abs(transform.rotation.eulerAngles.y - TargetRotation.eulerAngles.y) < 1.0f)
        {
            transform.rotation = TargetRotation;
            State = Enums.ManStates.None; // Will trigger next action
        }
    }

    protected override void RotateToOrientation()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, _TargetRot, 0.10f);

        if (Mathf.Abs(transform.rotation.eulerAngles.y - _TargetRot.eulerAngles.y) < 1.0f)
        {
            transform.rotation = _TargetRot;
            State = Enums.ManStates.None; // Will trigger next action
        }
    }

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
