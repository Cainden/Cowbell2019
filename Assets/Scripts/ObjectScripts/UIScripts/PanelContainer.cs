using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnPanelPreOpen();
public delegate void OnPanelPreClose();

public class PanelContainer : UnityEngine.UI.Image
{
    [SerializeField]
    private float m_animationSpeed = 2.0f;

    private static readonly string TRANSITION_IN_ANIMATION = "Panel_IN";

    private Animator m_animator;
    private List<PanelButton> m_panelButtons;

    private event OnPanelPreOpen m_onPanelPreOpen;
    private event OnPanelPreClose m_onPanelPreClose;

    public PanelContainer Parent { get; private set; }

    public bool PanelIsOpen { get; private set; }

    /// <summary>
    /// Open the PanelContainer.
    /// </summary>
    public virtual void Open()
    {
        gameObject.SetActive(true);

        PanelIsOpen = true;

        if(m_onPanelPreOpen != null)
        {
            m_onPanelPreOpen();
        }

        Show();
        HideAllSubPanels();
    }

    /// <summary>
    /// Close the PanelContainer.
    /// </summary>
    public virtual void Close()
    {
        HideButtons();

        if (m_onPanelPreClose != null)
        {
            m_onPanelPreClose();
        }

        Hide();

        PanelIsOpen = false;
    }

    /// <summary>
    /// Close this panel, as well as a given number of parent
    /// panels.
    /// </summary>
    /// <param name="parentPanelsToClose">Number of parents in the hierarchy
    /// to close.</param>
    public virtual void CloseParents(int parentPanelsToClose)
    {
        if(parentPanelsToClose == 0 || parentPanelsToClose < -1)
        {
            return;
        }

        PanelContainer target = Parent;
        int parentCounter = 1;

        while (target.Parent != null && ((parentCounter < parentPanelsToClose) ||
                                         (parentPanelsToClose == -1)))
        {
            target = target.Parent;
        }

        target?.Close();
    }

    /// <summary>
    /// Set the parent PanelContainer reference for this object.
    /// </summary>
    /// <param name="panel">PanelContainer reference.</param>
    public void SetParentContainer(PanelContainer panel)
    {
        Parent = panel;
    }

    /// <summary>
    /// Event handler for all PanelButtons owned by this
    /// PanelContainer.
    /// </summary>
    /// <param name="clickedPanelButton">PanelButton reference of
    /// clicked button.</param>
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
        if (clickedButtonWasLastActive == false)
        {
            clickedPanelButton.ShowSubPanelContainer();
        }
    }

    /// <summary>
    /// Register handler for OnPanelPreOpen events.
    /// </summary>
    /// <param name="handler">OnPanelPreOpen event handler.</param>
    public void RegisterOnPanelPreOpen(OnPanelPreOpen handler)
    {
        if (handler != null)
        {
            m_onPanelPreOpen += handler;
        }
    }

    /// <summary>
    /// Unregister handler for OnPanelPreOpen events.
    /// </summary>
    /// <param name="handler">OnPanelPreOpen event handler.</param>
    public void UnregisterOnPanelPreOpen(OnPanelPreOpen handler)
    {
        if (handler != null)
        {
            m_onPanelPreOpen -= handler;
        }
    }

    /// <summary>
    /// Register handler for OnPanelPreClose events.
    /// </summary>
    /// <param name="handler">OnPanelPreClose event handler.</param>
    public void RegisterOnPanelPreClose(OnPanelPreClose handler)
    {
        if (handler != null)
        {
            m_onPanelPreClose += handler;
        }
    }

    /// <summary>
    /// Unregister handler for OnPanelPreClose events.
    /// </summary>
    /// <param name="handler">OnPanelPreClose event handler.</param>
    public void UnregisterOnPanelPreClose(OnPanelPreClose handler)
    {
        if (handler != null)
        {
            m_onPanelPreClose -= handler;
        }
    }

    /// <summary>
    /// Show the PanelContainer.
    /// </summary>
    protected virtual void Show()
    {
        Init();
        ShowButtons();
        SetAnimationBool(TRANSITION_IN_ANIMATION, true, m_animationSpeed);
    }

    /// <summary>
    /// Hide the PanelContainer.
    /// </summary>
    protected virtual void Hide()
    {
        SetAnimationBool(TRANSITION_IN_ANIMATION, false, m_animationSpeed);

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(PerformClose());
        }
    }

    /// <summary>
    /// Delayed disable of the PanelContainer object to allow for
    /// animated transitions.
    /// </summary>
    /// <returns>IEnumerator</returns>
    IEnumerator PerformClose()
    {
        while (IsPanelAnimating(TRANSITION_IN_ANIMATION))
        {
            yield return null;
        }

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Show all PanelButtons owned by this container.
    /// </summary>
    protected void ShowButtons()
    {
        foreach (PanelButton panelButton in m_panelButtons)
        {
            panelButton.Show();
        }
    }

    /// <summary>
    /// Hide all PanelButtons owned by this container.
    /// </summary>
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

    /// <summary>
    /// Hide all sub PanelContainer objects.
    /// </summary>
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

    /// <summary>
    /// Initialize this object.
    /// </summary>
    protected void Init()
    {
        GetButtonReferences();
    }

    /// <summary>
    /// Get references to all PanelButton objects owned by this
    /// PanelContainer.
    /// </summary>
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

                if (panelButton != null)
                {
                    panelButton.onClick.AddListener(() => OnButtonClicked(panelButton));
                    panelButton.SetPanel(this);

                    if (panelButton != null)
                    {
                        m_panelButtons.Add(panelButton);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Set a bool and animation speed for a given animation.
    /// </summary>
    /// <param name="label">Name of the animation.</param>
    /// <param name="boolValue">Value to set.</param>
    /// <param name="speed">Sets animation speed.</param>
    protected void SetAnimationBool(string label, bool boolValue, float speed)
    {
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

    /// <summary>
    /// Returns true if animation is still playing.
    /// </summary>
    /// <param name="animationLabel">Name of animation.</param>
    /// <returns>True if playing. Otherwise, false.</returns>
    public bool IsPanelAnimating(string animationLabel)
    {
        if (m_animator != null)
        {
            return m_animator.GetCurrentAnimatorStateInfo(0).IsName(animationLabel);
        }

        return false;
    }
}
