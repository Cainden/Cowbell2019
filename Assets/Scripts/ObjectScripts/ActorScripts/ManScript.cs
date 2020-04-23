// Object script for general behaviour of one man avatar

using MySpace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ManScript : MonoBehaviour
{
    #region Variables
    #region Inspector Variables

    public string ManName;

    #endregion

    #region Public Variables
    // Instance specific data
    [HideInInspector] public ManInstanceData ManData;

    public Enums.ManStates State { get => state; protected set => state = value; }
    private Enums.ManStates state = Enums.ManStates.None;

    //Used by the manager to know what type of man it is before the man is instantiated.
    public virtual Enums.ManTypes ManType { get { return Enums.ManTypes.StandardMan; } }

    public Enums.ManRole role = Enums.ManRole.None;

    public MySpace.Stats.GeneralStat[] genStats;

    public abstract float GetNetRevenueCalculation { get; }
    public abstract RevenueInfo.RevenueType RevenueType { get; }
    #endregion

    #region Private Variables
    // Avatar movement
    protected List<ActionData> _ActionList = new List<ActionData>();

    protected Animator _Animator;



    #region Material Handling
    private Renderer[] _Renderers;
    private Material _MaterialNormal;
    private Material _MaterialHighlight; // Selected
    private Material _MaterialGhost;     // Leaving
    #endregion

    // Couroutines
    private IEnumerator WaitCoroutine;


    #endregion
    #endregion

    #region Mono Methods
    /// <summary>
    /// Call Base Start!
    /// </summary>
    protected virtual void Start()
    {
        _Animator = GetComponentInChildren<Animator>();
        _Renderers = GetComponentsInChildren<Renderer>();
        SetMaterials();
        CheckReferences();
        ManName = NameFactory.GetNewFirstName() + " " + NameFactory.GetNewLastName();
    }

    /// <summary>
    /// Call Base Update!
    /// </summary>
    protected virtual void Update()
    {
        StateUpdate();
    }

    protected virtual void OnEnable()
    {
        GameManager.NetRevenueCalculationEvent -= RevenueCalc;
        GameManager.NetRevenueCalculationEvent += RevenueCalc;
    }

    protected virtual void OnDisable()
    {
        GameManager.NetRevenueCalculationEvent -= RevenueCalc;
    }

    void RevenueCalc(List<RevenueInfo> list)
    {
        list.Add(new RevenueInfo(GetNetRevenueCalculation, RevenueType, ManData.ManId));
    }
    #endregion

    #region Helper Methods
    protected void CheckReferences()
    {
        Debug.Assert(ManData != null);
        Debug.Assert(_Animator != null);
        Debug.Assert(_Renderers != null);
        Debug.Assert(_MaterialNormal != null);
        Debug.Assert(_MaterialHighlight != null);
        Debug.Assert(_MaterialGhost != null);
    }

    private void SetMaterials()
    {
        _MaterialNormal = GetComponentInChildren<Renderer>().material;
        _MaterialHighlight = Resources.Load<Material>(Constants.ManSelectedMaterial);
        _MaterialGhost = Resources.Load<Material>(Constants.ManGhostMaterial);
        switch (ManType)
        {
            case Enums.ManTypes.Max: // Who is Max? lol
                break;
            case Enums.ManTypes.Monster:
                _MaterialNormal.color = Color.red;
                _MaterialGhost.color = Color.red;
                break;
            case Enums.ManTypes.None:
            case Enums.ManTypes.StandardMan:
                break;
            case Enums.ManTypes.Worker:
                _MaterialNormal.color = Color.blue;
                _MaterialGhost.color = Color.blue;
                break;
            case Enums.ManTypes.Guest:
                _MaterialNormal.color = Color.green;
                _MaterialGhost.color = Color.green;
                break;
            case Enums.ManTypes.MC: // MC is main character? not sure what this one is either.
                break;
            default:
                break;
        }
    }

    public virtual Sprite GetSprite()
    {
        Debug.LogWarning("GetSprite() is still returning null! Men do not have sprites yet!");
        return null;
    }
    #endregion

    #region State Methods
    public void SetSelectedState(bool selected)
    {
        if (selected)
            foreach (Renderer r in _Renderers) r.material = _MaterialHighlight;
        else
            foreach (Renderer r in _Renderers) r.material = _MaterialNormal;
    }

    public void SetGhostState()
    {
        GetComponent<BoxCollider>().enabled = false; // Disable raycasting
        foreach (Renderer r in _Renderers) r.material = _MaterialGhost;
    }

    protected void StateUpdate()
    {
        switch (State)
        {
            case Enums.ManStates.Running: DoMovement(Constants.ManRunSpeed); break;
            case Enums.ManStates.RotatingToPlayer: FaceTowardsPlayer(); break;
            case Enums.ManStates.Rotating: RotateToOrientation(); break;
            case Enums.ManStates.Idle:
            case Enums.ManStates.None: ProcessActions(); break;
        }
    }

    private void SetFaceTowardsPlayer()
    {
        State = Enums.ManStates.RotatingToPlayer;
        SetAnimation(State);
    }

    protected void SetAnimation(Enums.ManStates state)
    {
        switch (state)
        {
            case Enums.ManStates.Idle:
            case Enums.ManStates.Waiting:
                if (!_Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                    _Animator.SetTrigger("IdleTrigger");
                break;
            case Enums.ManStates.Running:
            case Enums.ManStates.Rotating:
            case Enums.ManStates.RotatingToPlayer:
                if (!_Animator.GetCurrentAnimatorStateInfo(0).IsName("Running"))
                    _Animator.SetTrigger("RunningTrigger");
                break;
        }
    }

    protected void SetMoveToPosition(Enums.ManStates state, Vector3 position)
    {
        State = state;
        _TargetPos = position;
        SetAnimation(State);
    }

    protected void SetRotateToOrientation(Enums.ManStates state, Quaternion rotation)
    {
        State = state;
        _TargetRot = rotation;
        //SetAnimation(state);
    }

    #region State Functionality Methods

    protected Vector3 _TargetPos;
    private void DoMovement(float movementSpeed)
    {
        float Distance = Vector3.Distance(transform.position, _TargetPos);
        float Travel = movementSpeed * Time.deltaTime;

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

    private void FaceTowardsWaypoint(Vector3 deltaPos)
    {
        deltaPos.y = 0.0f;
        if (deltaPos.magnitude == 0) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(deltaPos), 0.15f);
    }

    private void FaceTowardsPlayer()
    {
        Quaternion TargetRotation = Quaternion.Euler(0, 180, 0);

        transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, 0.10f);

        if (Mathf.Abs(transform.rotation.eulerAngles.y - TargetRotation.eulerAngles.y) < 1.0f)
        {
            transform.rotation = TargetRotation;
            State = Enums.ManStates.None; // Will trigger next action
        }
    }

    protected Quaternion _TargetRot;
    private void RotateToOrientation()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, _TargetRot, 0.10f);

        if (Mathf.Abs(transform.rotation.eulerAngles.y - _TargetRot.eulerAngles.y) < 1.0f)
        {
            transform.rotation = _TargetRot;
            State = Enums.ManStates.None; // Will trigger next action
        }
    }

    #endregion

    #endregion

    #region Room Helper Functions
    public void AssignToRoom(Guid assignedRoom, int assignedRoomSlot)
    {
        SetOwnerOfRoom(assignedRoom);
        //ManData.AssignedRoom = assignedRoom;
        ManData.AssignedRoom = RoomManager.Ref.GetRoomData(assignedRoom)?.RoomScript;
        ManData.AssignedRoomSlot = assignedRoomSlot;
    }

    public virtual void SetOwnerOfRoom(Guid assignedRoom)
    {

        if (assignedRoom == Guid.Empty)
        {
            RemoveRoomOwnership();
            return;
        }
        //Temporary set up for setting room ownership
        //SET ownership of room only if
        //Man is of type Guest
        //Room is of type Bedroom
        //Owner does NOT own a room already
    }

    public virtual void TransferOwnershipToNewRoom(Guid newRoom)
    {


        if (newRoom == Guid.Empty)
        {
            RemoveRoomOwnership();
            return;
        }
        //else
        //{
        //    RoomManager.Ref.GetRoomData(newRoom).RoomScript.AssignOwnerToRoomSlot(ManData.ManId, RoomManager.Ref.GetRoomData(newRoom).RoomScript.GetFreeOwnerSlotIndex());
        //}
    }

    public void RemoveRoomOwnership()
    {
        if (ManData.OwnedRoomRef != null)
        {
            for (int i = 0; i < ManData.OwnedRoomRef.OwnerSlotsAssignments.Length; i++)
            {
                if (ManData.OwnedRoomRef.OwnerSlotsAssignments[i] == ManData.ManId)
                {
                    ManData.OwnedRoomRef.OwnerSlotsAssignments[i] = Guid.Empty;
                    break;
                }
            }
        }

        ManData.OwnedRoomRef = null;
    }

    public bool IsAssignedToAnyRoom()
    {
        //return (ManData.AssignedRoom != Guid.Empty);
        return ManData.AssignedRoom != null;
    }

    public bool IsOwnerOfRoom()
    {
        return (ManData.OwnedRoomRef != null);
    }
    #endregion

    #region Queue Methods
    protected void ProcessActions()
    {
        if (_ActionList.Count == 0) return;
        _ActionList[0].ActionItem.Invoke();
        _ActionList.RemoveAt(0);
    }

    public void Add_AccessAction_ToList(Guid roomId)
    {
        _ActionList.Add(new ActionData(() => StartCoroutine(WaitForRoomAccess(roomId)), ActionData.ActionType.Wait));
    }

    public void Add_RunAction_ToList(Vector3 position)
    {
        _ActionList.Add(new ActionData(() => SetMoveToPosition(Enums.ManStates.Running, position), ActionData.ActionType.Movement));
    }

    public void Add_FacePlayerAction_ToList()
    {
        _ActionList.Add(new ActionData(() => SetFaceTowardsPlayer(), ActionData.ActionType.Movement));
    }

    public void Add_RotateAction_ToList(Quaternion rotation)
    {
        _ActionList.Add(new ActionData(() => SetRotateToOrientation(Enums.ManStates.Rotating, rotation), ActionData.ActionType.Movement));
    }

    public void Add_IdleAction_ToList()
    {
        _ActionList.Add(new ActionData(() => SetAnimation(Enums.ManStates.Idle), ActionData.ActionType.Animation));
    }

    // The working state can be of different animations, dependent on room (idle, working, ...)
    public void Add_WorkingAction_ToList(Enums.ManStates manState)
    {
        _ActionList.Add(new ActionData(() => SetAnimation(manState), ActionData.ActionType.Animation));
    }

    public void Add_DoorOpenAction_ToList(Guid roomId)
    {
        //Changed this lambda in preparation to set up the "line" system for elevators.
        _ActionList.Add(new ActionData(() => 
        {
            (RoomManager.Ref.GetRoomData(roomId).RoomScript as Room_Elevator).SetAnimation_OpenDoor(true);
        }, ActionData.ActionType.Elevator));
    }

    public void Add_DoorCloseAction_ToList(Guid roomId)
    {
        _ActionList.Add(new ActionData(() => (RoomManager.Ref.GetRoomData(roomId).RoomScript as Room_Elevator).SetAnimation_CloseDoor(CheckElevatorActionQueue(ActionData.ActionType.Elevator)), ActionData.ActionType.Elevator));
    }

    public void Add_SelfDestruction_ToList()
    {
        _ActionList.Add(new ActionData(() => Destroy(gameObject), ActionData.ActionType.Die));
    }

    public void Add_WaitTime_ToList(float seconds)
    {
        WaitCoroutine = WaitForSeconds(seconds);
        _ActionList.Add(new ActionData(() => StartCoroutine(WaitCoroutine), ActionData.ActionType.Wait));
    }

    private IEnumerator WaitForSeconds(float seconds)
    {
        State = Enums.ManStates.Waiting;
        SetAnimation(State);
        yield return new WaitForSeconds(seconds);
        State = Enums.ManStates.None; // Will trigger next action
    }

    private IEnumerator WaitForRoomAccess(Guid room)
    {
        State = Enums.ManStates.Waiting;
        SetAnimation(State);
        RoomScript r = RoomManager.Ref.GetRoomData(room).RoomScript;
        yield return new WaitUntil(() => r.GetAccessRequest());
        State = Enums.ManStates.None; // Will trigger next action
    }

    public void Add_Action_ToList(ActionData action)
    {
        _ActionList.Add(action);
    }

    /// <summary>
    /// Checks if the given action type is in the action queue up to the given number
    /// </summary>
    /// <param name="typeCheck"></param>
    /// <param name="actionsToCheck"></param>
    /// <returns>true if there is an action of type <c>typeCheck</c> within the next <c>actionsToCheck</c> actions</returns>
    private bool CheckElevatorActionQueue(ActionData.ActionType typeCheck, int actionsToCheck = 4)
    {
        //Make sure there is another action after the current one
        if (_ActionList.Count > actionsToCheck)
        {
            for (int i = 0; i < actionsToCheck; i++)
            {
                if (_ActionList[i].ActionMethod == typeCheck)
                    return true;
            }
            return false;
        }
        else return false;
    }
    #endregion


}
