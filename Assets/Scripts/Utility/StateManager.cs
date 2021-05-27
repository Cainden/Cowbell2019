// Containing the statemachine et al.

using MySpace;
using System;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [HideInInspector]
    public static StateManager Ref { get; private set; } // For external access of script

    private Enums.GameStates _GameState = Enums.GameStates.None;
    private RoomScript m_selectedRoom;
    private RoomScript m_highlightedRoom;
    private ManScript m_selectedMan;
    private ManScript m_waitingMan;

    void Awake()
    {
        if (Ref == null) Ref = GetComponent<StateManager>();
    }

    void Start()
    {
        //Moved the code for actually creating rooms to the room manager so the information that creates the start rooms can be displayed in the inspector there.
        RoomManager.Ref.CreateStartRooms();

        SetGameState(Enums.GameStates.Normal);
    }

    public void ClearAll()
    {
        ResetSelectedMan();
        ResetSelectedRoom();
        SetWaitingMan(null);
        SetCurrentHoveredRoom(null);
    }

    private void SetSelectedRoom(RoomScript room)
    {
        m_selectedRoom = room;
        RoomManager.Ref.SelectRoom(room.RoomData.RoomId);
    }

    public RoomScript GetSelectedRoom()
    {
        return (m_selectedRoom);
    }

    public void SetCurrentHoveredRoom(RoomScript roomId)
    {
        m_highlightedRoom = roomId;
    }

    public RoomScript GetHighlightedRoom()
    {
        return (m_highlightedRoom);
    }

    private void ResetSelectedRoom()
    {
        //if (m_selectedRoom == Guid.Empty) return;
        GuiManager.Ref.Hide_RoomInfoWindow();
        m_selectedRoom = null;
        RoomManager.Ref.DeselectRooms();
    }

    private void SetSelectedMan(ManScript man)
    {
        if (m_selectedMan == man) return;
        ResetSelectedMan();
        m_selectedMan = man;
        ManManager.Instance.SetHighlightedMan(man);
    }

    public ManScript GetSelectedMan()
    {
        return (m_selectedMan);
    }

    private void ResetSelectedMan()
    {
        if (m_selectedMan == null) return;
        GuiManager.Ref.Hide_ManInfoWindow();
        ManManager.Instance.ResetHighlightedMan(m_selectedMan);
        m_selectedMan = null;
    }

    public void SetWaitingMan(ManScript man)
    {
        m_waitingMan = man;
    }

    public ManScript GetWaitingMan()
    {
        return (m_waitingMan);
    }

    public bool IsManWaiting()
    {
        return (m_waitingMan != null);
    }

    public Enums.GameStates GetGameState()
    {
        return (_GameState);
    }

    public void SetGameState(Enums.GameStates newGameState, GameObject selectedObject)
    {
        switch (newGameState)
        {
            case Enums.GameStates.ManPressed:
                SetSelectedMan(selectedObject.GetComponent<ManScript>());
                break;
            case Enums.GameStates.ManSelected:
                SetSelectedMan(selectedObject.GetComponent<ManScript>());
                break;
            case Enums.GameStates.RoomSelected:
                SetSelectedRoom(selectedObject.GetComponent<RoomScript>());
                break;
        }

        SetGameState(newGameState);
    }

    public void SetGameState(Enums.GameStates newGameState)
    {
        // Handles state transitions. 
        //_GameState is the previous state, newGameState is the state to transition to.
        switch (newGameState)
        {
            case Enums.GameStates.Normal:

                //NOT USED ANYMORE. LEAVING FOR ARCHIVAL PURPOSES FOR NOW
                //Refund the room purchase by getting the room BuildPosition info. Is this the best way for this?
                //if (_GameState == Enums.GameStates.BuildRoom && ClickManager.Ref.buildCancelled == true)
                //    WalletManager.Ref.Hoots += Constants.RoomCostDefinitions[GameObject.FindObjectOfType<BuildPositionScript>().RoomSize];

                // If the player is chaning a guest's room and cancels (right clicks)
                // return to the ManInfoWindow they were just on
                // May be a better way to do this (Dupe code in ManSelected case)
                // didn't want to use a GoTo/while loop
                if (_GameState == Enums.GameStates.ChangeOwnedRoom)
                {
                    newGameState = Enums.GameStates.ManSelected;
                    ResetSelectedRoom();
                    GuiManager.Ref.Show_ManInfoWindow(m_selectedMan.ManData.ManId);
                    break;
                }


                ResetSelectedRoom();
                ResetSelectedMan();
                GuiManager.Ref.SetNormalCursor();
                BuildManager.Ref.HideRoomPositionSelectors();
                RoomManager.Ref.HighlightNoRoom();
                SidePanel.SetPanel(false);
                GuiManager.Ref.SetCancelButton(false);
                //GuiManager.Ref.ShowBuildRoomDlg(false);
                //GuiManager.Ref.ShowMainMenuDlg(false);
                //GuiManager.Ref.ShowHireDlg(false);
                //GuiManager.Ref.ShowBookGuestDlg(false);
                break;
            case Enums.GameStates.GuiBlocking:
                GuiManager.Ref.SetGuiBlockingCursor();
                ResetSelectedRoom();
                ResetSelectedMan();
                break;
            case Enums.GameStates.BuildRoom:
                ResetSelectedRoom();
                ResetSelectedMan();
                SidePanel.SetPanel(false);
                GuiManager.Ref.SetCancelButton(true);
                //GuiManager.Ref.ShowBuildRoomDlg(false);
                break;
            case Enums.GameStates.RoomSelected:
                ResetSelectedMan();
                if (RoomManager.IsRoomOfType<Room_Hallway>(m_selectedRoom))
                    (RoomManager.Ref.GetRoomData(m_selectedRoom.RoomData.RoomId).RoomScript as Room_Hallway).GetBedroomFromRayCheck().TogglePopUp();
                //GuiManager.Ref.Show_RoomInfoWindow((RoomManager.Ref.GetRoomData(m_selectedRoom).RoomScript as Room_Hallway).GetBedroomFromRayCheck());
                else
                    GuiManager.Ref.Show_RoomInfoWindow(m_selectedRoom);
                break;
            case Enums.GameStates.ManPressed:
                ResetSelectedRoom();
                break;
            case Enums.GameStates.ManSelected:
                ResetSelectedRoom();
                GuiManager.Ref.Show_ManInfoWindow(m_selectedMan.ManData.ManId);
                break;
            case Enums.GameStates.ManDragging:
                ResetSelectedRoom();
                GuiManager.Ref.SetManDragCursor();
                break;
            case Enums.GameStates.ChangeOwnedRoom:
                GuiManager.Ref.Hide_ManInfoWindow();
                //ManManager.Instance.RemoveManOwnershipFromRoom(GetSelectedMan());
                ResetSelectedRoom();
                break;
        }

        _GameState = newGameState;
    }

    public bool IsCameraDragAllowed()
    {
        switch (_GameState)
        {
            case Enums.GameStates.Normal:
            case Enums.GameStates.BuildRoom:
            case Enums.GameStates.ManSelected:
            case Enums.GameStates.RoomSelected: return (true);
            default: return (false);
        }
    }

    public bool IsRoomSelectionAllowed()
    {
        if (CameraScript.Ref.IsCamDragging) return (false);

        switch (_GameState)
        {
            case Enums.GameStates.Normal:
            case Enums.GameStates.ManSelected:
            case Enums.GameStates.RoomSelected: return (true);
            default: return (false);
        }
    }

    public bool IsManSelectionAllowed()
    {
        if (CameraScript.Ref.IsCamDragging) return (false);

        switch (_GameState)
        {
            case Enums.GameStates.Normal:
            case Enums.GameStates.ManSelected:
            case Enums.GameStates.RoomSelected: return (true);
            default: return (false);
        }
    }

    public bool IsRoomBuildDialogAllowed()
    {
        if (CameraScript.Ref.IsCamDragging) return (false);

        switch (_GameState)
        {
            case Enums.GameStates.Normal:
            case Enums.GameStates.ManSelected:
            case Enums.GameStates.RoomSelected: return (true);
            default: return (false);
        }
    }

    public bool IsMainMenuDialogAllowed()
    {
        if (CameraScript.Ref.IsCamDragging) return (false);

        switch (_GameState)
        {
            case Enums.GameStates.Normal:
            case Enums.GameStates.ManSelected:
            case Enums.GameStates.RoomSelected: return (true);
            default: return (false);
        }
    }

    public bool IsHireDialogAllowed()
    {
        if (CameraScript.Ref.IsCamDragging) return (false);

        switch (_GameState)
        {
            case Enums.GameStates.Normal:
            case Enums.GameStates.ManSelected:
            case Enums.GameStates.RoomSelected: return (true);
            default: return (false);
        }
    }

    public bool IsBookGuestDialogAllowed()
    {
        if (CameraScript.Ref.IsCamDragging) return (false);

        switch (_GameState)
        {
            case Enums.GameStates.Normal:
            case Enums.GameStates.ManSelected:
            case Enums.GameStates.RoomSelected: return (true);
            default: return (false);
        }
    }
}
