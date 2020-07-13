// Script, acting as main click event receiver and dispatcher.

using MySpace;
using System;
using System.Xml;
using UnityEngine;
using UnityEngine.EventSystems;
using MySpace.Stats;

public class ClickManager : MonoBehaviour
{
	public static ClickManager Ref { get; private set; } // singleton
	[SerializeField] EventSystem MyEventSystem = null; // To be set in editor

    private bool  _MouseOnRoom = false;
    private Guid  _MouseOnRoomGuid = Guid.Empty;
    private float _MouseOnManDownTime = 0.0f;
    private bool  _WasGuiClick = false;

    private int _LayerMaskMan; // For faster access later
    private int _LayerMaskRoom;
    private int _LayerMaskBuildPos;

    public static event Action<bool> SetGuestLayerEvent;
    public static bool GuestsDraggable
    {
        get => guestsDraggable;
        set
        {
            guestsDraggable = value;
            SetGuestLayerEvent?.Invoke(value);
        }
    }
    private static bool guestsDraggable = false;



    void Awake()
    {
		#region Singleton Managment
		if (Ref && Ref != this)
		{
			Destroy(this);
			return;
		}
		Ref = this;
		DontDestroyOnLoad(this);
		#endregion
	}

	void Start()
    {
        Debug.Assert(MyEventSystem != null);

        _LayerMaskMan = LayerMask.GetMask("Man");
        _LayerMaskRoom = LayerMask.GetMask("Room");
        _LayerMaskBuildPos = LayerMask.GetMask("BuildPos");
    }

	public static event Action LeftClick_Down;
	public static event Action LeftClick_Up;
	public static event Action LeftClick;

	void Update()
    {
		if (Input.GetMouseButtonDown(0))
			LeftClick_Down?.Invoke();

		if (Input.GetMouseButtonUp(0))
			LeftClick_Up?.Invoke();

        if (Input.GetMouseButton(0))
			LeftClick?.Invoke();
	}

	private void OnEnable()
	{
		LeftClick_Down += LeftMouseDown;
		LeftClick_Up += LeftMouseUp;
		LeftClick += LeftMouse;
	}

	private void OnDisable()
	{
		LeftClick_Down -= LeftMouseDown;
		LeftClick_Up -= LeftMouseUp;
		LeftClick -= LeftMouse;
	}

	#region input methods

	void LeftMouseDown()
    {
        if (MyEventSystem.IsPointerOverGameObject())
        {
            _WasGuiClick = true; // Remember for later (used in LeftMouseButtonUp)
            return;
        }

        if (StateManager.Ref.IsCameraDragAllowed())
        {
            // Prepare for camera dragging (will only become active if movement threshold is reached)
            CameraScript.Ref.CamPanStart();
        }

        if (RayCastCheckManClicked()) return;
        if (RayCastCheckRoomClicked()) return;
        if (RayCastCheckBuildPositionClicked()) return;
    }

	void LeftMouseUp()
	{
		if (_WasGuiClick)
		{
			_WasGuiClick = false;
			return;
		}

        if (CameraScript.Ref.IsCamDragging == true)
        {
            CameraScript.Ref.ResetCameraDragging();
            return;
        }

        if (_MouseOnRoom && StateManager.Ref.GetGameState() == Enums.GameStates.ChangeOwnedRoom)
		{
			RoomRef roomToChangeTo = RoomManager.Ref.GetRoomData(_MouseOnRoomGuid);

			//VERY temporary, needs to be changed to be less spaghetti once it is tested and works
			if (roomToChangeTo.RoomScript as Room_Hallway != null && ManManager.Ref.IsManTypeOf<ManScript_Guest>(StateManager.Ref.GetSelectedMan()))
			{
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, float.PositiveInfinity, _LayerMaskRoom))
				{
					roomToChangeTo = RoomManager.Ref.GetRoomData((roomToChangeTo.RoomScript as Room_Hallway).GetBedroomFromX(hitInfo.point.x - hitInfo.transform.position.x).RoomData.RoomId);

				}
			}

			if (roomToChangeTo.RoomScript.RoomHasFreeOwnerSlots())
			{
				ManManager.Ref.TransferOwnershipToRoom(StateManager.Ref.GetSelectedMan(), _MouseOnRoomGuid);
				StateManager.Ref.SetGameState(Enums.GameStates.ManSelected);
				return;
			}
			else
			{
				GuiManager.Ref.Initiate_UserInfoSmall("Sorry, that room is completely booked! Select another or Right Click to exit!");
				return;
			}
		}

		if (_MouseOnRoom && (StateManager.Ref.IsRoomSelectionAllowed()))
		{
			StateManager.Ref.SetGameState(Enums.GameStates.RoomSelected, _MouseOnRoomGuid);
			_MouseOnRoom = false;
			return;
		}

		if (StateManager.Ref.GetGameState() == Enums.GameStates.ManPressed)
		{
			StateManager.Ref.SetGameState(Enums.GameStates.ManSelected);
			return;
		}

		if (StateManager.Ref.GetGameState() == Enums.GameStates.ManDragging)
		{
			ManManager.Ref.MoveManToNewRoom(StateManager.Ref.GetSelectedMan(), StateManager.Ref.GetHighlightedRoom(), true);
			StateManager.Ref.SetGameState(Enums.GameStates.Normal);
			return;
		}
	}

	void LeftMouse()
	{
		if (_WasGuiClick) return;

		// In some modes, we could start to drag the camera
		if (StateManager.Ref.IsCameraDragAllowed())
		{
			CameraScript.Ref.CamPanUpdate();
		}

		if (RayCastCheckManDraggingMouseIsDown()) return;
		if (RayCastCheckRoomOverMouseIsDown()) return;
	}

	#endregion

	bool RayCastCheckManClicked()
    {
        if (StateManager.Ref.IsManSelectionAllowed())
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, _LayerMaskMan))
            {
                _MouseOnManDownTime = Time.time;
                Guid ManId = hitInfo.transform.GetComponent<ManScript>().ManData.ManId;
                if (!guestsDraggable && ManManager.Ref.GetManData(ManId).ManScript.ManType == Enums.ManTypes.Guest)
                    StateManager.Ref.SetGameState(Enums.GameStates.ManSelected, ManId);
                else
                    StateManager.Ref.SetGameState(Enums.GameStates.ManPressed, ManId);
                return true;
            }
        }

        return false;
    }

    bool RayCastCheckRoomClicked()
    {
        if (StateManager.Ref.IsRoomSelectionAllowed() || StateManager.Ref.GetGameState() == Enums.GameStates.ChangeOwnedRoom)
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, _LayerMaskRoom))
            {
                // Prepare for room being selected onMouseUp
                _MouseOnRoom = true;
                _MouseOnRoomGuid = hitInfo.transform.GetComponent<RoomScript>().RoomData.RoomId;
                return (true);
            }
        }

        _MouseOnRoom = false;
        _MouseOnRoomGuid = Guid.Empty;

        return (false);
    }

    bool RayCastCheckBuildPositionClicked()
    {
        if (StateManager.Ref.GetGameState() == Enums.GameStates.BuildRoom)
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, _LayerMaskBuildPos))
            {
                BuildPositionScript BuildPosScript = hitInfo.transform.GetComponent<BuildPositionScript>();

                if (WalletManager.SubtractHoots(RoomManager.Ref.GetCostByRoomType(BuildPosScript.RoomType)))
                {
                    BuildManager.Ref.Build_Finished(BuildPosScript.RoomType,
                                                BuildPosScript.RoomOverUnder,
                                                BuildPosScript.LeftmostIndex);
                }
                else
                {
                    //This can happen if the player builds right before an automated payment is made that causes them to lose money.
                    GuiManager.Ref.Initiate_UserInfoSmall("Sorry, You don't have enough Hoots to build that room!");
                }

                

                //For buying rooms
                

                StateManager.Ref.SetGameState(Enums.GameStates.Normal);
                return (true);
            }
        }

        return (false);
    }

    bool RayCastCheckManDraggingMouseIsDown()
    {
        // In ManPressed mode, we could possible move to ManDragging mode
        if (StateManager.Ref.GetGameState() == Enums.GameStates.ManPressed)
        {
            // Check if LMB has been pressed on a man, is still over it and held
            // long enough to enter ManDragging mode
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, float.PositiveInfinity, _LayerMaskMan))
            {
                if ((Time.time - _MouseOnManDownTime) > Constants.MouseDragInvokeDownTime)
                {
                    StateManager.Ref.SetGameState(Enums.GameStates.ManDragging);
                    return (true);
                }
            }
        }

        return (false);
    }

    //Need to change this to highlight doors of bedrooms in hallways if the player is currently dragging a tennant person, but highlight regularly if they are holding a worker.
    bool RayCastCheckRoomOverMouseIsDown()
    {
        // In ManDragging mode, we check for target room highlighting
        if (StateManager.Ref.GetGameState() == Enums.GameStates.ManDragging)
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, _LayerMaskRoom))
            {
                Guid RoomID = hitInfo.transform.GetComponent<RoomScript>().RoomData.RoomId;
                if (ManManager.Ref.IsManTypeOf<ManScript_Guest>(StateManager.Ref.GetSelectedMan()))
                {
                    if (RoomManager.IsRoomOfType<Room_Bedroom>(RoomID))
                    {
                        Room_Bedroom bedroom = (hitInfo.transform.GetComponent<RoomScript>() as Room_Hallway).GetBedroomFromX(hitInfo.point.x - hitInfo.transform.position.x);
                        RoomManager.Ref.HighlightBedroom(bedroom.RoomData.RoomId, bedroom.transform.position);
                        return false;
                    }
                }
                else if (ManManager.Ref.IsManTypeOf<ManScript_Worker>(StateManager.Ref.GetSelectedMan()))
                {
                    if (RoomManager.IsRoomOfType<Room_WorkQuarters>(RoomID))
                    {
                        RoomManager.Ref.HighlightRoom(RoomID);
                        return false;
                    }
                }
                RoomManager.Ref.HighlightNoRoom();
                StateManager.Ref.SetCurrentHoveredRoom(RoomID);
                return false;
            }
        }
        RoomManager.Ref.HighlightNoRoom();
        return (false);
    }    

    public void DeleteSelectedRoom()
    {
        if (StateManager.Ref.GetGameState() != Enums.GameStates.RoomSelected) return;
        Guid RoomID = StateManager.Ref.GetSelectedRoom();
        if (RoomManager.Ref.CanRemoveRoom(RoomID))
        {
            RoomManager.Ref.RemoveRoom(RoomID);
        }
        else
        {
            GuiManager.Ref.Initiate_UserInfoSmall("Sorry, can't remove this room!");
        }
        StateManager.Ref.SetGameState(Enums.GameStates.Normal);
    }

    public void DeleteSelectedMan()
    {
        if (StateManager.Ref.GetGameState() != Enums.GameStates.ManSelected) return;
        DeleteMan(StateManager.Ref.GetSelectedMan());
    }

    public void DeleteMan(Guid manId)
    {
        ManManager.Ref.MakeManLeave(manId);
        ManManager.Ref.RemoveManFromRoom(manId);
        ManManager.Ref.RemoveManFromList(manId);
        StateManager.Ref.SetGameState(Enums.GameStates.Normal);
    }

    public void DeleteMan(ManScript man)
    {
        DeleteMan(man.ManData.ManId);
    }
    
    public void AddNewCleaner()
    {
        //if (StateManager.Ref.IsManWaiting()) return;
        #region TEMPORARY - THIS NEEDS TO BE MADE INTO A RANDOM FUNCTION OR BE A CALLABLE METHOD THAT RETURNS WorkerConstructionData!
        WorkerConstructionData ManData = new WorkerConstructionData();
        ManData.manId = Guid.NewGuid();
        ManData.manType = Enums.ManTypes.Worker;
        ManData.manFirstName = NameFactory.GetNewFirstName();
        ManData.manLastName = NameFactory.GetNewLastName();
        ManData.generalStats = new GeneralStat[2]
        {
            new GeneralStat()
            {
                statType = GeneralStat.StatType.Speed, value = 1
            },
            new GeneralStat()
            {
                statType = GeneralStat.StatType.Loyalty, value = 1
            }
        };
        ManData.specialtyStats = new SpecialtyStat[3]
        {
            new SpecialtyStat()
            {
                statType = SpecialtyStat.StatType.Intelligence, BaseValue = 1, experience = 0
            },
            new SpecialtyStat()
            {
                statType = SpecialtyStat.StatType.Physicality, BaseValue = 1, experience = 0
            },
            new SpecialtyStat()
            {
                statType = SpecialtyStat.StatType.Professionalism, BaseValue = 1, experience = 0
            }
        };
        #endregion

        AddNewCleaner(ManData);
    }

    public void AddNewCleaner(WorkerConstructionData man)
    {
        if (man.manId == Guid.Empty)
            man.manId = Guid.NewGuid();
        ManManager.Ref.hireList.Add(man);

        GuiManager.Ref.Initiate_UserInfoSmall("New Cleaner application received!");
    }

    public void AddNewGuest()
    {
        //if (StateManager.Ref.IsManWaiting()) return;

        GuestConstructionData ManData = GameManager.CreateDefaultGuest();

        AddNewGuest(ManData);
    }

    public void AddNewGuest(GuestConstructionData guest)
    {
        ManManager.Ref.bookingList.Add(guest);

        GuiManager.Ref.Initiate_UserInfoSmall("New Guest waiting!");
    }

    public void SaveStateToFile()
    {
        LoadSaveManager.Ref.SaveCurrentState();
    }

    public void LoadStateFromFile()
    {
        StartCoroutine(LoadSaveManager.Ref.LoadCurrentState());
    }

    private void InitiateBuilding(Enums.RoomTypes RoomType)
    {
        // if there is no available build position, let the user know and don't try to build.
        GridManager.BuildInfo[] BuildingIndexArray = GridManager.Ref.GetPossibleBuildingindizes(RoomManager.Ref.GetRoomSizeByRoomType(RoomType));
        if (BuildingIndexArray.Length == 0)
        {
            GuiManager.Ref.Initiate_UserInfoSmall("No Available Build Locations Available!");
            return;
        }

        if (WalletManager.Hoots - RoomManager.Ref.GetCostByRoomType(RoomType) >= 0)
        {
            //Money deductions now made in RayCastCheckBuildPositionClicked()
            //WalletManager.Ref.Hoots -= Constants.RoomCostDefinitions[RoomSize];
            StateManager.Ref.SetGameState(Enums.GameStates.BuildRoom);
            BuildManager.Ref.ShowRoomPositionSelectors(BuildingIndexArray, RoomType, RoomManager.Ref.GetRoomSizeByRoomType(RoomType), RoomManager.Ref.GetOverUnderByRoomType(RoomType));
        }
        else
            GuiManager.Ref.Initiate_UserInfoSmall("Not enough hoots!");
    }

    public void BuildClick(string roomType)
    {
        if (Enum.TryParse(roomType, out Enums.RoomTypes type))
        {
            InitiateBuilding(type);
        }
        else
        {
            Debug.LogWarning("ClickManager.cs 'BuildClick'/n Try Parse Failed. '" + roomType + "' string input was not able to be converted into Enums.RoomTypes enum.");
        }
        
    }

    public void BuildClick(Enums.RoomTypes roomType)
    {
        InitiateBuilding(roomType);
    }

	#region Preset Build Functions
	/*
    public void BuildDlg_Click_Elevator()
    {    
        InitiateBuilding(Enums.RoomTypes.Elevator);
    }

    public void BuildDlg_Click_Sz2()
    { 
        InitiateBuilding(Enums.RoomSizes.Size2, Enums.RoomTypes.Common);
    }

    public void BuildDlg_Click_Sz4()
    {
        InitiateBuilding(Enums.RoomSizes.Size4, Enums.RoomTypes.Common);
    }

    public void BuildDlg_Click_Sz6()
    {
        InitiateBuilding(Enums.RoomSizes.Size6, Enums.RoomTypes.Common);
    }
    /// <summary>
    //OVERWORLD BUILD BUTTONS
    /// </summary>
    public void BuildDlg_Click_OW_Sz2()
    {
        InitiateBuilding(Enums.RoomSizes.Size2, Enums.RoomTypes.Common, Enums.RoomOverUnder.Over);
    }

    public void BuildDlg_Click_OW_Sz4()
    {
        InitiateBuilding(Enums.RoomSizes.Size4, Enums.RoomTypes.Common, Enums.RoomOverUnder.Over);
    }

    public void BuildDlg_Click_OW_Sz6()
    {
        InitiateBuilding(Enums.RoomSizes.Size6, Enums.RoomTypes.Common, Enums.RoomOverUnder.Over);
    }

    public void BuildDlg_Click_OW_BedroomSz2()
    {
        InitiateBuilding(Enums.RoomSizes.Size2, Enums.RoomTypes.Bedroom, Enums.RoomOverUnder.Over);
    }

    public void BuildDlg_Click_OW_BedroomSz4()
    {
        InitiateBuilding(Enums.RoomSizes.Size4, Enums.RoomTypes.Bedroom, Enums.RoomOverUnder.Over);
    }

    public void BuildDlg_Click_OW_BedroomSz6()
    {
        InitiateBuilding(Enums.RoomSizes.Size6, Enums.RoomTypes.Bedroom, Enums.RoomOverUnder.Over);
    }
    /// <summary>
    //UNDERWORLD BUILD BUTTONS
    /// </summary>
    public void BuildDlg_Click_UW_Sz2()
    {
        InitiateBuilding(Enums.RoomSizes.Size2, Enums.RoomTypes.Common, Enums.RoomOverUnder.Under);
    }

    public void BuildDlg_Click_UW_Sz4()
    {
        InitiateBuilding(Enums.RoomSizes.Size4, Enums.RoomTypes.Common, Enums.RoomOverUnder.Under);
    }

    public void BuildDlg_Click_UW_Sz6()
    {
        InitiateBuilding(Enums.RoomSizes.Size6, Enums.RoomTypes.Common, Enums.RoomOverUnder.Under);
    }
    */
	#endregion

	#region button methods
	
	public void Button_Close()
	{
		//switch to handle similar state mechanics to ChangeOwnedRoom state
		switch (StateManager.Ref.GetGameState())
		{
			case Enums.GameStates.ChangeOwnedRoom:
				StateManager.Ref.SetGameState(Enums.GameStates.ManSelected);
				break;
			default:
				//buildCancelled = true;
				StateManager.Ref.SetGameState(Enums.GameStates.Normal); // TODO: Probably need to refine this
																		//buildCancelled = false;
				break;
		}
	}

	/// <param name="_buttonNumber">If it's the first button on the UI from the top, = 0. If it's the second, this = 1. And so on.</param>
	public void Button_Hire(int _buttonNumber)
    {
        if (StateManager.Ref.IsManWaiting()) return;
        WorkerConstructionData newHire = ManManager.Ref.hireList[_buttonNumber];

        ManManager.Ref.CreateWorker(newHire);
        ManManager.Ref.hireList.RemoveAt(_buttonNumber);

        StateManager.Ref.SetWaitingMan(newHire.manId);
        GuiManager.Ref.Initiate_UserInfoSmall("New Employee incoming!");
    }

    public void Button_Hire(WorkerConstructionData worker)
    {
        if (StateManager.Ref.IsManWaiting()) return;

        ManManager.Ref.CreateWorker(worker);
        ManManager.Ref.hireList.Remove(worker);

        StateManager.Ref.SetWaitingMan(worker.manId);
        GuiManager.Ref.Initiate_UserInfoSmall("New Employee incoming!");
    }

	/// <param name="_buttonNumber">If it's the first button on the UI from the top, = 0. If it's the second, this = 1. And so on.</param>
	public void Button_Book(int _buttonNumber)
    {
        //if (StateManager.Ref.IsManWaiting()) return;
        GuestConstructionData newGuest = ManManager.Ref.bookingList[_buttonNumber];

        ManManager.Ref.CreateGuest(newGuest);
        ManManager.Ref.bookingList.RemoveAt(_buttonNumber);

        //StateManager.Ref.SetWaitingMan(newGuest.manId); guests should never be "waiting" where it blocks incoming characters.
        GuiManager.Ref.Initiate_UserInfoSmall("New Guest incoming!");
    }

    public void Button_Book(GuestConstructionData guest)
    {
        //if (StateManager.Ref.IsManWaiting()) return;

        ManManager.Ref.CreateGuest(guest);
        ManManager.Ref.bookingList.Remove(guest);

        //StateManager.Ref.SetWaitingMan(guest.manId); guests should never be "waiting" where it blocks incoming characters.
        GuiManager.Ref.Initiate_UserInfoSmall("New Guest incoming!");
    }

	public void ViewSelectedManOwnedRoom()
	{
		if (StateManager.Ref.GetGameState() != Enums.GameStates.ManSelected) return;
		Guid ManId = StateManager.Ref.GetSelectedMan();

		if (ManManager.Ref.GetManData(ManId).ManScript.ManData.OwnedRoomRef != null)
		{
			Guid RoomId = ManManager.Ref.GetManData(ManId).ManScript.ManData.OwnedRoomRef.RoomId;
			RoomManager.Ref.SelectRoom(RoomId);
		}

		//For moving the camera's position. May implement later.
		//GameObject ownedRoomObject = RoomManager.Ref.GetRoomData(RoomId).RoomObject;

	}

	public void ChangeSelectedManOwnedRoom()
	{
		if (StateManager.Ref.GetGameState() != Enums.GameStates.ManSelected) return;
		//Guid ManId = StateManager.Ref.GetSelectedMan();
		//ManManager.Ref.MakeManLeave(ManId);
		//ManManager.Ref.RemoveManOwnershipFromRoom(ManId);
		//ManManager.Ref.RemoveManFromList(ManId);
		StateManager.Ref.SetGameState(Enums.GameStates.ChangeOwnedRoom);
	}

	#endregion
}
