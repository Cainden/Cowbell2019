using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomsToBuy : MonoBehaviour
{

	public Button button;
	public TextMeshProUGUI roomName;
	public TextMeshProUGUI roomPrice;
	public Image roomImage;

    private MySpace.Enums.RoomTypes type;
    private bool isLocked;
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

    public void Setup(MySpace.RoomDefData currentItem)
	{
        type = currentItem.RoomType;
        isLocked = currentItem.Locked;
        roomName.text = currentItem.RoomName;
        roomPrice.text = currentItem.RoomCost.ToString();
        if (currentItem.RoomSprite != null)
            roomImage.sprite = currentItem.RoomSprite;

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
        {
            ClickManager.Ref.BuildClick(type);

            //Close the side panel
            SidePanel.SetPanel(false);
            //CameraScript.ZoomDisabled = false;
        }
            
    }
}
