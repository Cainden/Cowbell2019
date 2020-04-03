using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using MySpace;



public class DebugToolsScript : MonoBehaviour
{

    public static bool debugToolsActive = false;
    public GameObject debugElementsUI = null;
    public InputField ifHoots = null;
    public InputField ifSouls = null;
    public InputField ifMonCoins = null;

    [HideInInspector]
    public static DebugToolsScript Ref { get; private set; } // For external access of script


    void Awake()
    {
        if (Ref == null) Ref = GetComponent<DebugToolsScript>();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CheckDebugInputs();

        //if(ifHoots.text.Length > 0 && Input.GetKeyDown(KeyCode.KeypadEnter))
        //{
        //    InputField_SetHoots();
        //}
        //if (ifSouls.isFocused && Input.GetKeyDown(KeyCode.Return))
        //{
        //    InputField_SetSouls();
        //}
        //if (ifMonCoins.isFocused && Input.GetKeyDown(KeyCode.Return))
        //{
        //    InputField_SetMonCoins();
        //}
    }

    void CheckDebugInputs()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            debugToolsActive = !debugToolsActive;
            
            debugElementsUI.SetActive(debugToolsActive);

        }
    }

    public void SetPanelActive(bool on)
    {
        debugElementsUI.SetActive(on);
        debugToolsActive = on;
    }

    public void InputField_AddHoots()
    {
        if(!string.IsNullOrWhiteSpace(ifHoots.text))
            WalletManager.AddHoots(int.Parse(ifHoots.text));
    }

    public void InputField_AddSouls()
    {
        if (!string.IsNullOrWhiteSpace(ifSouls.text))
            WalletManager.AddHoots(int.Parse(ifSouls.text));
    }

    public void InputField_AddMonCoins()
    {
        if (!string.IsNullOrWhiteSpace(ifMonCoins.text))
            WalletManager.AddHoots(int.Parse(ifMonCoins.text));
    }

    public void CreateRandomGuest()
    {
        GuestConstructionData g = new GuestConstructionData()
        {
            dirtiness = 1,
            manFirstName = NameFactory.GetNewFirstName(),
            manLastName = NameFactory.GetNewLastName(),
            manId = System.Guid.NewGuid(),
            manType = Enums.ManTypes.Guest
        };

        ClickManager.Ref.Button_Book(g);
    }
}
