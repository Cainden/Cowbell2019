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
    public RoomInstanceData ownedRoom = null;

    void Start ()
    {
        Deactivate();
        Debug.Assert(ManNameText != null);
        OwnedRoomName.gameObject.SetActive(false);
    }

    public void Activate(Guid manId)
    {
        ManNameText.text = ManManager.Ref.GetManData(manId).ManScript.ManData.GetManFullName();
        ManRef manRef = ManManager.Ref.GetManData(manId);
        ManNameText.text = manRef.ManScript.ManData.GetManFullName();
        ownedRoom = manRef.ManScript.ManData.OwnedRoomRef;
        gameObject.SetActive(true);

        if(ownedRoom != null)
        {
            OwnedRoomName.text = ownedRoom.RoomName;
            OwnedRoomName.gameObject.SetActive(true);
        }
        
        
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
