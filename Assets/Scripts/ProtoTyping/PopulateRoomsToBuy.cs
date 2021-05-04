using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySpace;

//[System.Serializable]
//public struct Item
//{
//	public string roomName;
//	public Sprite roompic;
//	public float roomPrice;
	

//}


public class PopulateRoomsToBuy : MonoBehaviour
{
	//public List<Item> itemlist;
	public Transform contentPanel;
	public RoomsToBuyObjectPool buttonObjectPool, hireObjectPool;

	private PanelContainer m_panelContainer;

    private void Awake()
    {
		m_panelContainer = GetComponent<PanelContainer>();
		m_panelContainer?.RegisterOnPanelPreOpen(OnPanelPreOpen);
		m_panelContainer?.RegisterOnPanelPreClose(OnPanelPreClose);
	}

    public void DisplayRoomsToBuild()
	{
		RefreshList();
		AddRoomButtons();
		//CameraScript.ZoomDisabled = true;
	}

	public void OnPanelPreOpen()
	{
		SetAllChildrenActiveRecursively(true, gameObject);
		DisplayRoomsToBuild();
	}

	public void OnPanelPreClose()
	{
		SetAllChildrenActiveRecursively(false, gameObject);
	}

	protected void SetAllChildrenActiveRecursively(bool activeState, GameObject currentObject)
	{
		currentObject.SetActive(activeState);
		foreach (Transform child in currentObject.transform)
		{
			SetAllChildrenActiveRecursively(activeState, child.gameObject);
		}
	}

	private void AddRoomButtons()
	{
        RoomDefData[] itemList = RoomManager.GetAllRoomsofCategory(new Enums.RoomCategories[4] { Enums.RoomCategories.Miscellaneous , Enums.RoomCategories.Overworld, Enums.RoomCategories.Underworld, Enums.RoomCategories.Utility });

		for (int i = 0; i < itemList.Length; i++)
		{
            if (itemList[i].RoomCost <= 0) //Rooms with a cost of 0 or less should not be displayed as purchasable, these include the lobbies or other pre-built rooms
                continue;
			GameObject newButton = buttonObjectPool.GetObject(); // get object from the pool
			newButton.transform.SetParent(contentPanel, true); // parent the button to the content pool
            newButton.GetComponent<RoomsToBuy>().Setup(itemList[i]);
		}
	}

    private void RefreshList()
    {
        foreach (PooledObject button in contentPanel.GetComponentsInChildren<PooledObject>())
        {
            button.pool.ReturnObject(button.gameObject);
        }
    }
}
