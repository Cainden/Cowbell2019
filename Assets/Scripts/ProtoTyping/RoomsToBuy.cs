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

	private Item item;
	private PopulateRoomsToBuy scrollList;


	// Start is called before the first frame update
	void Start()
    {
        
    }

	

	public void Setup(Item currentItem, PopulateRoomsToBuy currentScrollList)
	{
		item = currentItem;
		roomName.text = item.roomName;
		roomPrice.text = item.roomPrice.ToString();
		roomImage.sprite = item.roompic;

		scrollList = currentScrollList;
	}

 
}
