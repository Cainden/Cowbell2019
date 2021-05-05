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

	private PanelContainer m_panelContainer;

	public void Awake()
	{
		m_panelContainer = GetComponent<PanelContainer>();
		m_panelContainer?.RegisterOnPanelPreOpen(OnPanelPreOpen);
		m_panelContainer?.RegisterOnPanelPreClose(OnPanelPreClose);
	}

	public void DisplayHiredWorkers()
	{
		RefreshList();
		AddHiredWorkerButtons();
	}

	public void OnPanelPreOpen()
	{
		SetAllChildrenActiveRecursively(true, gameObject);
		DisplayHiredWorkers();
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
