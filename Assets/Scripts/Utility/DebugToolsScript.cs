﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using MySpace;



public class DebugToolsScript : MonoBehaviour
{
    public Toggle guestDraggableToggle = null;
    public static bool debugToolsActive = false;
    public GameObject debugElementsUI = null;
    public InputField ifHoots = null;
    public InputField ifMonCoins = null;
	[SerializeField] TMP_Text timerTextComponent;

	[HideInInspector]
    public static DebugToolsScript Ref { get; private set; } // For external access of script


    void Awake()
    {
        if (Ref == null) Ref = GetComponent<DebugToolsScript>();
    }

    private void OnEnable()
    {
        guestDraggableToggle.SetIsOnWithoutNotify(ClickManager.GuestsDraggable);
        guestDraggableToggle.onValueChanged.AddListener(SetGuestDraggable);
    }

    private void OnDisable()
    {
        guestDraggableToggle.onValueChanged.RemoveListener(SetGuestDraggable);
    }

    void Update()
    {
        CheckDebugInputs();
    }

	public void SetTimerText(string textToDisplay)
	{
		timerTextComponent.text = textToDisplay;
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
        //if (!string.IsNullOrWhiteSpace(ifSouls.text))
        //    WalletManager.AddMonCoins(int.Parse(ifSouls.text));
    }

    public void InputField_AddMonCoins()
    {
        if (!string.IsNullOrWhiteSpace(ifMonCoins.text))
            WalletManager.AddMonCoins(int.Parse(ifMonCoins.text));
    }

    public void CreateRandomGuest()
    {
        ClickManager.Ref.Button_Book(GameManager.CreateDefaultGuest());
    }

    public void SetGuestDraggable(bool enabled)
    {
        ClickManager.GuestsDraggable = enabled;
    }




}
