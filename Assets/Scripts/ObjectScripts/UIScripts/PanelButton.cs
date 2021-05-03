using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelButton : UnityEngine.UI.Button
{
    private PanelContainer m_panelContainer;

    public virtual void Show()
    {
        EnableButton(true);
        GetChildContainer();
        HideSubPanelContainer();
    }

    public virtual void Hide()
    {
        HideSubPanelContainer();
        EnableButton(false);
    }

    protected virtual void EnableButton(bool activeState)
    {
        gameObject.SetActive(activeState);

        foreach (Transform child in transform)
        {
            // HACK : Clean this up to improve performance
            UnityEngine.UI.Text textTest = child.gameObject.GetComponent<UnityEngine.UI.Text>();
            UnityEngine.UI.Image imageTest = child.gameObject.GetComponent<UnityEngine.UI.Image>();
            if (textTest != null || imageTest != null)
            {
                child.gameObject.SetActive(activeState);
            }
        }
    }

    protected virtual void GetChildContainer()
    {
        foreach (Transform child in transform)
        {
            // This early return allows us to avoid having
            // duplicate null checks before the foreach loop
            if (m_panelContainer != null)
            {
                return;
            }

            m_panelContainer = child.GetComponent<PanelContainer>();
        }
    }

    public bool HideSubPanelContainer()
    {
        if(m_panelContainer != null)
        {
            bool panelIsOpen = m_panelContainer.gameObject.activeInHierarchy;
            m_panelContainer.Close();
            return panelIsOpen;
        }
        else
        {
            Debug.Log("PanelContainer is null for " + gameObject.name);
        }
        return false;
    }

    public void ShowSubPanelContainer()
    {
        if (m_panelContainer != null)
        {
            m_panelContainer.Open();
        }
        else
        {
            Debug.Log("PanelContainer is null for " + gameObject.name);
        }
    }
}
