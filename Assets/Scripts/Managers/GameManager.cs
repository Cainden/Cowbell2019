using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;

//A Manager of game-loop changing functionality between the tycoon-style daytime gameplay and the nighttime gameplay.

public class GameManager : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField] GameObject DebugMenu;

    [SerializeField] Roles roleInfo;
    private static Roles _roleInfo;

    [SerializeField] int startingHoots = 1000;
    #endregion

    //Might not want to do this for the gamemanager?
    #region Singleton Management

    public static GameManager Ref { get; set; }

    private void Awake()
    {
        if (!Ref)
        {
            Ref = this;

            //We don't want this for the gamemanager
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Ref != this)
            {
                Destroy(gameObject);
            }
        }
    }
    #endregion

    private void Start()
    {
        //TimeManager.AddEventTriggerInSeconds(20, GiveGuest);
        TimeManager.AddEventTriggerToGameTime(9, 0, 0, GiveGuest, true);
        DebugMenu.SetActive(false);
        _roleInfo = roleInfo;

        //Might need to change this if loading a save
        WalletManager.SetHoots(1000);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AppManager.Ref.ChangeApplicationState(Enums.AppState.MainMenu);
        }
        if (Input.GetKeyDown(KeyCode.Tilde))
        {
            DebugMenu.SetActive(!DebugMenu.activeInHierarchy);
        }
    }

    private void GiveGuest()
    {
        ClickManager.Ref.AddNewGuest();
        //TimeManager.AddEventTriggerInSeconds(60, GiveGuest);
    }

    public static RoleInfo GetRoleInfo(Enums.ManRole role)
    {
        switch (role)
        {
            case MySpace.Enums.ManRole.Cleaner:
                return _roleInfo.Cleaner;
            default:
                return new RoleInfo();
        }
    }

    
}

namespace MySpace
{
    [System.Serializable]
    public struct Roles
    {
        public RoleInfo Cleaner;
    }

    [System.Serializable]
    public struct RoleInfo
    {
        [Tooltip("Paid Daily")]
        public int income;
    }
}

