using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    private Enums.RoomTypes type;
    private Enums.RoomCategories category;
    private bool isLocked;
    private Button button;
    private ColorBlock originColor;
    #region Disabled Color Colorblock
    private ColorBlock disabledColor = new ColorBlock()
    {
        colorMultiplier = 1,
        disabledColor = Color.black,
        highlightedColor = Color.gray * 1.2f,
        normalColor = Color.gray,
        pressedColor = Color.gray * 0.8f,
        selectedColor = Color.gray,
        fadeDuration = 0.1f
    };
    #endregion

    [SerializeField] Text buttonText, costText;

    private void OnDisable()
    {
        if (isLocked)
            if (RoomManager.UnlockEvents.ContainsKey(type))
                RoomManager.UnlockEvents[type] -= Unlock;
    }

    public static ButtonScript[] CreateButtonArrayFromRoomData(RoomDefData[] data, GameObject prefab)
    {
        ButtonScript[] ar = new ButtonScript[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            if (i == 0)
            {
                ar[i] = prefab.GetComponent<ButtonScript>().Init(BuildDlgScript.yOffset * i, data[i].Locked, data[i].RoomCategory, data[i].RoomType, data[i].RoomCost);
                continue;
            }
            ar[i] = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation, prefab.transform.parent).GetComponent<ButtonScript>().
                Init(BuildDlgScript.yOffset * i, data[i].Locked, data[i].RoomCategory, data[i].RoomType, data[i].RoomCost);
        }
        return ar;
    }

    public ButtonScript Init(float yOffset, bool locked, Enums.RoomCategories category, Enums.RoomTypes type, int cost)
    {
        this.type = type;
        this.category = category;
        isLocked = locked;
        button = GetComponent<Button>();
        originColor = button.colors;

        transform.Translate(0, yOffset, 0);


        if (isLocked)
        {
            if (!RoomManager.UnlockEvents.ContainsKey(type))
                RoomManager.UnlockEvents.Add(type, new System.Action(Unlock));
            else
            {
                RoomManager.UnlockEvents[type] -= Unlock;
                RoomManager.UnlockEvents[type] += Unlock;
            }
            button.colors = disabledColor;
        }

        buttonText.text = "Build " + type.RoomTypeToRoomDisplayName();
        costText.text = "Cost: " + cost + " Hoots";

        return this;
    }

    void Unlock()
    {
        if (isLocked)
        {
            button.colors = originColor;
        }
        isLocked = false;
        RoomManager.UnlockEvents[type] -= Unlock;
    }

    public void BuildClick()
    {
        if (isLocked)
        {
            GuiManager.Ref.Initiate_UserInfoSmall("Sorry, you have not yet unlocked the ability to build this room yet!");
        }
        else
            ClickManager.Ref.BuildClick(type);
    }
}
