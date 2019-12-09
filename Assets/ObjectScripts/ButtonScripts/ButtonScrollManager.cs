using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;
using UnityEngine.UI;

public class ButtonScrollManager : MonoBehaviour
{
    [SerializeField] Enums.RoomCategories category;
    [SerializeField] GameObject button1, panel, scrollView;
    private ButtonScript[] buttons;
    public bool Scrolling { get; set; }

    public void Init()
    {
        buttons = ButtonScript.CreateButtonArrayFromRoomData(RoomManager.GetAllRoomsofCategory(category, true), button1);
        if (buttons.Length > 4)
        {
            panel.GetComponent<RectTransform>().offsetMin = new Vector2(panel.GetComponent<RectTransform>().offsetMin.x, 135 - (60 * buttons.Length));
            scrollView.GetComponentInChildren<Scrollbar>().size = 4 / buttons.Length;
            scrollView.GetComponentInChildren<Scrollbar>().value = 1;
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].transform.parent = panel.transform;
            }
            Scrolling = true;
        }
        else
        {
            scrollView.SetActive(false);
            Scrolling = false;
        }
    }




}
