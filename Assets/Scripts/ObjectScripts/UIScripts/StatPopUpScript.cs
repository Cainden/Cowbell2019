using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatPopUpScript : MonoBehaviour
{
    public Image Sprite;
    public TextMeshProUGUI StatNameText;
    public TextMeshProUGUI StatValueText;

    public void SetComponents(Sprite sprite, string name, string value)
    {
        if (sprite)
            Sprite.sprite = sprite;
        
        StatNameText.text = name;
        StatValueText.text = value;
    }
}
