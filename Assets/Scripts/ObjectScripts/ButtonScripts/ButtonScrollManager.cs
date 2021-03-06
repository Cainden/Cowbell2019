﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;
using UnityEngine.UI;

public class ButtonScrollManager : MonoBehaviour
{
    [SerializeField] Enums.RoomCategories category;
    [SerializeField] GameObject panel, scrollView;
    private ButtonScript[] buttons;
    //public bool Scrolling { get; set; }

    public void Init(GameObject buttonPrefab)
    {
        buttons = ButtonScript.CreateButtonArrayFromRoomData(RoomManager.GetAllRoomsofCategory(category, true), buttonPrefab);

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].transform.SetParent(panel.transform, false);
        }
    }


}
