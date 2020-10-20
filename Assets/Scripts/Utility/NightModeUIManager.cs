using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightModeUIManager : MonoBehaviour
{
    public GameObject NightModeStartButton;
    public GameObject NightModeUI;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region ButtonFuntions
    public void StartNightMode()
    {
        //Play night mode
        NightModeUI.gameObject.SetActive(true);
        NightModeStartButton.gameObject.SetActive(false);
    }
    public void TarotCardOpen()
    {
        //Tarot Card Open here
    }
    public void TarotCardClose()
    {
        //Tarot Card Close here
    }
    #endregion
}
