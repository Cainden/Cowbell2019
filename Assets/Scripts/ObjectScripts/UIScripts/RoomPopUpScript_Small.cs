using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using MySpace;

public class RoomPopUpScript_Small : MonoBehaviour
{
    Guid room = Guid.Empty;

    [SerializeField] Image fill;
    [SerializeField] float tickTimer = 1;

    
    public Color green;
    public Color red;

    float time;
    private void Update()
    {
        if (!gameObject.activeInHierarchy)
        {
            time = 0;
            return;
        }
        else if (room == Guid.Empty)
        {
            time = 0;
            gameObject.SetActive(false);
            return;
        }

        time += Time.deltaTime;
        if (time >= tickTimer)
        {
            time = 0;
            fill.fillAmount = (RoomManager.Ref.GetRoomData(room).RoomScript as Room_Bedroom).Cleanliness;
            fill.color = ColorLerp(red, green, fill.fillAmount);
        }
    }

    public void SetRoom(Guid id)
    {
        room = id;
    }

    public void Enable()
    {
        gameObject.SetActive(true);
        time = 0;

        if (room == Guid.Empty)
            return;
        fill.fillAmount = (RoomManager.Ref.GetRoomData(room).RoomScript as Room_Bedroom).Cleanliness;
        fill.color = ColorLerp(red, green, fill.fillAmount);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        time = 0;
    }



    //For the info button
    public void DisplayBigPopUp()
    {
        GuiManager.Ref.Show_RoomInfoWindow(room);
    }

    private Color ColorLerp(Color a, Color b, float t)
    {
        return new Color(Mathf.Lerp(a.r, b.r, t), Mathf.Lerp(a.g, b.g, t), Mathf.Lerp(a.b, b.b, t));
    }
}
