using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySpace;

public class ManInfoWindowScript : MonoBehaviour
{
    public Text ManNameText;
    public Text OwnedRoomName;
    public Button viewOwnedButton, changeOwnedButton;
    public RoomInstanceData ownedRoom = null;

    void Start ()
    {
        
        Debug.Assert(ManNameText != null);
        OwnedRoomName.gameObject.SetActive(false);
        viewOwnedButton.gameObject.SetActive(false);
        changeOwnedButton.gameObject.SetActive(false);
        Deactivate();
    }

    public void Activate(Guid manId)
    {
        ManRef<ManScript> manRef = ManManager.Ref.GetManData(manId);
        ManNameText.text = manRef.ManScript.ManData.GetManFullName();
        
        gameObject.SetActive(true);

        switch (manRef.ManScript.ManData.ManType)
        {
            case Enums.ManTypes.Max:
                break;
            case Enums.ManTypes.Monster:
                break;
            case Enums.ManTypes.None:
                break;
            case Enums.ManTypes.StandardMan:
                break;
            case Enums.ManTypes.Worker:
                break;
            case Enums.ManTypes.Guest:
                ownedRoom = manRef.ManScript.ManData.OwnedRoomRef;
                if (ownedRoom != null)
                {
                    OwnedRoomName.text = ownedRoom.RoomName;
                    OwnedRoomName.gameObject.SetActive(true);
                    viewOwnedButton.gameObject.SetActive(true);
                    changeOwnedButton.gameObject.SetActive(true);
                }
                else
                {
                    OwnedRoomName.text = string.Empty;
                    OwnedRoomName.gameObject.SetActive(false);
                    viewOwnedButton.gameObject.SetActive(false);
                    changeOwnedButton.gameObject.SetActive(false);
                }
                break;
            case Enums.ManTypes.MC:
                break;
            default:
                break;
        }



        
        
        
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        RoomManager.Ref.DeselectRooms();
    }
}
