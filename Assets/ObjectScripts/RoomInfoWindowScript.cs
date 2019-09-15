using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomInfoWindowScript : MonoBehaviour
{
    public Text RoomNameText;
    public Text RoomInfoText1;
    public Text RoomInfoText2;

    void Start ()
    {
        Deactivate();
        Debug.Assert(RoomNameText != null);
        Debug.Assert(RoomInfoText1 != null);
        Debug.Assert(RoomInfoText2 != null);
    }

    public void Activate(Guid roomId)
    {
        RoomScript RoomScript = RoomManager.Ref.GetRoomData(roomId).RoomScript;        

        RoomNameText.text = RoomScript.RoomData.RoomName;

        string Info;

        Info = "Category: " + RoomScript.RoomData.RoomCategory.ToString() + "\r\n";
        Info += "Size: " + RoomScript.RoomData.RoomSize.ToString() + "\r\n";
        RoomInfoText1.text = Info;

        RoomInfoText2.text = RoomFactory.Ref.GetRoomDefData(RoomScript.RoomData.RoomType, RoomScript.RoomData.RoomSize, RoomScript.RoomData.RoomOverUnder).RoomDescription;
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
