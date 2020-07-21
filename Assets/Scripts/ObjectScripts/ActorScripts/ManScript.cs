// Object script for general behaviour of one man avatar

using MySpace;
using MySpace.Stats;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private Enums.ManStates state = Enums.ManStates.None; //Here for debug purposes

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
    //protected List<ActionData> _ActionList = new List<ActionData>();
    protected List<GridIndex> MovementPath = new List<GridIndex>();
    protected Action NextAction;

    //protected Animator _Animator;
    [SerializeField] protected Animator animator;

    #region Material Handling
    //private Renderer[] _Renderers;
    //private Material _MaterialNormal;
    //private Material _MaterialHighlight; // Selected
    //private Material _MaterialGhost;     // Leaving
    #endregion

    // Couroutines
    private IEnumerator WaitCoroutine;

    protected MoodBubbleScript moodScript;

    /// <summary>
    /// Used by whatever room the guest/worker is currently in to slow down function calls.
    /// </summary>
    public float delayTimer;
    #endregion
    #endregion

    #region Mono Methods
    /// <summary>
    /// Call Base Start!
    /// </summary>
    protected virtual void Start()
    {
        //_Animator = GetComponentInChildren<Animator>();
        //_Renderers = GetComponentsInChildren<Renderer>();
        SetMaterials();
        CheckReferences();
        ManName = NameFactory.GetNewFirstName() + " " + NameFactory.GetNewLastName();
        animator.speed = (1 + (GetGeneralStatValue(GeneralStat.StatType.Speed) * 0.1f)) * 2;
        moodScript = GetComponentInChildren<MoodBubbleScript>();

        StartCoroutine(MoveToLobby(GameManager.StartPath));
        delayTimer = 0;
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
        GameManager.MoodCalcEvent -= MoodCalc;
        GameManager.MoodCalcEvent += MoodCalc;
    }

    protected virtual void OnDisable()
    {
        GameManager.NetRevenueCalculationEvent -= RevenueCalc;
        GameManager.MoodCalcEvent -= MoodCalc;
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
        //Debug.Assert(_Renderers != null);
        //Debug.Assert(_MaterialNormal != null);
        //Debug.Assert(_MaterialHighlight != null);
        //Debug.Assert(_MaterialGhost != null);
    }

    private void SetMaterials()
    {
        //HAVE THIS DISABLED FOR NOW
        //_MaterialNormal = GetComponentInChildren<Renderer>().material;
        //_MaterialHighlight = Resources.Load<Material>(Constants.ManSelectedMaterial);
        //_MaterialGhost = Resources.Load<Material>(Constants.ManGhostMaterial);
        //switch (ManType)
        //{
        //    case Enums.ManTypes.Max: // Who is Max? lol
        //        break;
        //    case Enums.ManTypes.Monster:
        //        _MaterialNormal.color = Color.red;
        //        _MaterialGhost.color = Color.red;
        //        break;
        //    case Enums.ManTypes.None:
        //    case Enums.ManTypes.StandardMan:
        //        break;
        //    case Enums.ManTypes.Worker:
        //        _MaterialNormal.color = Color.blue;
        //        _MaterialGhost.color = Color.blue;
        //        break;
        //    case Enums.ManTypes.Guest:
        //        _MaterialNormal.color = Color.green;
        //        _MaterialGhost.color = Color.green;
        //        break;
        //    case Enums.ManTypes.MC: // MC is main character? not sure what this one is either.
        //        break;
        //    default:
        //        break;
        //}
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
        return 1;
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

    public void SetMood(Enums.ManMood mood, bool displayMoodChange, float? displayDuration = null)
    {
        if (!moodScript)
            return;
        if (displayMoodChange)
            moodScript.DisplayMood(mood, displayDuration);
        else
            moodScript.SetMoodValue(mood);
    }

    public void ResolveMood(Enums.ManMood mood)
    {
        moodScript?.ResolveMood(mood);
    }

    public void ChangeHappiness(float change)
    {
        moodScript.ModifyMood(change);
    }

    public Enums.ManMood GetMood()
    {
        if (!moodScript)
            return Enums.ManMood.Happy;
        return moodScript.Mood;
    }

    void MoodCalc(ref float t, ref int n)
    {
        t += (moodScript.OverrideMood == Enums.ManMood.None ? moodScript.CurrentMood : (int)moodScript.OverrideMood);
        n++;
    }
    #endregion

    #region State Methods
    public void SetSelectedState(bool selected)
    {
        if (selected)
        {
            if (GameManager.Debug)
                print("Selecting man material " + ManData.GetManFullName() + ".");
            //foreach (Renderer r in _Renderers) r.material = _MaterialHighlight;
        }
        else
        {
            if (GameManager.Debug)
                print("Resetting man material " + ManData.GetManFullName() + ".");
            //foreach (Renderer r in _Renderers) r.material = _MaterialNormal;
        }
        
    }

    //public void SetGhostState()
    //{
    //    GetComponent<BoxCollider>().enabled = false; // Disable raycasting
    //    foreach (Renderer r in _Renderers) r.material = _MaterialGhost;
    //}

    public void SetFaceTowardsPlayer()
    {
        SetState(State, 2);
    }

    /// <summary>
    /// Changes the character's animation and also rotation to match direction, as well as changes the character's state to match.
    /// </summary>
    /// <param name="state">The state and animation to change to.</param>
    /// <param name="dir">0 = noChange, 1 = forward, 2 = backward, 3 = left, 4 = right. Any other number automatically assigns the direction</param>
    /// <param name="_TargetPos">The position that the character is currently being moved to</param>
    public void SetState(Enums.ManStates state, int dir, Vector3 _TargetPos = default)
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


        if (state == State)
            return;
        SetAnimation(state, dir, _TargetPos);

        State = state;
        if (GameManager.Debug)
            Debug.Log("Setting man '" + gameObject.name + " (" + ManData.GetManFullName() + ")' state to " + State.ToString() + ". Guid: " + ManData.ManId);
    }

    /// <summary>
    /// Same Functionality as SetState, but doesn't change the character's state, only set's it's animations and facing direction
    /// </summary>
    /// <param name="state">The animation to set to</param>
    /// <param name="dir">0 = noChange, 1 = forward, 2 = backward, 3 = left, 4 = right. Any other number automatically assigns the direction</param>
    /// <param name="_TargetPos">The position that the character is currently being moved to</param>
    public void SetAnimation(Enums.ManStates state, int dir, Vector3 _TargetPos = default)
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
            case Enums.ManStates.None:
            case Enums.ManStates.Idle:
            case Enums.ManStates.Waiting:
                animator.SetTrigger("IdleTrigger");
                break;
            case Enums.ManStates.Running:
                animator.SetTrigger("RunningTrigger");
                break;
            case Enums.ManStates.Dancing:
                animator.SetTrigger("IsDance");
                break;
        }
    }

    protected void StateUpdate()
    {
        if (State == Enums.ManStates.None)
        {
            ProcessActions();
        }
    }



    #region State Functionality Methods
    public virtual void BeginAnnihilation()
    {
        StartCoroutine(LeaveLobby(GameManager.StartPath));
    }

    protected virtual IEnumerator MoveToLobby(Vector3[] path)
    {
        
        if (path.Length <= 2)
        {
            Debug.LogError("Entrance path is less than two nodes!!");
            yield break;
        }

        transform.position = path[0];

        for (int i = 0; i < path.Length; i++)
        {
            if (path[i].y != transform.position.y)
                path[i] = new Vector3(path[i].x, transform.position.y, path[i].z);
        }

        for (int i = 1; i < path.Length; i++)
        {
            SetState(Enums.ManStates.Running, -1, path[i]);

            if (i == path.Length - 1)
            {
                (RoomManager.Ref.GetRoomData(RoomManager.lobbyId).RoomScript as Room_Lobby).OpenDoorOutsideEnter(1 + (GetGeneralStatValue(GeneralStat.StatType.Speed) * 0.1f));
            }
            
            while (!DoMovement(path[i]))
            {
                yield return null;
            }

            if (i == path.Length - 1)
            {
                (RoomManager.Ref.GetRoomData(RoomManager.lobbyId).RoomScript as Room_Lobby).CloseDoor();
            }
        }
        ManManager.Ref.MoveManToNewRoom(ManData.ManId, RoomManager.lobbyId);
        SetState(Enums.ManStates.None, 0);

    }

    protected virtual IEnumerator LeaveLobby(Vector3[] path)
    {
        if (path.Length <= 2)
        {
            Debug.LogError("Entrance path is less than two nodes!!");
            yield break;
        }

        for (int i = 0; i < path.Length; i++)
        {
            if (path[i].y != transform.position.y)
                path[i] = new Vector3(path[i].x, transform.position.y, path[i].z);
        }

        for (int i = path.Length - 1; i >= 0; i--)
        {
            SetState(Enums.ManStates.Running, -1, path[i]);
            if (i == path.Length - 2)
            {
                (RoomManager.Ref.GetRoomData(RoomManager.lobbyId).RoomScript as Room_Lobby).OpenDoorInsideEnter(1 + (GetGeneralStatValue(GeneralStat.StatType.Speed) * 0.1f));
            }
            
            while (!DoMovement(path[i]))
            {
                yield return null;
            }

            if (i == path.Length - 2)
            {
                (RoomManager.Ref.GetRoomData(RoomManager.lobbyId).RoomScript as Room_Lobby).CloseDoor();
            }
        }
        Destroy(gameObject);
    }

    public bool DoMovement(Vector3 _TargetPos)
    {
        ////debug stuff
        //string path = "MovementPath: ";
        //for (int i = 0; i < MovementPath.Count; i++)
        //{
        //    path += MovementPath[i] + ", ";
        //}
        //print(path);

        float Travel = (Constants.ManRunSpeed + (GetGeneralStatValue(GeneralStat.StatType.Speed) * 0.1f)) * Time.deltaTime;
        //print("Travel: " + Travel);
        if (Travel >= Vector3.Distance(transform.position, _TargetPos)) // Target reached
        {
            transform.position = _TargetPos;
            return true;
        }
        else // Regular movement
        {
            Vector3 dir = (_TargetPos - transform.position);
            dir.Normalize();
            transform.position += (dir * Travel);
            return false;
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

    protected void ProcessActions(GridIndex lastPos = new GridIndex())
    {
        //if (_ActionList.Count == 0) return;
        //_ActionList[0].ActionItem.Invoke();
        //_ActionList.RemoveAt(0);
        if (MovementPath.Count > 0)
        {
            StartCoroutine(Movement(new IndexPair((lastPos == GridIndex.Zero) ? GridManager.Ref.GetXYGridIndexFromWorldPosition(transform.position, true) : lastPos, MovementPath[0])));
            return;
        }
        else if (NextAction != null)
        {
            NextAction();
            NextAction = null;
        }
        if (State != Enums.ManStates.None)
        {
            SetState(Enums.ManStates.None, 0);
        }
    }

    private IEnumerator Movement(IndexPair pair)
    {
        if (GameManager.Debug)
            print("Pair Movement: " + pair.start.ToString() + ", " + pair.end);
        if (!GridManager.GetIndexPairAccessRequest(this, pair))
        {
            SetState(Enums.ManStates.Waiting, 0);
            yield return new WaitUntil(() => GridManager.GetIndexPairAccessRequest(this, pair));
        }

        GridManager.CallPreWaitActionStart(this, pair);
        if (!GridManager.WaitForPairStart(this, pair))
        {
            SetState(Enums.ManStates.Waiting, 0);
            yield return new WaitUntil(() => GridManager.WaitForPairStart(this, pair));
        }
        
        GridManager.CallPairStartEvent(this, pair);

        Vector3 target = GridManager.Ref.GetWorldPositionFromGridIndexZOffset(pair.end, Constants.GridPositionWalkZOffset);
        if (GridManager.HasOverrideMovement(pair))
        {
            //debug stuff
            //print("Has override update! Index: (" + pair.start + " + " + pair.end + ")");
            while (!GridManager.DoUpdateOnIndexPair(this, pair, target))
            {
                yield return null;
            }
        }
        else
        {
            SetState(Enums.ManStates.Running, -1, target);
            while (!DoMovement(target))
            {
                yield return null;
            }
        }

        GridManager.CallPreWaitActionEnd(this, pair);
        if (!GridManager.WaitForPairEnd(this, pair))
        {
            SetState(Enums.ManStates.Waiting, 0);
            yield return new WaitUntil(() => GridManager.WaitForPairEnd(this, pair));
        }
        
        GridManager.CallPairEndEvent(this, pair);

        GridIndex last = MovementPath[0]; //Remember, this is needed because we need to remove this gridindex from the movement path before we call the next process actions 
        MovementPath.Remove(MovementPath[0]);
        CheckMovementActions();
        ProcessActions(last);
    }

    #region Old
    /*
    public void Add_AccessAction_ToList(Guid roomId)
    {
        _ActionList.Add(new ActionData(() => StartCoroutine(WaitForRoomAccess(roomId)), ActionData.ActionType.Wait));
    }

    public void Add_RunAction_ToList(Vector3 position)
    {
        if (position == Vector3.zero)
        {
            Debug.LogError("WTF!");
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
            (RoomManager.Ref.GetRoomData(roomId).RoomScript as Room_Elevator).SetAnimation_OpenDoor();
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

    public void Add_ElevatorMovementAction_ToList(Room_Elevator room, int YIndex, bool moveMan)
    {
        _ActionList.Add(new ActionData(() =>
        {
            if (moveMan)
                room.MoveBoxToFloor(YIndex, this);
            else
                room.MoveBoxToFloor(YIndex);
            StartCoroutine(WaitForRoomAccess(room));
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

    public void Add_Action_ToList(ActionData action)
    {
        _ActionList.Add(action);
    }

    public void Add_Action_ToList(ActionData action, int index)
    {
        _ActionList.Insert(index, action);
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

    protected void SetMoveToPosition(Enums.ManStates state, Vector3 position)
    {
        State = state;
        _TargetPos = position;
        SetAnimation(State, -1);
    }
    */
    #endregion

    #region Movement List Helper Function
    public void SetMovementPath(GridIndex[] path)
    {
        MovementPath = path.ToList();
    }
    public void SetMovementPath(List<GridIndex> path)
    {
        MovementPath = path;
    }

    public void AddMovementActions(GridIndex[] path)
    {
        if (GameManager.Debug)
        {
            string p = "Man Added Movement Path: ";
            for (int i = 0; i < path.Length; i++)
            {
                p += "[" + i + "]" + path[i].ToString() + ", ";
            }
            print(p);
        }
        
        for (int i = 0; i < path.Length; i++)
        {
            MovementPath.Add(path[i]);
        }
    }

    public void AddMovementActions(List<GridIndex> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            MovementPath.Add(path[i]);
        }
    }

    public void AddMovementAction(GridIndex position)
    {
        MovementPath.Add(position);
    }

    public void AddMovementPositionAtIndex(GridIndex position, int index)
    {
        MovementPath.Insert(index, position);
    }

    public void AddActionToEndOfMovement(Action action)
    {
        NextAction -= action;
        NextAction += action;
    }


    protected List<Container<int, Action<ManScript>>> MovementActions = new List<Container<int, Action<ManScript>>>();
    public void AddActionToMovement(Action<ManScript> action, int stepsUntilAction)
    {
        for (int i = 0; i < MovementActions.Count; i++)
        {
            if (MovementActions[i].object1 == stepsUntilAction)
            {
                MovementActions[i].object2.Add(action);
                return;
            }
        }
        MovementActions.Add(new Container<int, Action<ManScript>>() { object1 = stepsUntilAction, object2 = action });
    }

    protected void CheckMovementActions()
    {
        if (MovementActions.Count > 0)
        {
            for (int i = 0; i < MovementActions.Count; i++)
            {
                if (MovementActions[i].object1 == 0)
                {
                    MovementActions[i].object2?.Invoke(this);
                    MovementActions.Remove(MovementActions[i]);
                    i--;
                }
                else
                {
                    MovementActions[i] = new Container<int, Action<ManScript>>() { object1 = (MovementActions[i].object1 - 1), object2 = MovementActions[i].object2 };
                }
            }
        }
    }

    #endregion

    #region Old_2
    //private IEnumerator WaitForSeconds(float seconds)
    //{
    //    State = Enums.ManStates.Waiting;
    //    SetAnimation(State, 0);
    //    yield return new WaitForSeconds(seconds);
    //    State = Enums.ManStates.None; // Will trigger next action
    //}

    //private IEnumerator WaitForRoomAccess(Guid room)
    //{
    //    var r = RoomManager.Ref.GetRoomData(room).RoomScript;
    //    if (!r.GetAccessRequest(this))
    //    {
    //        State = Enums.ManStates.Waiting;
    //        SetAnimation(State, 0);
    //        yield return null;
    //        yield return new WaitUntil(() => r.GetAccessRequest(this));
    //    }
    //    r.ManHasEntered(this); //Notify the room that there is now a man in the room
    //    State = Enums.ManStates.None; // Will trigger next action
    //}

    //private IEnumerator WaitForRoomAccess(RoomScript room)
    //{
    //    if (!room.GetAccessRequest(this))
    //    {
    //        State = Enums.ManStates.Waiting;
    //        SetAnimation(State, 0);
    //        yield return null;
    //        yield return new WaitUntil(() => room.GetAccessRequest(this));
    //    }
    //    room.ManHasEntered(this); //Notify the room that there is now a man in the room
    //    State = Enums.ManStates.None; // Will trigger next action
    //}

    //private IEnumerator WaitForDoor(Room_Elevator room, bool closed)
    //{
    //    Enums.ManStates s = State;
    //    State = Enums.ManStates.Waiting;
    //    SetAnimation(State, 0);
    //    yield return new WaitUntil(() => room.CheckDoor(closed) == false);
    //    yield return new WaitUntil(() => room.CheckDoor(closed));

    //    State = s;
    //    SetAnimation(s, 0);
    //}

    //private IEnumerator WaitForElevatorMovement(Room_Elevator room)
    //{
    //    State = Enums.ManStates.Waiting;
    //    SetAnimation(State, 0);
    //    yield return new WaitUntil(() => !room.BoxMoving); //Wait until the elevator box has reached it's destination
    //    State = Enums.ManStates.None;
    //}
    #endregion

    #endregion

}
