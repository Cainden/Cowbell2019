using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManInfoWindowScript : MonoBehaviour
{
    public Text ManNameText;

    void Start ()
    {
        Deactivate();
        Debug.Assert(ManNameText != null);
    }

    public void Activate(Guid manId)
    {
        ManNameText.text = ManManager.Ref.GetManData(manId).ManScript.ManData.GetManFullName();
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
