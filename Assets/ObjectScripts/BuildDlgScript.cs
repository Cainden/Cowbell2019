using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BuildDlgScript : MonoBehaviour
{
    public enum ShowState { Type1, Type2, Type3, Type4 }

    public ButtonScrollManager BuildWindowSel1;
    public ButtonScrollManager BuildWindowSel2;
    public ButtonScrollManager BuildWindowSel3;
    public ButtonScrollManager BuildWindowSel4;

    /// <summary>
    /// The offset of each new button in the Y position, from the previous button.
    /// </summary>
    public const float yOffset = -40;

    #region MonoMethods

    void Start()
    {
        CheckReferences();
        BuildWindowSel1.Init();
        BuildWindowSel2.Init();
        BuildWindowSel3.Init();
        BuildWindowSel4.Init();
        SetBuildWindow(ShowState.Type1);
    }

    private void CheckReferences()
    {
        Debug.Assert(BuildWindowSel1 != null);
        Debug.Assert(BuildWindowSel2 != null);
        Debug.Assert(BuildWindowSel3 != null);
        Debug.Assert(BuildWindowSel4 != null);
    }
    #endregion

    #region Change Build Tab Button Functions
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        if (!active)
        {
            CameraScript.ZoomDisabled = false;
        }
    }

    public void SetWindowMisc()
    {
        SetBuildWindow(ShowState.Type1);
    }

    public void SetWindowUtility()
    {
        SetBuildWindow(ShowState.Type2);
    }

    public void SetWindowOverworld()
    {
        SetBuildWindow(ShowState.Type3);
    }

    public void SetWindowUnderworld()
    {
        SetBuildWindow(ShowState.Type4);
    }
    #endregion

    #region Old Button Text Method
    /*
    private void SetButtonText()
    {

        //THIS NEXT SECTION is to dynamically set the cost that displays for each item on the build screen

        GameObject[] buildWindowSels = { BuildWindowSel1, BuildWindowSel2, BuildWindowSel3, BuildWindowSel4 };

        //loop through build windows
        for (int i = 0; i < buildWindowSels.Length; ++i)
        {
            Button[] currSelectionButtons = buildWindowSels[i].GetComponentsInChildren<Button>(true);

            //loop through buttons on current window
            for (int j = 0; j < currSelectionButtons.Length; ++j)
            {
                //Get the second item, since that's the COST text
                Text costText = currSelectionButtons[j].GetComponentsInChildren<Text>(true)[1];

                //Set the text based on the game object name
                if (costText.name == "Sz2Cost")
                {
                    costText.text = "Cost:"
                        + RoomManager.Ref.GetCostByRoomType(MySpace.Enums.RoomTypes.Bedroom_Size2)
                        + " Hoots";
                }
                else if (costText.name == "Sz4Cost")
                {
                    costText.text = "Cost:"
                        + RoomManager.Ref.GetCostByRoomType(MySpace.Enums.RoomTypes.Bedroom_Size4)
                        + " Hoots";
                }
                else if (costText.name == "Sz6Cost")
                {
                    costText.text = "Cost:"
                        + RoomManager.Ref.GetCostByRoomType(MySpace.Enums.RoomTypes.Bedroom_Size6)
                        + " Hoots";
                }
                else if (costText.name == "UtilCost")
                {
                    costText.text = "Cost:"
                        + RoomManager.Ref.GetCostByRoomType(MySpace.Enums.RoomTypes.Elevator)
                        + " Hoots";
                }



            }
        }



        //neutralButtons = BuildWindowSel1.GetComponentsInChildren<Button>();
        //neutralButtons[0].GetComponentInChildren<Text>(true).text += ": " + MySpace.Constants.RoomCostDefinitions[MySpace.Enums.RoomSizes.Size2] + " Hoots";
        //neutralButtons[1].GetComponentInChildren<Text>(true).text += ": " + MySpace.Constants.RoomCostDefinitions[MySpace.Enums.RoomSizes.Size4] + " Hoots";
        //neutralButtons[2].GetComponentInChildren<Text>(true).text += ": " + MySpace.Constants.RoomCostDefinitions[MySpace.Enums.RoomSizes.Size6] + " Hoots";

        //utilButtons = BuildWindowSel2.GetComponentsInChildren<Button>();
        //utilButtons[0].GetComponentInChildren<Text>(true).text += ": " + MySpace.Constants.RoomCostDefinitions[MySpace.Enums.RoomSizes.Size1] + " Hoots";

        //overButtons = BuildWindowSel3.GetComponentsInChildren<Button>();
        //overButtons[0].GetComponentInChildren<Text>(true).text += ": " + MySpace.Constants.RoomCostDefinitions[MySpace.Enums.RoomSizes.Size2] + " Hoots";
        //overButtons[1].GetComponentInChildren<Text>(true).text += ": " + MySpace.Constants.RoomCostDefinitions[MySpace.Enums.RoomSizes.Size4] + " Hoots";
        //overButtons[2].GetComponentInChildren<Text>(true).text += ": " + MySpace.Constants.RoomCostDefinitions[MySpace.Enums.RoomSizes.Size6] + " Hoots";

        //underButtons = BuildWindowSel4.GetComponentsInChildren<Button>();
        //underButtons[0].GetComponentInChildren<Text>(true).text += ": " + MySpace.Constants.RoomCostDefinitions[MySpace.Enums.RoomSizes.Size2] + " Hoots";
        //underButtons[1].GetComponentInChildren<Text>(true).text += ": " + MySpace.Constants.RoomCostDefinitions[MySpace.Enums.RoomSizes.Size4] + " Hoots";
        //underButtons[2].GetComponentInChildren<Text>(true).text += ": " + MySpace.Constants.RoomCostDefinitions[MySpace.Enums.RoomSizes.Size6] + " Hoots";
    }
    */
    #endregion

    private void SetBuildWindow(ShowState NewShowState)
    {
        switch (NewShowState)
        {
            case ShowState.Type1:
                BuildWindowSel1.gameObject.SetActive(true);
                BuildWindowSel2.gameObject.SetActive(false);
                BuildWindowSel3.gameObject.SetActive(false);
                BuildWindowSel4.gameObject.SetActive(false);
                if (BuildWindowSel1.Scrolling)
                {
                    CameraScript.ZoomDisabled = true;
                }
                else
                {
                    CameraScript.ZoomDisabled = false;
                }
                break;              
            case ShowState.Type2:   
                BuildWindowSel1.gameObject.SetActive(false);
                BuildWindowSel2.gameObject.SetActive(true);
                BuildWindowSel3.gameObject.SetActive(false);
                BuildWindowSel4.gameObject.SetActive(false);
                if (BuildWindowSel2.Scrolling)
                {
                    CameraScript.ZoomDisabled = true;
                }
                else
                {
                    CameraScript.ZoomDisabled = false;
                }
                break;              
            case ShowState.Type3:   
                BuildWindowSel1.gameObject.SetActive(false);
                BuildWindowSel2.gameObject.SetActive(false);
                BuildWindowSel3.gameObject.SetActive(true);
                BuildWindowSel4.gameObject.SetActive(false);
                if (BuildWindowSel3.Scrolling)
                {
                    CameraScript.ZoomDisabled = true;
                }
                else
                {
                    CameraScript.ZoomDisabled = false;
                }
                break;              
            case ShowState.Type4:   
                BuildWindowSel1.gameObject.SetActive(false);
                BuildWindowSel2.gameObject.SetActive(false);
                BuildWindowSel3.gameObject.SetActive(false);
                BuildWindowSel4.gameObject.SetActive(true);
                if (BuildWindowSel4.Scrolling)
                {
                    CameraScript.ZoomDisabled = true;
                }
                else
                {
                    CameraScript.ZoomDisabled = false;
                }
                break;
        }
    }
}
