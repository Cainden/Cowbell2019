using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManPopUpScript : MonoBehaviour
{
    public Image Sprite;
    public TextMeshProUGUI ManNameText;

    public void SetComponents(Sprite sprite, string nameText)
    {
        ManNameText.text = nameText;
        if (sprite != null)
            Sprite.sprite = sprite;
    }
}
