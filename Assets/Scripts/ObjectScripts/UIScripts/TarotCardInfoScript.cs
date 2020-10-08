using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TarotCardInfoScript: MonoBehaviour
{
    #region Inspector Variables
    public GameObject NameInfo;
    private TextMeshProUGUI NameText;
    public GameObject CharacterHead;
    private SpriteRenderer HeadSprite;
    public GameObject CharacterBody;
    private SpriteRenderer BodySprite;

    public GameObject BountyInfo;
    private TextMeshProUGUI BountyText;
    public GameObject LocationInfo;
    private TextMeshProUGUI LocationText;
    #endregion
    // Start is called before the first frame update
    void Awake()
    {
        NameText = NameInfo.GetComponent<TextMeshProUGUI>();
        //Character Art set up
        HeadSprite = CharacterHead.GetComponent<SpriteRenderer>();
        BodySprite = CharacterBody.GetComponent<SpriteRenderer>();
        //Character Info set up
        BountyText = BountyInfo.GetComponent<TextMeshProUGUI>();
        LocationText = LocationInfo.GetComponent<TextMeshProUGUI>();
    }

    public void TarotCardInfo(string FillNameText, Sprite FillHeadSprite, Sprite FillBodySprite, string FillBountyText, string FillLocationText)
    {
        NameText.text = FillNameText;
        HeadSprite.sprite = FillHeadSprite;
        BodySprite.sprite = FillBodySprite;
        BountyText.text = FillBountyText;
        LocationText.text = FillLocationText;
    }
}
