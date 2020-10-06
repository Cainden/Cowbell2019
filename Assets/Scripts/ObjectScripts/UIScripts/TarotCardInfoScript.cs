using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TarotCardInfoScript: MonoBehaviour
{
    public GameObject CharacterHead;
    private SpriteRenderer HeadSprite;
    public GameObject CharacterBody;
    private SpriteRenderer BodySprite;

    public GameObject BountyInfo;
    public GameObject LocationInfo;
    private TextMeshProUGUI BountyText;
    private TextMeshProUGUI LocationText;

    // Start is called before the first frame update
    void Start()
    {
        //Character Art set up
        HeadSprite = CharacterHead.GetComponent<SpriteRenderer>();
        BodySprite = CharacterBody.GetComponent<SpriteRenderer>();
        //Character Info set up
        BountyText = BountyInfo.GetComponent<TextMeshProUGUI>();
        LocationText = LocationInfo.GetComponent<TextMeshProUGUI>();
    }

    void TerotCardInfo(Sprite FillHeadSprite, Sprite FillBodySprite, string FillBountyText, string FillLocationText)
    {
        HeadSprite.sprite = FillHeadSprite;
        BodySprite.sprite = FillBodySprite;
        BountyText.text = FillBountyText;
        LocationText.text = FillLocationText;
    }
}
