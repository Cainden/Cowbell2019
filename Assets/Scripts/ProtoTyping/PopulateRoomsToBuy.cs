using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Item
{
	public string roomName;
	public Sprite roompic;
	public float roomPrice;
	

}


public class PopulateRoomsToBuy : MonoBehaviour
{
	public List<Item> itemlist;
	public Transform contentPanel;
	public RoomsToBuyObjectPool buttonObjectPool;

	// Start is called before the first frame update
	void Start()
	{
		RefreshDisplay();
	}
	private void RefreshDisplay()
	{
		AddButton();
	}

	private void AddButton()
	{
		for (int i = 0; i < itemlist.Count; i++)
		{
			Item item = itemlist[i];
			// get object from the pool
			GameObject newButton = buttonObjectPool.GetObject();
			// parent the button to the content pool
			newButton.transform.SetParent(contentPanel,false);
			// tell button to set it's self up
			RoomsToBuy roomstobuy = newButton.GetComponent<RoomsToBuy>();
			roomstobuy.Setup(item, this);


		}
	}
}
