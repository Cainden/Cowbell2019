using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomInfoWindowScript : MonoBehaviour
{
    public TextMeshProUGUI RoomNameText;
    public TextMeshProUGUI StankText;
    public TextMeshProUGUI RoomInfoText;

    public GameObject ManInfoParent;

    

    void Start ()
    {
        Deactivate();
        Debug.Assert(RoomNameText != null);
        Debug.Assert(StankText != null);
        Debug.Assert(RoomInfoText != null);
    }

    public void Activate(Guid roomId)
    {
        RoomScript RoomScript = RoomManager.Ref.GetRoomData(roomId).RoomScript;        

        RoomNameText.text = RoomScript.RoomData.RoomName;

        string Info = /*"Category: " + */RoomScript.RoomData.RoomCategory.ToString()/* + "\r\n"*/;
        //Info += "Size: " + RoomScript.RoomData.RoomSize.ToString() + "\r\n";
        StankText.text = Info;

        RoomInfoText.text = RoomManager.GetRoomDefData(RoomScript.RoomData.RoomType).RoomDescription;
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
