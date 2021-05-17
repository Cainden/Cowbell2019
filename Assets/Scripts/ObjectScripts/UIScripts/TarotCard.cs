using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]
public class TarotCard : MonoBehaviour
{
    private static readonly string CHAR_NAME = "CharacterName";
    private static readonly string CHAR_HEAD_ART = "CharacterHeadArt";
    private static readonly string CHAR_BODY_ART = "CharacterBodyArt";
    private static readonly string BOUNTY = "Tarrot_Hobbies";
    private static readonly string SELECTED_IMAGE = "Tarrot_Select";

    private static readonly string CARD_REVEALED = "CardRevealed";
    private static readonly string IDLE_ANIMATION = "Idle";
    private static readonly string FLIP_CARD = "FlipCard";

    private static Color SELECTED_COLOR = new Color(36.0f / 255.0f, 231.0f / 255.0f, 111.0f / 255.0f);

    private GameObject m_nameInfo;
    private TextMeshPro m_nameText;
    private GameObject m_characterHead;
    private SpriteRenderer m_headSprite;
    private GameObject m_characterBody;
    private SpriteRenderer m_bodySprite;
    private GameObject m_selected;
    private UnityEngine.UI.Image m_selectedImage;
    private Color m_unselectedColor;

    private GameObject m_bountyInfo;
    private TextMeshPro m_bountyText;

    private Animator m_cardAnimator;
    private bool m_flipping;

    private bool m_currentlySelected;

    // Start is called before the first frame update
    void Start()
    {
        FindChildObjectByName(CHAR_NAME, transform, ref m_nameInfo);
        m_nameText = m_nameInfo.GetComponent<TextMeshPro>();

        //Character Art set up
        FindChildObjectByName(CHAR_HEAD_ART, transform, ref m_characterHead);
        m_headSprite = m_characterHead.GetComponent<SpriteRenderer>();
        FindChildObjectByName(CHAR_BODY_ART, transform, ref m_characterBody);
        m_bodySprite = m_characterBody.GetComponent<SpriteRenderer>();

        //Character Info set up
        FindChildObjectByName(BOUNTY, transform, ref m_bountyInfo);
        m_bountyText = m_bountyInfo.GetComponent<TextMeshPro>();
        m_cardAnimator = GetComponent<Animator>();

        //Selected button
        FindChildObjectByName(SELECTED_IMAGE, transform, ref m_selected);
        m_selectedImage = m_selected.GetComponent<UnityEngine.UI.Image>();
        if (m_selectedImage != null)
        {
            m_unselectedColor = m_selectedImage.color;
        }

        m_flipping = false;
        m_currentlySelected = false;
    }

    /// <summary>
    /// Fill out all Tarot Card info.
    /// </summary>
    /// <param name="FillNameText">Name of target.</param>
    /// <param name="FillHeadSprite">Head sprite for target.</param>
    /// <param name="FillBodySprite">Body sprite for target.</param>
    /// <param name="FillBountyText">Bounty text for target.</param>
    public void TarotCardInfo(string FillNameText, Sprite FillHeadSprite, Sprite FillBodySprite, string FillBountyText)
    {
        m_nameText.text = FillNameText;
        m_headSprite.sprite = FillHeadSprite;
        m_bodySprite.sprite = FillBodySprite;
        m_bountyText.text = FillBountyText;
    }

    /// <summary>
    /// Set target name on Tarot Card.
    /// </summary>
    /// <param name="name">Name of target.</param>
    public void SetName(string name)
    {
        m_nameText.text = name;
    }

    /// <summary>
    /// Set bounty text on Tarot Card.
    /// </summary>
    /// <param name="bountyText">Bounty text.</param>
    public void SetBountyText(string bountyText)
    {
        m_bountyText.text = bountyText;
    }

    /// <summary>
    /// Play flip animation for this card.
    /// </summary>
    public void Flip()
    {
        if (m_cardAnimator.GetCurrentAnimatorStateInfo(0).IsName(IDLE_ANIMATION))
        {
            m_flipping = true;
            m_cardAnimator.SetBool(CARD_REVEALED, !m_cardAnimator.GetBool(CARD_REVEALED));
            m_cardAnimator.SetTrigger(FLIP_CARD);
        }
    }

    public void ToggleSelection()
    {
        m_currentlySelected = !m_currentlySelected;

        SetSelectedColor(m_currentlySelected);
    }

    public bool IsSelected()
    {
        return m_currentlySelected;
    }

    /// <summary>
    /// Recursive function to find a child object by the name of the game object.
    /// </summary>
    /// <param name="name">Name of target object.</param>
    /// <param name="transformObject">Parent object to check children of.</param>
    /// <param name="targetObject">Reference to assign with target object.</param>
    private void FindChildObjectByName(string name, Transform transformObject, ref GameObject targetObject)
    {
        if (transformObject.gameObject.name == name)
        {
            targetObject = transformObject.gameObject;
        }

        if (targetObject == null)
        {
            foreach (Transform child in transformObject)
            {
                FindChildObjectByName(name, child, ref targetObject);

                if (targetObject != null && targetObject.name == name)
                {
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Set the color of the selected image based on selection
    /// state.
    /// </summary>
    /// <param name="isSelected">True if selected. Otherwise, false.</param>
    private void SetSelectedColor(bool isSelected)
    {
        if (m_selectedImage != null)
        {
            if (isSelected)
            {
                m_selectedImage.color = SELECTED_COLOR;
            }
            else
            {
                m_selectedImage.color = m_unselectedColor;
            }
        }
    }
}
