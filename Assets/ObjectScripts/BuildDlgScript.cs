using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BuildDlgScript : MonoBehaviour
{
    public enum ShowState { Type1, Type2, Type3, Type4 }

    private ShowState _CurrentShowState = ShowState.Type1;

    public GameObject BuildWindowSel1;
    public GameObject BuildWindowSel2;
    public GameObject BuildWindowSel3;
    public GameObject BuildWindowSel4;



    Button[] neutralButtons;
    Button[] utilButtons;
    Button[] overButtons;
    Button[] underButtons;

    void Start()
    {
        CheckReferences();
        SetBuildWindow(_CurrentShowState);
        SetButtonText();

    }

    private void CheckReferences()
    {
        Debug.Assert(BuildWindowSel1 != null);
        Debug.Assert(BuildWindowSel2 != null);
        Debug.Assert(BuildWindowSel3 != null);
        Debug.Assert(BuildWindowSel4 != null);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void SetWindowAdministration()
    {
        SetBuildWindow(ShowState.Type1);
    }

    public void SetWindowProduction()
    {
        SetBuildWindow(ShowState.Type2);
    }

    public void SetWindowSupport()
    {
        SetBuildWindow(ShowState.Type3);
    }

    public void SetWindowOther()
    {
        SetBuildWindow(ShowState.Type4);
    }
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
    private void SetBuildWindow(ShowState NewShowState)
    {
        switch (NewShowState)
        {
            case ShowState.Type1:
                BuildWindowSel1.SetActive(true);
                BuildWindowSel2.SetActive(false);
                BuildWindowSel3.SetActive(false);
                BuildWindowSel4.SetActive(false);
                break;
            case ShowState.Type2:
                BuildWindowSel1.SetActive(false);
                BuildWindowSel2.SetActive(true);
                BuildWindowSel3.SetActive(false);
                BuildWindowSel4.SetActive(false);
                break;
            case ShowState.Type3:
                BuildWindowSel1.SetActive(false);
                BuildWindowSel2.SetActive(false);
                BuildWindowSel3.SetActive(true);
                BuildWindowSel4.SetActive(false);
                break;
            case ShowState.Type4:
                BuildWindowSel1.SetActive(false);
                BuildWindowSel2.SetActive(false);
                BuildWindowSel3.SetActive(false);
                BuildWindowSel4.SetActive(true);
                break;
        }

        _CurrentShowState = NewShowState;
    }
}
