using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A Manager of game-loop changing functionality between the tycoon-style daytime gameplay and the nighttime gameplay.

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject DebugMenu;



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
        TimeManager.AddEventTriggerInSeconds(20, GiveGuest);
        DebugMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AppManager.Ref.ChangeApplicationState(MySpace.Enums.AppState.MainMenu);
        }
        if (Input.GetKeyDown(KeyCode.Tilde))
        {
            DebugMenu.SetActive(!DebugMenu.activeInHierarchy);
        }
    }

    private void GiveGuest()
    {
        ClickManager.Ref.AddNewGuest();
        TimeManager.AddEventTriggerInSeconds(60, GiveGuest);
    }
}
