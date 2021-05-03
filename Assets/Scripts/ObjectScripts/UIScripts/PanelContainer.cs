using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelContainer : UnityEngine.UI.Image
{
    [SerializeField]
    private float m_animationSpeed = 2.0f;

    private Animator m_animator;
    private List<PanelButton> m_panelButtons;

    public bool PanelIsOpen { get; private set; }

    public virtual void Open()
    {
        gameObject.SetActive(true);

        PanelIsOpen = true;

        // Show the panel
        Show();

        // Close any sub planels
        HideAllSubPanels();
    }

    public virtual void Close()
    {
        HideButtons();

        Hide();

        PanelIsOpen = false;
    }

    protected virtual void Show()
    {
        Init();
        ShowButtons();

        // play any universal transition on here
        SetAnimationBool("Panel_IN", true, m_animationSpeed);
    }

    protected virtual void Hide()
    {
        // TODO : play any universal transition on here
        SetAnimationBool("Panel_IN", false, m_animationSpeed);

        gameObject.SetActive(false);
    }

    protected void ShowButtons()
    {
        foreach (PanelButton panelButton in m_panelButtons)
        {
            panelButton.Show();
        }
    }

    protected void HideButtons()
    {
        if (m_panelButtons != null)
        {
            foreach (PanelButton panelButton in m_panelButtons)
            {
                panelButton.Hide();
            }
        }
    }

    protected void HideAllSubPanels()
    {
        if (m_panelButtons != null)
        {
            foreach (PanelButton panelButton in m_panelButtons)
            {
                panelButton.HideSubPanelContainer();
            }
        }
    }

    protected void Init()
    {
        GetButtonReferences();
    }

    protected void GetButtonReferences()
    {
        if(m_panelButtons == null)
        {
            m_panelButtons = new List<PanelButton>();
        }

        if (m_panelButtons.Count == 0)
        {
            foreach (Transform child in transform)
            {
                PanelButton panelButton = child.GetComponent<PanelButton>();
                panelButton.onClick.AddListener(() => OnButtonClicked(panelButton));

                if (panelButton != null)
                {
                    m_panelButtons.Add(panelButton);
                }
            }

            Debug.Log(m_panelButtons.Count + " buttons added to PanelContainer");
        }
    }

    protected void SetAnimationBool(string label, bool boolValue, float speed)
    {
        // TODO : Move this somewhere else!!
        if (m_animator == null)
        {
            m_animator = GetComponent<Animator>();
        }

        if (m_animator != null)
        {
            m_animator.speed = speed;
            m_animator.SetBool(label, boolValue);
        }
    }

    public bool IsPanelAnimating(string animationLabel)
    {
        // TODO : optimize this
        if (m_animator != null)
        {
            return m_animator.GetCurrentAnimatorStateInfo(0).IsName(animationLabel);
        }

        return false;
    }

    public void OnButtonClicked(PanelButton clickedPanelButton)
    {
        bool clickedButtonWasLastActive = false;

        // Hide all subpanels
        foreach (PanelButton panelButton in m_panelButtons)
        {
            if (panelButton.HideSubPanelContainer())
            {
                if (panelButton == clickedPanelButton)
                {
                    clickedButtonWasLastActive = true;
                }
            }
        }

        // Attempt to open the subpanel for the clicked button
        if(clickedButtonWasLastActive == false)
        {
            clickedPanelButton.ShowSubPanelContainer();
        }

        Debug.Log("Clicked button was already active = " + clickedButtonWasLastActive);
    }
}
