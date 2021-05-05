using UnityEngine;

public class PanelButton : UnityEngine.UI.Button
{
    private PanelContainer m_panelContainer;
    private PanelContainer m_parentContainer;

    /// <summary>
    /// Activate this button.
    /// </summary>
    public virtual void Show()
    {
        EnableButton(true);
        GetChildContainer();
        SetPanelParent();
        HideSubPanelContainer();
    }

    /// <summary>
    /// Deactivate this button.
    /// </summary>
    public virtual void Hide()
    {
        HideSubPanelContainer();
        EnableButton(false);
    }

    /// <summary>
    /// Set the container that owns this button.
    /// </summary>
    /// <param name="panel">PanelContainer object.</param>
    public void SetPanel(PanelContainer panel)
    {
        m_parentContainer = panel;
    }

    /// <summary>
    /// Hide the child container of this object, if one is present.
    /// </summary>
    /// <returns></returns>
    public bool HideSubPanelContainer()
    {
        if (m_panelContainer != null)
        {
            bool panelIsOpen = m_panelContainer.gameObject.activeInHierarchy;
            m_panelContainer.Close();
            return panelIsOpen;
        }
        return false;
    }

    /// <summary>
    /// Show the child PanelContainer of this object, if one is present.
    /// </summary>
    public void ShowSubPanelContainer()
    {
        if (m_panelContainer != null)
        {
            m_panelContainer.Open();
        }
    }

    /// <summary>
    /// Set the parent reference for the child container of this button.
    /// </summary>
    protected void SetPanelParent()
    {
        m_panelContainer?.SetParentContainer(m_parentContainer);
    }

    /// <summary>
    /// Enable this PanelButton in the hierarchy, as well as any direct child
    /// images or text.
    /// </summary>
    /// <param name="activeState">True to enable the object in the hierarchy.
    /// Otherwise, false.</param>
    protected virtual void EnableButton(bool activeState)
    {
        gameObject.SetActive(activeState);

        foreach (Transform child in transform)
        {
            UnityEngine.UI.Text textTest = child.gameObject.GetComponent<UnityEngine.UI.Text>();
            UnityEngine.UI.Image imageTest = child.gameObject.GetComponent<UnityEngine.UI.Image>();
            if (textTest != null || imageTest != null)
            {
                child.gameObject.SetActive(activeState);
            }
        }
    }

    /// <summary>
    /// Set the child container reference, if one is present.
    /// </summary>
    protected virtual void GetChildContainer()
    {
        foreach (Transform child in transform)
        {
            // This early return allows us to avoid having
            // duplicate null checks before the foreach loop
            // since this is called each time the button is
            // to be shown
            if (m_panelContainer != null)
            {
                return;
            }

            m_panelContainer = child.GetComponent<PanelContainer>();
        }
    }
}
