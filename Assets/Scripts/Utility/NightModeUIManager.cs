using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightModeUIManager : MonoBehaviour
{
    public GameObject NightModeStartButton;
    public GameObject NightModeUI;
    public GameObject TarotPrefab;
    public GameObject[] AvailableTarotCards;
    public GameObject TarotCardParent;
    public int TarotCardNumber = 5;

    // Start is called before the first frame update
    void Start()
    {
        // TODO : Register for night-mode trigger event
    }
    // Update is called once per frame
    void Update()
    {

    }

    private void CreateTarotCards()
    {
        AvailableTarotCards = new GameObject[TarotCardNumber];
        for (int i = 0; i < TarotCardNumber; i++)
        {
            GameObject TarotCardCopy = Instantiate(TarotPrefab);
            TarotCardCopy.transform.parent = TarotCardParent.transform;
            TarotCardCopy.transform.localPosition = Vector3.zero;
            TarotCardCopy.transform.localScale = Vector3.one;
            AvailableTarotCards[i] = TarotCardCopy;
        }
        //ManManager.Ref.GetAllActiveMenOfType<ManScript_Guest>();
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
        CreateTarotCards();
        //Tarot Card Open here
    }
    public void TarotCardClose()
    {
        //Tarot Card Close here
    }
    #endregion
}