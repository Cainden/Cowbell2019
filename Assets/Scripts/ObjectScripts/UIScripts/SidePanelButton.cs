using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidePanelButton : MonoBehaviour
{
    public PanelContainer m_topLevelPanel;

    void Start()
    {
        SetAllPanelsActiveRecursively(false, m_topLevelPanel.gameObject);
    }

    public void SetAllPanelsActiveRecursively(bool activeState, GameObject currentObject)
    {
        currentObject.SetActive(activeState);
        foreach (Transform child in currentObject.transform)
        {
            SetAllPanelsActiveRecursively(activeState, child.gameObject);
        }
    }
}
