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

	public void DisplayRoomsToBuild()
	{
        RefreshList();
		AddRoomButtons();
        //CameraScript.ZoomDisabled = true;
	}

    public void DisplayWorkersToHire()
    {
        RefreshList();
        AddWorkerButtons();
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

    private void AddWorkerButtons()
    {
        //Worker construction data can contain all information that will need to be displayed to the player about a worker they might want to hire
        foreach (WorkerConstructionData man in ManManager.Ref.hireList)
        {
            GameObject newButton = hireObjectPool.GetObject();

            newButton.transform.SetParent(contentPanel, true);

            newButton.GetComponent<WorkerToHire>().Setup(man);
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
