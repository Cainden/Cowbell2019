// Dealing with all GUI things (show dialogs etc)

using MySpace;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour
{
    public Text TextObject_GameState = null; // To be set by editor
    public Text TextObject_ManCount = null;
    public Text TextObject_HootCount = null;
    public Text TextObject_SoulCount = null;
    public Text TextObject_MonCoinCount = null;


    public Texture2D CursorNormalTex = null;
    public Texture2D CursorDragCamTex = null;
    public Texture2D CursorDragManTex = null;
    public Texture2D CursorGuiBlockingTex = null;

    public GameObject GuiSmallUserInfoDlg = null;
    public GameObject RoomInfoWindow = null;
    public GameObject ManInfoWindow = null;
    public GameObject BuildRoomDlg = null;
    public GameObject MainMenuDlg = null;
    public GameObject HireDlg = null;
    public GameObject BookGuestDlg = null;

    private Enums.CursorStates _CursorState = Enums.CursorStates.None;
    private Enums.CursorStates _PrevCursorState = Enums.CursorStates.None;

    [HideInInspector]
    public static GuiManager Ref { get; private set; } // For external access of script

    void Awake()
    {
        if (Ref == null) Ref = GetComponent<GuiManager>();
    }

    void Start()
    {
        Debug.Assert(TextObject_GameState != null);
        Debug.Assert(TextObject_ManCount != null);
        Debug.Assert(TextObject_HootCount != null);
        Debug.Assert(TextObject_SoulCount != null);
        Debug.Assert(TextObject_MonCoinCount != null);

        Debug.Assert(CursorNormalTex != null);
        Debug.Assert(CursorDragCamTex != null);
        Debug.Assert(CursorDragManTex != null);
        Debug.Assert(CursorGuiBlockingTex != null);

        Debug.Assert(GuiSmallUserInfoDlg != null);
        Debug.Assert(RoomInfoWindow != null);
        Debug.Assert(ManInfoWindow != null);
        Debug.Assert(BuildRoomDlg != null);
        Debug.Assert(MainMenuDlg != null);
        Debug.Assert(HireDlg != null);
        Debug.Assert(BookGuestDlg != null);

        SetCursorState(Enums.CursorStates.Normal);
        UpdateManCount(ManManager.Ref.GetManCount());
    }

    void Update()
    {
        UpdateStateInfo();
    }

    void UpdateStateInfo()
    {
        TextObject_GameState.text = "GameState:" + StateManager.Ref.GetGameState().ToString() +
                                    "  " +
                                    "CamDrag: " + CameraScript.Ref.IsCamDragging.ToString() +
                                    "\r\n" +
                                    "SelRoom: " + StateManager.Ref.GetSelectedRoom().ToString() +
                                    "\r\n" +
                                    "HiliRoom: " + StateManager.Ref.GetHighlightedRoom().ToString() +
                                    "\r\n" +
                                    "SelMan: " + StateManager.Ref.GetSelectedMan().ToString() +
                                    "\r\n" +
                                    "WaitMan: " + StateManager.Ref.GetWaitingMan().ToString();
    }

    private void SetCursorState(Enums.CursorStates newState)
    {
        if (_CursorState == newState) return;

        switch (newState)
        {
            case Enums.CursorStates.Normal:
                Cursor.SetCursor(CursorNormalTex, Vector2.zero, CursorMode.ForceSoftware);
                break;
            case Enums.CursorStates.CamDrag:
                Cursor.SetCursor(CursorDragCamTex, Vector2.zero, CursorMode.ForceSoftware);
                break;
            case Enums.CursorStates.ManDrag:
                Cursor.SetCursor(CursorDragManTex, Vector2.zero, CursorMode.ForceSoftware);
                break;
            case Enums.CursorStates.GuiBlocking:
                Cursor.SetCursor(CursorGuiBlockingTex, Vector2.zero, CursorMode.ForceSoftware);
                break;
        }

        _PrevCursorState = _CursorState;
        _CursorState = newState;
    }


    public void SetCamDragCursor(bool active)
    {
        if (active) SetCursorState(Enums.CursorStates.CamDrag);
        else SetCursorState(_PrevCursorState);
    }

    public void SetManDragCursor()
    {
        SetCursorState(Enums.CursorStates.ManDrag);
    }

    public void SetGuiBlockingCursor()
    {
        SetCursorState(Enums.CursorStates.GuiBlocking);
    }

    public void SetNormalCursor()
    {
        SetCursorState(Enums.CursorStates.Normal);
    }

    public void Initiate_UserInfoSmall(string infoText)
    {
        GuiSmallUserInfoDlg.GetComponent<GuiUserInfoSmallScript>().StartInfoText(infoText);
    }

    public void Show_RoomInfoWindow(Guid roomId)
    {
        RoomInfoWindow.GetComponent<RoomInfoWindowScript>().Activate(roomId);
    }

    public void Hide_RoomInfoWindow()
    {
        RoomInfoWindow.GetComponent<RoomInfoWindowScript>().Deactivate();
    }

    public void Show_ManInfoWindow(Guid roomId)
    {
        ManInfoWindow.GetComponent<ManInfoWindowScript>().Activate(roomId);
    }

    public void Hide_ManInfoWindow()
    {
        ManInfoWindow.GetComponent<ManInfoWindowScript>().Deactivate();
    }    
    
    public void UpdateManCount(int number)
    {
        TextObject_ManCount.text = number.ToString();
    }

    public void UpdateHootCount(int number)
    {
        TextObject_HootCount.text = number.ToString();
    }

    public void UpdateSoulCount(int number)
    {
        TextObject_SoulCount.text = number.ToString();
    }

    public void UpdateMonCoinCount(int number)
    {
        TextObject_MonCoinCount.text = number.ToString();
    }

    public void ShowBuildRoomDlg(bool showIt)
    {
        BuildRoomDlg.GetComponent<BuildDlgScript>().SetActive(showIt);
    }

    public void ShowHireDlg(bool showIt)
    {
        HireDlg.GetComponent<HireDlgScript>().SetActive(showIt);
    }
    public void ShowBookGuestDlg(bool showIt)
    {
        BookGuestDlg.GetComponent<BookGuestDlgScript>().SetActive(showIt);
    }
    public void ShowMainMenuDlg(bool showIt)
    {
        MainMenuDlg.SetActive(showIt);
    }

    public bool IsBuildRoomDlgActive()
    {
        return (BuildRoomDlg.activeInHierarchy);
    }

    public bool IsMainMenuDlgActive()
    {
        return (MainMenuDlg.activeInHierarchy);
    }
    public bool IsHireDlgActive()
    {
        return (HireDlg.activeInHierarchy);
    }

    public bool IsBookGuestDlgActive()
    {
        return (BookGuestDlg.activeInHierarchy);
    }
}

