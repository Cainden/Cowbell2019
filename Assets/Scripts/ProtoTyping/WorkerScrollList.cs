using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySpace;
using System;

public class WorkerScrollList : MonoBehaviour
{
	public Transform contentPanel;
	public WorkerToHireObjectPool hireObjectPool;



	public void DisplayWorkersToHire()
	{
		RefreshList();
		AddWorkerButtons();
	}

	private void AddWorkerButtons()
	{

			//	Worker construction data can contain all information that will need to be displayed to the player about a worker they might want to hire
			foreach (WorkerConstructionData man in ManManager.Ref.hireList)
		{
			

			GameObject newButton = hireObjectPool.GetObject();

			newButton.transform.SetParent(contentPanel, true);

			newButton.GetComponent<WorkerToHire>().Setup(man);


		}
	}

	private void RefreshList()
	{
		foreach (WorkerPooledObject button in contentPanel.GetComponentsInChildren<WorkerPooledObject>())
		{
			button.pool.ReturnObject(button.gameObject);
		}
	}
}
