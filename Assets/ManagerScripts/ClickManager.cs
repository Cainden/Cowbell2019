// Script, acting as main click event receiver and dispatcher.

using MySpace;
using System;
using System.Xml;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickManager : MonoBehaviour
{
    public EventSystem MyEventSystem = null; // To be set in editor

    [HideInInspector]
    public static ClickManager Ref { get; private set; } // For external access of script

    private bool  _MouseOnRoom = false;
    private Guid  _MouseOnRoomGuid = Guid.Empty;
    private float _MouseOnManDownTime = 0.0f;
    private bool  _WasGuiClick = false;

    private int _LayerMaskMan; // For faster access later
    private int _LayerMaskRoom;
    private int _LayerMaskBuildPos;

    //for refunding rooms if right click occurs during build
    // Used in right click function
    public bool buildCancelled = false;

    void Awake()
    {
        if (Ref == null) Ref = GetComponent<ClickManager>();
    }

    void Start()
    {
        Debug.Assert(MyEventSystem != null);

        _LayerMaskMan = LayerMask.GetMask("Man");
        _LayerMaskRoom = LayerMask.GetMask("Room");
        _LayerMaskBuildPos = LayerMask.GetMask("BuildPos");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) LeftMouseButtonDown();  // LMB pressed during last frame (only one event)
        if (Input.GetMouseButtonDown(1)) RightMouseButtonDown(); // RMB pressed during last frame (only one event)
        if (Input.GetMouseButtonUp(0)) LeftMouseButtonUp();    // LMB released during last frame (only one event)
        if (Input.GetMouseButtonUp(1)) RightMouseButtonUp();   // RMB released during last frame (only one event)

        if (Input.GetMouseButton(0)) LeftMouseButton_IS_down();  // LMB is down right now
    }

    void LeftMouseButtonDown()
    {
        if (MyEventSystem.IsPointerOverGameObject())
        {
            _WasGuiClick = true; // Remember for later (used in LeftMouseButtonUp)
            return;
        }

        if (StateManager.Ref.IsCameraDragAllowed())
        {
            // Prepare for camera dragging (will only become active if movement threshold is reached)
            CameraScript.Ref.SetDragStartMousePosition(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (RayCastCheckManClicked()) return;
        if (RayCastCheckRoomClicked()) return;
        if (RayCastCheckBuildPositionClicked()) return;
    }

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
                StateManager.Ref.SetGameState(Enums.GameStates.ManPressed, ManId);
                return (true);
            }
        }

        return (false);
    }

    bool RayCastCheckRoomClicked()
    {
        if (StateManager.Ref.IsRoomSelectionAllowed())
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
                BuildManager.Ref.Build_Finished(BuildPosScript.RoomSize,
                                                BuildPosScript.RoomType,
                                                BuildPosScript.RoomOverUnder,
                                                BuildPosScript.LeftmostIndex);
                StateManager.Ref.SetGameState(Enums.GameStates.Normal);
                return (true);
            }
        }

        return (false);
    }

    void RightMouseButtonDown()
    {
        buildCancelled = true;
        StateManager.Ref.SetGameState(Enums.GameStates.Normal); // TODO: Probably need to refine this
        buildCancelled = false;
    }

    void LeftMouseButtonUp()
    {
        if (_WasGuiClick)
        {
            _WasGuiClick = false;
            return;
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
            ManManager.Ref.MoveManToNewRoom(StateManager.Ref.GetSelectedMan(), StateManager.Ref.GetHighlightedRoom());
            StateManager.Ref.SetGameState(Enums.GameStates.Normal);
            return;
        }

        if (CameraScript.Ref.IsCameraDragging() == true)
        {
            CameraScript.Ref.ResetCameraDragging();
            return;
        }
    }

    void RightMouseButtonUp()
    {
    }

    void LeftMouseButton_IS_down()
    {
        if (_WasGuiClick) return;

        // In some modes, we could start to drag the camera
        if (StateManager.Ref.IsCameraDragAllowed())
        {
            CameraScript.Ref.MoveDragCamera();
        }

        if (RayCastCheckManDraggingMouseIsDown()) return;
        if (RayCastCheckRoomOverMouseIsDown()) return;        
    }

    bool RayCastCheckManDraggingMouseIsDown()
    {
        // In ManPressed mode, we could possible move to ManDragging mode
        if (StateManager.Ref.GetGameState() == Enums.GameStates.ManPressed)
        {
            // Check if LMB has been pressed on a man, is still over it and held
            // long enough to enter ManDragging mode
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, _LayerMaskMan))
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

    bool RayCastCheckRoomOverMouseIsDown()
    {
        // In ManDragging mode, we check for target room highlighting
        if (StateManager.Ref.GetGameState() == Enums.GameStates.ManDragging)
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, _LayerMaskRoom))
            {
                // Highlight box
                Guid RoomID = hitInfo.transform.GetComponent<RoomScript>().RoomData.RoomId;
                RoomManager.Ref.HighlightRoom(RoomID);
            }
            else
            {
                RoomManager.Ref.HighlightNoRoom();
            }
        }

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
        Guid ManId = StateManager.Ref.GetSelectedMan();
        ManManager.Ref.MakeManLeave(ManId);
        ManManager.Ref.RemoveManFromRoom(ManId);        
        ManManager.Ref.RemoveManFromList(ManId);
        StateManager.Ref.SetGameState(Enums.GameStates.Normal);
    }

    public void AddNewMan()
    {
        //if (StateManager.Ref.IsManWaiting()) return;

        Guid ManId = Guid.NewGuid();
        ManInstanceData ManData = new ManInstanceData();
        ManData.ManId = ManId;
        ManData.ManType = Enums.ManTypes.StandardMan;
        ManData.ManFirstName = NameFactory.GetNewFirstName();
        ManData.ManLastName = NameFactory.GetNewLastName();

        ManManager.Ref.hireList.Add(ManData);

        //ManManager.Ref.CreateMan(ManId, Enums.ManTypes.StandardMan);
        //StateManager.Ref.SetWaitingMan(ManId);
        //GuiManager.Ref.Initiate_UserInfoSmall("New man incoming!");
    }

    public void AddNewCleaner()
    {
        //if (StateManager.Ref.IsManWaiting()) return;

        Guid ManId = Guid.NewGuid();
        ManInstanceData ManData = new ManInstanceData();
        ManData.ManId = ManId;
        ManData.ManType = Enums.ManTypes.Cleaner;
        ManData.ManFirstName = NameFactory.GetNewFirstName();
        ManData.ManLastName = NameFactory.GetNewLastName();

        ManManager.Ref.hireList.Add(ManData);

        //ManManager.Ref.CreateMan(ManId, Enums.ManTypes.StandardMan);
        //StateManager.Ref.SetWaitingMan(ManId);
        GuiManager.Ref.Initiate_UserInfoSmall("New Cleaner application received!");
    }

    public void AddNewGuest()
    {
        //if (StateManager.Ref.IsManWaiting()) return;

        Guid ManId = Guid.NewGuid();
        ManInstanceData ManData = new ManInstanceData();
        ManData.ManId = ManId;
        ManData.ManType = Enums.ManTypes.Guest;
        ManData.ManFirstName = NameFactory.GetNewFirstName();
        ManData.ManLastName = NameFactory.GetNewLastName();

        ManManager.Ref.bookingList.Add(ManData);

        //ManManager.Ref.CreateMan(ManId, Enums.ManTypes.StandardMan);
        //StateManager.Ref.SetWaitingMan(ManId);
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

    public void BuildButtonClicked()
    {
        if ((StateManager.Ref.GetGameState() == Enums.GameStates.GuiBlocking) &&
            (GuiManager.Ref.IsBuildRoomDlgActive()))
        {
            StateManager.Ref.SetGameState(Enums.GameStates.Normal);
            return;
        }

        if (!StateManager.Ref.IsRoomBuildDialogAllowed()) return;

        GuiManager.Ref.ShowBuildRoomDlg(true);
        StateManager.Ref.SetGameState(Enums.GameStates.GuiBlocking);
    }

    public void HireButtonClicked()
    {
        if ((StateManager.Ref.GetGameState() == Enums.GameStates.GuiBlocking) &&
           (GuiManager.Ref.IsHireDlgActive()))
        {
            StateManager.Ref.SetGameState(Enums.GameStates.Normal);
            return;
        }

        if (!StateManager.Ref.IsHireDialogAllowed()) return;

        GuiManager.Ref.ShowHireDlg(true);
        StateManager.Ref.SetGameState(Enums.GameStates.GuiBlocking);
    }

    public void BookButtonClicked()
    {
        if ((StateManager.Ref.GetGameState() == Enums.GameStates.GuiBlocking) &&
           (GuiManager.Ref.IsBookGuestDlgActive()))
        {
            StateManager.Ref.SetGameState(Enums.GameStates.Normal);
            return;
        }

        if (!StateManager.Ref.IsBookGuestDialogAllowed()) return;

        GuiManager.Ref.ShowBookGuestDlg(true);
        StateManager.Ref.SetGameState(Enums.GameStates.GuiBlocking);
    }

    public void MainMenuButtonClicked()
    {
        if ((StateManager.Ref.GetGameState() == Enums.GameStates.GuiBlocking) &&
            (GuiManager.Ref.IsMainMenuDlgActive()))
        {
            StateManager.Ref.SetGameState(Enums.GameStates.Normal);
            return;
        }

        if (!StateManager.Ref.IsMainMenuDialogAllowed()) return;

        GuiManager.Ref.ShowMainMenuDlg(true);
        StateManager.Ref.SetGameState(Enums.GameStates.GuiBlocking);
    }

    private void InitiateBuilding(Enums.RoomSizes RoomSize, Enums.RoomTypes RoomType, Enums.RoomOverUnder RoomOverUnder = Enums.RoomOverUnder.Neutral)
    {
        if (WalletManager.Ref.Hoots - Constants.RoomCostDefinitions[RoomSize] >= 0)
        {
            WalletManager.Ref.Hoots -= Constants.RoomCostDefinitions[RoomSize];
            StateManager.Ref.SetGameState(Enums.GameStates.BuildRoom);
            GridIndex[] BuildingIndexArray = GridManager.Ref.GetPossibleBuildingindizes(RoomSize);
            BuildManager.Ref.ShowRoomPositionSelectors(BuildingIndexArray, RoomType, RoomSize, RoomOverUnder);
        }
        else
            GuiManager.Ref.Initiate_UserInfoSmall("Not enough hoots!");
    }    

    public void BuildDlg_Click_Elevator()
    {    
            InitiateBuilding(Enums.RoomSizes.Size1, Enums.RoomTypes.Elevator);
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

    /// <summary>
    //HIRE WINDOW BUTTONS
    /// </summary>
    public void HireDlg0_Click()
    {
        if (StateManager.Ref.IsManWaiting()) return;
        ManInstanceData newHire = ManManager.Ref.hireList[0];

        ManManager.Ref.CreateMan(newHire);
        ManManager.Ref.hireList.RemoveAt(0);

        StateManager.Ref.SetWaitingMan(newHire.ManId);
        GuiManager.Ref.Initiate_UserInfoSmall("New Employee incoming!");
    }

    public void HireDlg1_Click()
    {
        if (StateManager.Ref.IsManWaiting()) return;
        ManInstanceData newHire = ManManager.Ref.hireList[1];

        ManManager.Ref.CreateMan(newHire);
        ManManager.Ref.hireList.RemoveAt(1);

        StateManager.Ref.SetWaitingMan(newHire.ManId);
        GuiManager.Ref.Initiate_UserInfoSmall("New Employee incoming!");
    }

    public void HireDlg2_Click()
    {
        if (StateManager.Ref.IsManWaiting()) return;
        ManInstanceData newHire = ManManager.Ref.hireList[2];

        ManManager.Ref.CreateMan(newHire);
        ManManager.Ref.hireList.RemoveAt(2);

        StateManager.Ref.SetWaitingMan(newHire.ManId);
        GuiManager.Ref.Initiate_UserInfoSmall("New Employee incoming!");
    }

    public void HireDlg3_Click()
    {
        if (StateManager.Ref.IsManWaiting()) return;
        ManInstanceData newHire = ManManager.Ref.hireList[3];

        ManManager.Ref.CreateMan(newHire);
        ManManager.Ref.hireList.RemoveAt(3);

        StateManager.Ref.SetWaitingMan(newHire.ManId);
        GuiManager.Ref.Initiate_UserInfoSmall("New Employee incoming!");
    }
    /// <summary>
    //BOOKING WINDOW BUTTONS
    /// </summary>
    public void BookDlg0_Click()
    {
        if (StateManager.Ref.IsManWaiting()) return;
        ManInstanceData newGuest = ManManager.Ref.bookingList[0];

        ManManager.Ref.CreateMan(newGuest);
        ManManager.Ref.bookingList.RemoveAt(0);

        StateManager.Ref.SetWaitingMan(newGuest.ManId);
        GuiManager.Ref.Initiate_UserInfoSmall("New Guest incoming!");
    }

    public void BookDlg1_Click()
    {
        if (StateManager.Ref.IsManWaiting()) return;
        ManInstanceData newGuest = ManManager.Ref.bookingList[1];

        ManManager.Ref.CreateMan(newGuest);
        ManManager.Ref.bookingList.RemoveAt(1);

        StateManager.Ref.SetWaitingMan(newGuest.ManId);
        GuiManager.Ref.Initiate_UserInfoSmall("New Guest incoming!");
    }

    public void BookDlg2_Click()
    {
        if (StateManager.Ref.IsManWaiting()) return;
        ManInstanceData newGuest = ManManager.Ref.bookingList[2];

        ManManager.Ref.CreateMan(newGuest);
        ManManager.Ref.bookingList.RemoveAt(2);

        StateManager.Ref.SetWaitingMan(newGuest.ManId);
        GuiManager.Ref.Initiate_UserInfoSmall("New Guest incoming!");
    }

    public void BookDlg3_Click()
    {
        if (StateManager.Ref.IsManWaiting()) return;
        ManInstanceData newGuest = ManManager.Ref.bookingList[3];

        ManManager.Ref.CreateMan(newGuest);
        ManManager.Ref.bookingList.RemoveAt(3);

        StateManager.Ref.SetWaitingMan(newGuest.ManId);
        GuiManager.Ref.Initiate_UserInfoSmall("New Guest incoming!");
    }
}
