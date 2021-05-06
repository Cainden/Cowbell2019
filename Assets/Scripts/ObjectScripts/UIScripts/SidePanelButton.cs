using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class SidePanelButton : MonoBehaviour
{
    /// <summary>
    /// Used to set the target panel mode.
    /// </summary>
    public enum PanelMode
    {
        UNSET,
        DAY,
        NIGHT
    }

    [SerializeField]
    private PanelContainer m_topLevelDayPanel;

    [SerializeField]
    private PanelContainer m_topLevelNightPanel;

    private PanelMode m_panelMode = PanelMode.UNSET;
    private PanelContainer m_currentTopLevelPanel;
    private UnityEngine.UI.Button m_button;

    void Start()
    {
        m_button = GetComponent<UnityEngine.UI.Button>();
        m_button.onClick.AddListener(OnButtonClick);

        SetPanelMode(PanelMode.DAY); // HACK : This should be set externally
    }

    /// <summary>
    /// Closes all day and night panels.
    /// </summary>
    public void CloseAllOpenPanels()
    {
        m_topLevelDayPanel.Close();
        m_topLevelNightPanel.Close();
    }

    /// <summary>
    /// Sets the target panel mode.
    /// </summary>
    /// <param name="mode">Enumeration defining the target panel mode.
    /// Referenced by SidePanelButton.PanelMode.</param>
    public void SetPanelMode(PanelMode mode)
    {
        if (m_panelMode != mode)
        {
            CloseAllOpenPanels();

            m_currentTopLevelPanel?.UnregisterOnPanelPreClose(OnSidePanelPreClose);

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
                m_currentTopLevelPanel.RegisterOnPanelPreClose(OnSidePanelPreClose);
            }

            m_panelMode = mode;
        }
    }

    /// <summary>
    /// Sets the active state of all children of a given gameobject.
    /// </summary>
    /// <param name="activeState">True for active. Otherwise, false.</param>
    /// <param name="currentObject">Root object to set false, as well as all children
    /// in the hierarchy.</param>
    protected void SetAllPanelsActiveRecursively(bool activeState, GameObject currentObject)
    {
        currentObject.SetActive(activeState);
        foreach (Transform child in currentObject.transform)
        {
            SetAllPanelsActiveRecursively(activeState, child.gameObject);
        }
    }

    /// <summary>
    /// Event hander for registered OnClick event.
    /// </summary>
    public void OnButtonClick()
    {
        m_currentTopLevelPanel?.Open();
    }

    /// <summary>
    /// Event handler for OnPanelPreClose event.
    /// </summary>
    public void OnSidePanelPreClose()
    {
        // TODO : Whatever may be needed in the future.
    }
}
