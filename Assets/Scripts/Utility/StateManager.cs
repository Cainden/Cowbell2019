// Containing the statemachine et al.

using MySpace;
using System;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [HideInInspector]
    public static StateManager Ref { get; private set; } // For external access of script

    private Enums.GameStates _GameState = Enums.GameStates.None;
    private Guid _SelectedRoom = Guid.Empty;
    private Guid _HighlightedRoom = Guid.Empty;
    private Guid _SelectedMan = Guid.Empty;
    private Guid _WaitingMan = Guid.Empty;



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
        SetWaitingMan(Guid.Empty);
        SetCurrentHoveredRoom(Guid.Empty);
    }

    private void SetSelectedRoom(Guid RoomID)
    {
        _SelectedRoom = RoomID;
        RoomManager.Ref.SelectRoom(RoomID);
    }

    public Guid GetSelectedRoom()
    {
        return (_SelectedRoom);
    }

    public void SetCurrentHoveredRoom(Guid roomId)
    {
        _HighlightedRoom = roomId;
    }

    public Guid GetHighlightedRoom()
    {
        return (_HighlightedRoom);
    }

    private void ResetSelectedRoom()
    {
        if (_SelectedRoom == Guid.Empty) return;
        GuiManager.Ref.Hide_RoomInfoWindow();
        _SelectedRoom = Guid.Empty;
        RoomManager.Ref.DeselectRooms();
    }

    private void SetSelectedMan(Guid manId)
    {
        if (_SelectedMan == manId) return;
        ResetSelectedMan();
        _SelectedMan = manId;
        ManManager.Ref.SetHighlightedMan(manId);
    }

    public Guid GetSelectedMan()
    {
        return (_SelectedMan);
    }

    private void ResetSelectedMan()
    {
        if (_SelectedMan == Guid.Empty) return;
        GuiManager.Ref.Hide_ManInfoWindow();
        ManManager.Ref.ResetHighlightedMan(_SelectedMan);
        _SelectedMan = Guid.Empty;
    }

    public void SetWaitingMan(Guid manId)
    {
        _WaitingMan = manId;
    }

    public Guid GetWaitingMan()
    {
        return (_WaitingMan);
    }

    public bool IsManWaiting()
    {
        return (_WaitingMan != Guid.Empty);
    }

    public Enums.GameStates GetGameState()
    {
        return (_GameState);
    }

    public void SetGameState(Enums.GameStates newGameState, Guid selectedObjectId)
    {
        switch (newGameState)
        {
            case Enums.GameStates.ManPressed: SetSelectedMan(selectedObjectId); break;
            case Enums.GameStates.ManSelected: SetSelectedMan(selectedObjectId); break;
            case Enums.GameStates.RoomSelected: SetSelectedRoom(selectedObjectId); break;
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
                    GuiManager.Ref.Show_ManInfoWindow(_SelectedMan);
                    break;
                }


                ResetSelectedRoom();
                ResetSelectedMan();
                GuiManager.Ref.SetNormalCursor();
                BuildManager.Ref.HideRoomPositionSelectors();
                RoomManager.Ref.HighlightNoRoom();
                SidePanel.SetPanel(false);
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
                //GuiManager.Ref.ShowBuildRoomDlg(false);
                break;
            case Enums.GameStates.RoomSelected:
                ResetSelectedMan();
                if (RoomManager.IsRoomOfType<Room_Hallway>(_SelectedRoom))
                    GuiManager.Ref.Show_RoomInfoWindow((RoomManager.Ref.GetRoomData(_SelectedRoom).RoomScript as Room_Hallway).GetBedroomFromRayCheck());
                else
                    GuiManager.Ref.Show_RoomInfoWindow(_SelectedRoom);
                break;
            case Enums.GameStates.ManPressed:
                ResetSelectedRoom();
                break;
            case Enums.GameStates.ManSelected:
                ResetSelectedRoom();
                GuiManager.Ref.Show_ManInfoWindow(_SelectedMan);
                break;
            case Enums.GameStates.ManDragging:
                ResetSelectedRoom();
                GuiManager.Ref.SetManDragCursor();
                break;
            case Enums.GameStates.ChangeOwnedRoom:
                GuiManager.Ref.Hide_ManInfoWindow();
                //ManManager.Ref.RemoveManOwnershipFromRoom(GetSelectedMan());
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
