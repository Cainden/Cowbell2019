using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySpace;
using System;
using TMPro;



public class HiredWokerScrollList : MonoBehaviour
{
	public Transform contentPanel;
	public HiredWorkerPool hiredObjectPool;
	public List<MySpace.ManRef<ManScript_Worker>> hList;
	public GameObject noWorkers;
	public int workerCount = 0;

	public void DisplayHiredWorkers()
	{
		RefreshList();
		AddHiredWorkerButtons();
	}

	private void AddHiredWorkerButtons()
	{
		workerCount = ManManager.Ref.GetAllActiveMenOfType<ManScript_Worker>().Length;
		if (workerCount == 0)
		{
			noWorkers.SetActive(true);
		}
		//	Worker construction data can contain all information that will need to be displayed to the player about a worker they might want to hire
		foreach (MySpace.ManRef<ManScript_Worker> worker in ManManager.Ref.GetAllActiveMenOfType<ManScript_Worker>())
        {
			
			
			noWorkers.SetActive(false);
			
			
			GameObject newButton = hiredObjectPool.GetObject();

                newButton.transform.SetParent(contentPanel, true);

                HiredWorker hiredBTN = newButton.GetComponent<HiredWorker>();
                hiredBTN.Setup(worker, this);

            
        }
		
	


	}

	private void RefreshList()
	{
		foreach (HiredPooledObject button in contentPanel.GetComponentsInChildren<HiredPooledObject>())
		{
			button.pool.ReturnObject(button.gameObject);
		}
	}
}
