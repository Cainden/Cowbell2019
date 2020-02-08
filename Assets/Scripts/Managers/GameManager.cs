using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A Manager of game-loop changing functionality between the tycoon-style daytime gameplay and the nighttime gameplay.

public class GameManager : MonoBehaviour
{




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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AppManager.Ref.ChangeApplicationState(MySpace.Enums.AppState.MainMenu);
        }
    }

    private void GiveGuest()
    {
        ClickManager.Ref.AddNewGuest();
        TimeManager.AddEventTriggerInSeconds(60, GiveGuest);
    }
}
