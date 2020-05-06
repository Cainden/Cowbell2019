// Object script for general behaviour of one man avatar

using MySpace;
using MySpace.Stats;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ManScript : MonoBehaviour
{
    #region Variables
    #region Inspector Variables

    public string ManName;

    [SerializeField] GameObject MeshParent;

    #endregion

    #region Public Variables
    // Instance specific data
    [HideInInspector] public ManInstanceData ManData;

    public Enums.ManStates State { get => state; protected set => state = value; }
    private Enums.ManStates state = Enums.ManStates.None;

    //Used by the manager to know what type of man it is before the man is instantiated.
    public virtual Enums.ManTypes ManType { get { return Enums.ManTypes.StandardMan; } }

    public Enums.ManRole role = Enums.ManRole.None;

    [HideInInspector]
    public GeneralStat[] genStats;

    public abstract float GetNetRevenueCalculation { get; }
    public abstract RevenueInfo.RevenueType RevenueType { get; }
    #endregion

    #region Private Variables
    // Avatar movement
    protected List<ActionData> _ActionList = new List<ActionData>();

    //protected Animator _Animator;
    [SerializeField] protected Animator animator;

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
        //_Animator = GetComponentInChildren<Animator>();
        _Renderers = GetComponentsInChildren<Renderer>();
        SetMaterials();
        CheckReferences();
        ManName = NameFactory.GetNewFirstName() + " " + NameFactory.GetNewLastName();
        animator.speed = (1 + (GetGeneralStatValue(GeneralStat.StatType.Speed) * 0.1f)) * 2;
    }

    /// <summary>
    /// Call Base Update!
    /// </summary>
    protected virtual void Update()
    {
        StateUpdate();
        //print(GetCurrAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name);
    }

    #region Net Revenue Calculation
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
    #endregion

    #region Helper Methods
    protected void CheckReferences()
    {
        Debug.Assert(ManData != null);
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

    private void SetAnimatorRotation(Vector3 currDirection)
    {
        if (currDirection.x > 0)
        {
            if (MeshParent.transform.rotation.eulerAngles.y != 0)
                MeshParent.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (currDirection.x < 0)
        {
            if (MeshParent.transform.rotation.eulerAngles.y != 180)
                MeshParent.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

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
            //case Enums.ManStates.RotatingToPlayer: FaceTowardsPlayer(); break;
            //case Enums.ManStates.Rotating: RotateToOrientation(); break;
            case Enums.ManStates.Idle:
            case Enums.ManStates.None: ProcessActions(); break;
        }
    }

    private void SetFaceTowardsPlayer()
    {
        FaceTowardsPlayer();
        SetAnimation(State, 2);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="state"></param>
    /// <param name="dir">0 = noChange, 1 = forward, 2 = backward, 3 = left, 4 = right. Any other number automatically assigns the direction</param>
    protected void SetAnimation(Enums.ManStates state, int dir)
    {
        switch (dir)
        {
            case 0:
                SetAnimatorRotation(Vector3.zero);
                break;
            case 1:
                SetAnimatorRotation(Vector3.forward);
                break;
            case 2:
                SetAnimatorRotation(Vector3.back);
                break;
            case 3:
                SetAnimatorRotation(Vector3.left);
                break;
            case 4:
                SetAnimatorRotation(Vector3.right);
                break;
            default:
                SetAnimatorRotation(_TargetPos - transform.position);
                break;
        }

        switch (state)
        {
            case Enums.ManStates.Idle:
            case Enums.ManStates.Waiting:
                animator.SetTrigger("IdleTrigger");
                break;
            case Enums.ManStates.Running:
                CheckMovementDir((_TargetPos - transform.position).normalized);
                break;
        }

        void CheckMovementDir(Vector3 d)
        {
            if (d.y > 0)
            {
                if (d.x + d.z < d.y * 0.1f)
                    //Might want a flying animation here eventually if we end up doing that, or a specific animation for being in an elevator?
                    animator.SetTrigger("IdleTrigger");
                else
                    goto Run;
            }
            else
            {
                goto Run;
            }
            return;

        Run:
            animator.SetTrigger("RunningTrigger");
        }
    }

    

    protected void SetMoveToPosition(Enums.ManStates state, Vector3 position)
    {
        State = state;
        _TargetPos = position;
        SetAnimation(State, -1);
    }

    //protected void SetRotateToOrientation(Enums.ManStates state, Quaternion rotation)
    //{
    //    State = state;
    //    _TargetRot = rotation;
    //    //SetAnimation(state);
    //}

    #region State Functionality Methods

    protected Vector3 _TargetPos;
    private void DoMovement(float movementSpeed)
    {
        float Travel = (movementSpeed + (GetGeneralStatValue(GeneralStat.StatType.Speed) * 0.1f)) * Time.deltaTime;

        if (Travel > Vector3.Distance(transform.position, _TargetPos)) // Target reached
        {
            transform.position = _TargetPos;
            State = Enums.ManStates.None; // Will trigger next action
            return;
        }
        else // Regular movement
        {
            Vector3 dir = (_TargetPos - transform.position);
            dir.Normalize();
            transform.position += (dir * Travel);
            //SetAnimation(Enums.ManStates.Running, -1);
        }
    }

    //private void FaceTowardsWaypoint(Vector3 deltaPos)
    //{
    //    deltaPos.y = 0.0f;
    //    if (deltaPos.magnitude == 0) return;
    //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(deltaPos), 0.15f);
    //}

    private void FaceTowardsPlayer()
    {
        //Quaternion TargetRotation = Quaternion.Euler(0, 180, 0);

        //transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, 0.10f);

        //if (Mathf.Abs(transform.rotation.eulerAngles.y - TargetRotation.eulerAngles.y) < 1.0f)
        //{
        //    transform.rotation = TargetRotation;
        //    State = Enums.ManStates.None; // Will trigger next action
        //}
        SetAnimation(State, 2);
    }

    //protected Quaternion _TargetRot;
    //private void RotateToOrientation()
    //{
    //    transform.rotation = Quaternion.Slerp(transform.rotation, _TargetRot, 0.10f);

    //    if (Mathf.Abs(transform.rotation.eulerAngles.y - _TargetRot.eulerAngles.y) < 1.0f)
    //    {
    //        transform.rotation = _TargetRot;
    //        State = Enums.ManStates.None; // Will trigger next action
    //    }
    //}

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
        if (position == Vector3.zero)
        {
            Debug.Log("WTF!");
        }
        _ActionList.Add(new ActionData(() => SetMoveToPosition(Enums.ManStates.Running, position), ActionData.ActionType.Movement));
    }

    public void Add_FacePlayerAction_ToList()
    {
        _ActionList.Add(new ActionData(() => SetFaceTowardsPlayer(), ActionData.ActionType.Movement));
    }

    //public void Add_RotateAction_ToList(Quaternion rotation)
    //{
    //    _ActionList.Add(new ActionData(() => SetRotateToOrientation(Enums.ManStates.Rotating, rotation), ActionData.ActionType.Movement));
    //}

    public void Add_IdleAction_ToList()
    {
        _ActionList.Add(new ActionData(() => SetAnimation(Enums.ManStates.Idle, 2), ActionData.ActionType.Animation));
    }

    // The working state can be of different animations, dependent on room (idle, working, ...)
    public void Add_WorkingAction_ToList(Enums.ManStates manState)
    {
        _ActionList.Add(new ActionData(() => SetAnimation(manState, 0), ActionData.ActionType.Animation));
    }

    public void Add_DoorOpenAction_ToList(Guid roomId)
    {
        _ActionList.Add(new ActionData(() => 
        {
            (RoomManager.Ref.GetRoomData(roomId).RoomScript as Room_Elevator).SetAnimation_OpenDoor(/*true*/);
            StartCoroutine(WaitForDoor(RoomManager.Ref.GetRoomData(roomId).RoomScript as Room_Elevator, false));
        }, ActionData.ActionType.Elevator));
    }

    public void Add_DoorCloseAction_ToList(Guid roomId)
    {
        _ActionList.Add(new ActionData(() => 
        {
            (RoomManager.Ref.GetRoomData(roomId).RoomScript as Room_Elevator).SetAnimation_CloseDoor(!CheckElevatorActionQueue(ActionData.ActionType.Elevator));
            if (CheckElevatorActionQueue(ActionData.ActionType.Elevator))
                StartCoroutine(WaitForDoor(RoomManager.Ref.GetRoomData(roomId).RoomScript as Room_Elevator, true));
        }, ActionData.ActionType.Elevator));
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
        SetAnimation(State, 0);
        yield return new WaitForSeconds(seconds);
        State = Enums.ManStates.None; // Will trigger next action
    }

    private IEnumerator WaitForRoomAccess(Guid room)
    {
        var r = RoomManager.Ref.GetRoomData(room).RoomScript;
        if (!r.GetAccessRequest(this))
        {
            State = Enums.ManStates.Waiting;
            SetAnimation(State, 0);

            yield return new WaitUntil(() => r.GetAccessRequest(this));
        }
        r.ManHasEntered(this); //Notify the room that there is now a man in the room
        State = Enums.ManStates.None; // Will trigger next action
    }

    private IEnumerator WaitForDoor(Room_Elevator room, bool closed)
    {
        Enums.ManStates s = State;
        State = Enums.ManStates.Waiting;
        SetAnimation(State, 0);
        yield return new WaitUntil(() => room.CheckDoor(closed) == false);
        yield return new WaitUntil(() => room.CheckDoor(closed));

        State = s;
        SetAnimation(s, 0);
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
            for (int i = 1; i < actionsToCheck; i++)
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
