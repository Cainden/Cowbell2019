using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TarotCardController : MonoBehaviour
{
    private static readonly string FLIP_CARDS = "FlipCards";
    private static readonly string TAROT = "tarrot";
    private static readonly string PROCEED_BUTTON = "Proceed_BTN";

	private List<TarotCard> m_tarotCards;
	private Animator m_tarotCardAnimator;

    private UnityEngine.UI.Button m_proceedButton;
	
	void Start()
    {
        FindAndHideProceedButton();
        GetCardReferences();
        AssignTargets();
        m_tarotCardAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        bool stateChanged = false;

		// If we're in the "Flip Cards" state, Flip
		if (m_tarotCardAnimator.GetCurrentAnimatorStateInfo(0).IsName(FLIP_CARDS))
		{
			foreach (TarotCard tc in m_tarotCards)
			{
				tc.Flip();
			}

			// Set bool to get us out of this state
			m_tarotCardAnimator.SetBool(FLIP_CARDS, false);
		}

        // Select a card
        if (Input.GetMouseButtonDown(0) && !m_tarotCardAnimator.GetBool(FLIP_CARDS))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform != null && hit.transform.gameObject.tag == TAROT)
                {
                    hit.transform.gameObject.GetComponent<TarotCard>().ToggleSelection();
                    stateChanged = true;
                }
            }
        }

        if (stateChanged)
        {
            bool proceedShouldBeActive = CardsAreSelected();
            SetProceedButtonState(proceedShouldBeActive);
        }
    }

    /// <summary>
    /// Find the proceed button and hide it.
    /// </summary>
    private void FindAndHideProceedButton()
    {
        m_proceedButton = GameObject.Find(PROCEED_BUTTON)?.GetComponent<UnityEngine.UI.Button>();
        m_proceedButton.onClick.AddListener(OnProceedClicked);
        SetProceedButtonState(false);
    }

    /// <summary>
    /// Enable or disable the proceed button.
    /// </summary>
    /// <param name="isActive">Enabled if true. Otherwise, it is hidden.</param>
    private void SetProceedButtonState(bool isActive)
    {
        if (m_proceedButton != null)
        {
            m_proceedButton.gameObject.SetActive(isActive);
        }
    }

    /// <summary>
    /// Get references to the tarot cards.
    /// </summary>
    private void GetCardReferences()
    {
        m_tarotCards = new List<TarotCard>();

        foreach (Transform child in transform)
        {
            TarotCard temp = child.GetComponent<TarotCard>();
            if (temp != null)
            {
                m_tarotCards.Add(temp);
            }
        }
    }

    /// <summary>
    /// Checks to see if any tarot cards are currently selected.
    /// </summary>
    /// <returns>True if any are selected. Otherwise, false.</returns>
    private bool CardsAreSelected()
    {
        foreach (TarotCard card in m_tarotCards)
        {
            if(card.IsSelected())
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Assigns 3 random guest targets to the tarot cards.
    /// </summary>
    private void AssignTargets()
    {
        // TODO : Get 3 random targets from the list of active guests.
    }

    /// <summary>
    /// Event handler for when the proceed button is clicked.
    /// </summary>
    public void OnProceedClicked()
    {
        Debug.Log("Clicked proceed");
    }
}
