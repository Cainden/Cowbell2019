using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class SidePanel : MonoBehaviour
{

	public Animator sidepanel;
	
	bool openPanel = false;
	public GameObject roomPanel;
	public GameObject collapseBTN;

	private void Update()
	{
		
		if(openPanel == true)
		{
			
			collapseBTN.SetActive(false);
			sidepanel.SetBool("Panel_IN", true);
		}
		else
		{
			
			sidepanel.SetBool("Panel_IN", false);
			collapseBTN.SetActive(true);
			closeRoom();
		}

	}


	public void openPanelSwitch()
	{
		openPanel = !openPanel;
	}

	public void openRooms()
	{
		roomPanel.SetActive(true);

	}

	public void closeRoom()
	{
		roomPanel.SetActive(false);
	}

	

}
