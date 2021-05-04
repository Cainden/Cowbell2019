using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class SidePanelButton : MonoBehaviour
{
    public enum PanelMode
    {
        UNSET,
        DAY,
        NIGHT
    }

    public PanelContainer m_topLevelDayPanel;
    public PanelContainer m_topLevelNightPanel;

    private PanelMode m_panelMode = PanelMode.UNSET;
    private PanelContainer m_currentTopLevelPanel;
    private UnityEngine.UI.Button m_button;

    void Start()
    {
        m_button = GetComponent<UnityEngine.UI.Button>();
        SetPanelMode(PanelMode.DAY); // HACK : This should be set externally
    }

    public void CloseAllOpenPanels()
    {
        m_currentTopLevelPanel?.Close();
    }

    public void SetPanelMode(PanelMode mode)
    {
        if (m_panelMode != mode)
        {
            CloseAllOpenPanels();

            m_currentTopLevelPanel?.UnregisterOnPanelPreClose(OnSidePanelClose);

            switch (mode)
            {
                case PanelMode.DAY:
                    m_currentTopLevelPanel = m_topLevelDayPanel;
                    break;
                case PanelMode.NIGHT:
                    m_currentTopLevelPanel = m_topLevelNightPanel;
                    break;
            }

            if (m_currentTopLevelPanel != null)
            {
                SetAllPanelsActiveRecursively(false, m_currentTopLevelPanel.gameObject);
                m_currentTopLevelPanel.RegisterOnPanelPreClose(OnSidePanelClose);
            }
        }
    }

    protected void SetAllPanelsActiveRecursively(bool activeState, GameObject currentObject)
    {
        currentObject.SetActive(activeState);
        foreach (Transform child in currentObject.transform)
        {
            SetAllPanelsActiveRecursively(activeState, child.gameObject);
        }
    }

    public void OnButtonClick()
    {
        m_button.enabled = false;
    }

    public void OnSidePanelClose()
    {
        m_button.enabled = true;
    }
}
