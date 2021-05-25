using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;

public class AppManager : Singleton<AppManager>
{
    #region Inspector Variables

    private static readonly string GAME_SCENE = "MainHotelScene";
    private static readonly string MAIN_MENU_SCENE = "MainMenu";

    #endregion

    #region Statics
    public static Enums.AppState CurState;
    #endregion

    #region Singleton Management
    protected override void Awake()
    {
        base.Awake();
        GameObject.DontDestroyOnLoad(this);
    }
    #endregion

    public void ChangeApplicationState(Enums.AppState state)
    {
        switch (state)
        {
            case Enums.AppState.MainMenu:
                SceneManagement.Instance.LoadScene(MAIN_MENU_SCENE,
                                                   UnityEngine.SceneManagement.LoadSceneMode.Single,
                                                   true, null, null, null, false);
                return;
            case Enums.AppState.Game:
                SceneManagement.Instance.LoadScene(GAME_SCENE,
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
