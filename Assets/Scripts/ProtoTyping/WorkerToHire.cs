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
        SidePanel.SetPanel(false);
	}
}
