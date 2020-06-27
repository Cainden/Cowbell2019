using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MySpace;
using UnityEngine.UI;

public class HiredWorker : MonoBehaviour
{
	public TextMeshProUGUI protoText1, protoText2;
	public Image workerImage;
	public Button button;
	public GameObject Sidepanel;
	SidePanel sidepanelscript;

	private HiredWokerScrollList HworkerScroll;
	private MySpace.ManRef<ManScript_Worker> hItem;

	// Start is called before the first frame update
	public void Setup(MySpace.ManRef<ManScript_Worker> currentItem, HiredWokerScrollList hList)
	{
		hItem = currentItem;
		HworkerScroll = hList;
		//protoText1.text = data.manFirstName + " " + data.manLastName;
		//worker = data;
	}
	
}
