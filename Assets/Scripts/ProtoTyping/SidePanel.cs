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

	public PopulateRoomsToBuy roomPanel;
	public WorkerScrollList workerToHire;
	public GameObject collapseBTN;
	public GameObject workerParent;
	public GameObject workerHirePanel;
	public List<GameObject> menWorking = new List<GameObject>();

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
		workerToHire.gameObject.SetActive(true);
		workerToHire.DisplayWorkersToHire();
	}
	public void CloseHireList()
	{
		workerToHire.gameObject.SetActive(false);
		//workerToHire.DisplayWorkersToHire();
		HirePanel = !HirePanel;
		closeHirePanel();
	}

	public void OpenHirePanel()
	{
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
		HirePanel = !HirePanel;
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
		var men =  ManManager.Ref.GetAllActiveMenOfType<ManScript_Worker>();

		foreach (MySpace.ManRef<ManScript_Worker> worker in men)
		{
			Debug.Log(ManManager.Ref.GetAllActiveMenOfType<ManScript_Worker>().ToString());
		}

			
		//Debug.Log(ManManager.Ref.GetAllActiveMenOfType<_ManList>());
	}

	public void MonsterMenu()
    {

    }

    public void BellMenu()
    {

    }
}
