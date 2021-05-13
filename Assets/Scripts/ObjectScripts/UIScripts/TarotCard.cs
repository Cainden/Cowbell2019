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

    private GameObject m_nameInfo;
    private TextMeshPro m_nameText;
    private GameObject m_characterHead;
    private SpriteRenderer m_headSprite;
    private GameObject m_characterBody;
    private SpriteRenderer m_bodySprite;

    private GameObject m_bountyInfo;
    private TextMeshPro m_bountyText;

    private Animator m_cardAnimator;
    private bool m_flipping;

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
        //LocationText = LocationInfo.GetComponent<TextMeshProUGUI>();
        m_cardAnimator = GetComponent<Animator>();
        m_flipping = false;

        // TODO : REMOVE THIS!!
        QuickTest();
    }

    private void QuickTest()
    {
        SetName("Zavala");
        SetBountyText("Defeating the Darkness!!");
    }

    private void FindChildObjectByName(string name, Transform transformObject, ref GameObject targetObject)
    {
        if(transformObject.gameObject.name == name)
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

    public void TarotCardInfo(string FillNameText, Sprite FillHeadSprite, Sprite FillBodySprite, string FillBountyText)
    {
        m_nameText.text = FillNameText;
        m_headSprite.sprite = FillHeadSprite;
        m_bodySprite.sprite = FillBodySprite;
        m_bountyText.text = FillBountyText;
    }

    public void SetName(string name)
    {
        m_nameText.text = name;
    }

    public void SetBountyText(string bountyText)
    {
        m_bountyText.text = bountyText;
    }

    public void Flip()
    {
        if (m_cardAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            m_flipping = true;
            m_cardAnimator.SetBool("CardRevealed", !m_cardAnimator.GetBool("CardRevealed"));
            m_cardAnimator.SetTrigger("FlipCard");
        }
    }
}
