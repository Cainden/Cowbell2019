using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;



public class DebugToolsScript : MonoBehaviour
{

    public bool debugToolsActive = false;
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
            
            debugElementsUI.SetActive(!debugElementsUI.activeInHierarchy);

        }
    }

   public void InputField_SetHoots()
    {
        if(!string.IsNullOrWhiteSpace(ifHoots.text))
            WalletManager.Ref.Hoots = int.Parse( ifHoots.text );
    }

   public void InputField_SetSouls()
    {
        if (!string.IsNullOrWhiteSpace(ifSouls.text))
            WalletManager.Ref.Souls = int.Parse(ifSouls.text);
    }

   public void InputField_SetMonCoins()
    {
        if (!string.IsNullOrWhiteSpace(ifMonCoins.text))
            WalletManager.Ref.MonCoins = int.Parse(ifMonCoins.text);
    }
}
