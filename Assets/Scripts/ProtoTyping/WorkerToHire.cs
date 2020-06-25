using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MySpace;

public class WorkerToHire : MonoBehaviour
{
    public TextMeshProUGUI protoText1, protoText2;
    public Image workerImage;
	public Button button;
	public GameObject Sidepanel;
	SidePanel sidepanelscript;

    private WorkerConstructionData worker;



    public void Setup(WorkerConstructionData data)
    {
        protoText1.text = data.manFirstName + " " + data.manLastName;
        worker = data;
    }

    public void OnClick()
    {
        ClickManager.Ref.Button_Hire(worker);
		Sidepanel = GameObject.FindGameObjectWithTag("sidePanel");
		sidepanelscript = Sidepanel.GetComponent<SidePanel>();
		sidepanelscript.CloseHireList();

	}
}
