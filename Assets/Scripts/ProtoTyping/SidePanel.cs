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
    }

    public void SetPanelOn()
    {
        openPanel = true;
        collapseBTN.SetActive(false);
        sidepanel.SetBool("Panel_IN", true);
    }

	public void openRooms()
	{
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

	public void OpenHirePanel()
	{
		HirePanel = !HirePanel;

		if (HirePanel)
		{
			workerHirePanel.SetActive(true);
		}
		else
		{
			workerHirePanel.SetActive(false);
		}
	}

	public void OpenWorkerParent()
	{
		Debug.Log("workerParent");
		WorkerParentPanel =! WorkerParentPanel;

		if (WorkerParentPanel)
		{
			workerParent.SetActive(true);
		}
		else
		{
			workerParent.SetActive(false);
		}


	}

    public void MonsterMenu()
    {

    }

    public void BellMenu()
    {

    }
}
