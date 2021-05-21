﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;

public class AppManager : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField] string GameScene;
    [SerializeField] string MainMenuScene;

    #endregion

    #region Statics
    public static Enums.AppState CurState;

    //Singleton management.
    public static AppManager Ref { get; set; }
    #endregion

    #region Singleton Management
    private void Awake()
    {
        if (!Ref)
        {
            Ref = this;
            DontDestroyOnLoad(gameObject);
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

    public void ChangeApplicationState(Enums.AppState state)
    {
        switch (state)
        {
            case Enums.AppState.MainMenu:
                SceneManagement.Instance.LoadScene(MainMenuScene,
                                                   UnityEngine.SceneManagement.LoadSceneMode.Single,
                                                   true, null, null, null, false);
                return;
            case Enums.AppState.Game:
                SceneManagement.Instance.LoadScene(GameScene,
                                                   UnityEngine.SceneManagement.LoadSceneMode.Single,
                                                   true,null,null,null,false);
                return;
            default:
                break;
        }
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();

    }
}
