// Object script for general behaviour of one man avatar

using MySpace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManScript : MonoBehaviour
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
    #endregion

    #region Private Variables
    // Avatar movement
    protected Queue<ActionData> _ActionList = new Queue<ActionData>();

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
    }

    /// <summary>
    /// Call Base Update!
    /// </summary>
    protected virtual void Update()
    {
        StateUpdate();
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
            case Enums.ManStates.RotatingToPlayer:
            case Enums.ManStates.Waiting:
                if (!_Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                    _Animator.SetTrigger("IdleTrigger");
                break;
            case Enums.ManStates.Running:
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
        //SetAnimation(_State);
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
        _ActionList.Dequeue().ActionItem.Invoke();
    }

    public void Add_AccessAction_ToList(Guid roomId)
    {
        _ActionList.Enqueue(new ActionData(() => StartCoroutine(WaitForRoomAccess(roomId))));
    }

    public void Add_RunAction_ToList(Vector3 position)
    {
        _ActionList.Enqueue(new ActionData(() => SetMoveToPosition(Enums.ManStates.Running, position)));
    }

    public void Add_FacePlayerAction_ToList()
    {
        _ActionList.Enqueue(new ActionData(() => SetFaceTowardsPlayer()));
    }

    public void Add_RotateAction_ToList(Quaternion rotation)
    {
        _ActionList.Enqueue(new ActionData(() => SetRotateToOrientation(Enums.ManStates.Rotating, rotation)));
    }

    public void Add_IdleAction_ToList()
    {
        _ActionList.Enqueue(new ActionData(() => SetAnimation(Enums.ManStates.Idle)));
    }

    // The working state can be of different animations, dependent on room (idle, working, ...)
    public void Add_WorkingAction_ToList(Enums.ManStates manState)
    {
        _ActionList.Enqueue(new ActionData(() => SetAnimation(manState)));
    }

    public void Add_DoorOpenAction_ToList(Guid roomId)
    {
        //Changed this lambda in preparation to set up the "line" system for elevators.
        _ActionList.Enqueue(new ActionData(() => 
        {
            RoomManager.Ref.GetRoomData(roomId).RoomObject.GetComponent<RoomElevatorAnimation>().SetAnimation_OpenDoor();
        }));
    }

    public void Add_DoorCloseAction_ToList(Guid roomId)
    {
        _ActionList.Enqueue(new ActionData(() => RoomManager.Ref.GetRoomData(roomId).RoomObject.GetComponent<RoomElevatorAnimation>().SetAnimation_CloseDoor()));
    }

    public void Add_SelfDestruction_ToList()
    {
        _ActionList.Enqueue(new ActionData(() => Destroy(gameObject)));
    }

    public void Add_WaitTime_ToList(float seconds)
    {
        WaitCoroutine = WaitForSeconds(seconds);
        _ActionList.Enqueue(new ActionData(() => StartCoroutine(WaitCoroutine)));
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
        _ActionList.Enqueue(action);
    }
    #endregion

    
}
