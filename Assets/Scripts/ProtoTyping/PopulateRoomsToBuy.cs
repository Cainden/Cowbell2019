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
			//Item item = itemList[i];
			// get object from the pool
			GameObject newButton = buttonObjectPool.GetObject();
			// parent the button to the content pool
			newButton.transform.SetParent(contentPanel, false);
			// tell button to set it's self up
			RoomsToBuy roomstobuy = newButton.GetComponent<RoomsToBuy>();
			roomstobuy.Setup(itemList[i]);


		}
	}

    private void AddWorkerButtons()
    {
        //Worker construction data can contain all information that will need to be displayed to the player about a worker they might want to hire
        foreach (WorkerConstructionData man in ManManager.Ref.hireList)
        {
            GameObject newButton = hireObjectPool.GetObject();

            newButton.transform.SetParent(contentPanel, false);

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
