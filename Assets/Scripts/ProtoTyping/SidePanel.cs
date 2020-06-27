using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class SidePanel : MonoBehaviour
{
    //Give all scripts easy access to close the panel if they need to.
    private static System.Action<bool> PanelEvent;

    /// <summary>
    /// Global Access to enable or disable the menu UI panel on the left side of the screen.
    /// </summary>
    /// <param name="value">True enables the panel while false disables it.</param>
    public static void SetPanel(bool value)
    {
        PanelEvent?.Invoke(value);
    }

	public Animator sidepanel;
	
	bool openPanel = false;
	bool WorkerParentPanel = false;
	bool HirePanel = false;
	bool hiredPanle = false;

	public GameObject HiredPanel;
	public PopulateRoomsToBuy roomPanel;
	public WorkerScrollList workerToHire;
	public GameObject collapseBTN;
	public GameObject workerParent;
	public GameObject workerHirePanel;
	public List<GameObject> menWorking = new List<GameObject>();
	public HiredWokerScrollList hiredworkerScrollList;


	private void OnEnable()
    {
        sidepanel.speed = 2;
        PanelEvent -= SetPanelBool;
        PanelEvent += SetPanelBool;
    }

    private void OnDisable()
    {
        PanelEvent -= SetPanelBool;
    }

    public void SetPanelOff()
    {
		//openPanel = false;
	
		sidepanel.SetBool("Panel_IN", false);
        collapseBTN.SetActive(true);
        closeRoom();
		CloseHireList();
		closeWorkerParent();
		closeHiredPanel();

	}


	


    public void SetPanelOn()
    {
        openPanel = true;
        collapseBTN.SetActive(false);
        sidepanel.SetBool("Panel_IN", true);
    }

	public void openRooms()
	{
		closeWorkerParent();
		closeHirePanel();
		CloseHireList();
		closeHiredPanel();

		roomPanel.gameObject.SetActive(true);
        roomPanel.DisplayRoomsToBuild();
	}

	public void closeRoom()
	{
		roomPanel.gameObject.SetActive(false);
	}

	public void SetPanelBool(bool b)
    {
        if (b)
            SetPanelOn();
        else
            SetPanelOff();
    }

	public void OpenHireList()
	{

		closeRoom();
		workerToHire.gameObject.SetActive(true);
		workerToHire.DisplayWorkersToHire();
	}
	public void CloseHireList()
	{
		
		workerToHire.gameObject.SetActive(false);
		//workerToHire.DisplayWorkersToHire();
		HirePanel = false;
		closeHirePanel();
	}

	public void OpenHirePanel()
	{
		closeHiredPanel();
		//Debug.Log(HirePanel);
		HirePanel = !HirePanel;
		if (HirePanel == false)
		{
			CloseHireList();

		}
		else
		{
			workerHirePanel.SetActive(true);
			
			OpenHireList();
		}
		
		
	}
	public void closeHirePanel()
	{
		
		workerHirePanel.SetActive(false);	

	}

	public void OpenWorkerParent()
	{
		closeRoom();
		WorkerParentPanel = !WorkerParentPanel;

		if (WorkerParentPanel)
		{
			workerParent.SetActive(true);
			
		}
		else
		{
			closeWorkerParent();
			//workerParent.SetActive(false);
		}
	

	}
	public void closeWorkerParent()
	{
		WorkerParentPanel = false;
		workerParent.SetActive(false);

	}

	public void hiredList()
	{
		CloseHireList();
		closeRoom();
		hiredPanle = !hiredPanle;
		
		
		if(hiredPanle == true)
		{
			HiredPanel.SetActive(true);
			hiredworkerScrollList.DisplayHiredWorkers();
		}
		else
		{
			hiredPanle = false;
			closeHiredPanel();
		}



		//Debug.Log(ManManager.Ref.GetAllActiveMenOfType<_ManList>());
	}

	private void closeHiredPanel()
	{
		HiredPanel.SetActive(false);
		hiredPanle = false;
	}

	public void MonsterMenu()
    {

    }

    public void BellMenu()
    {

    }
}
