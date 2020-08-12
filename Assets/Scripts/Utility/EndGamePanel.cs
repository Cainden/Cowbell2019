using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGamePanel : MonoBehaviour
{
    public GameObject EndPanel;
    [SerializeField] int maxDays = 3;
    public static int numberOfDays;

    private void Update()
    {
        TimeTillEndGame();
    }
    private void TimeTillEndGame()
    {
        numberOfDays = TimeManager.numberOfDays;
        if (numberOfDays >= maxDays)
        {
            OpenEndGameMenu();
        }
    }
    private void OpenEndGameMenu()
    {
        EndPanel.gameObject.SetActive(true);
        PauseSpeed();
        numberOfDays = 0;
    }

    public void ToMainMenu()
    {
        
        EndPanel.gameObject.SetActive(false);
        PlaySpeed();
        LoadCurrentScene();
    }
    #region pause & play
    public void PauseSpeed()
    {
        if (GameManager.GameSpeed != 0) //if speed is not stoped
            GameManager.GameSpeed = 0f; //set speed to stop.
    }
    public void PlaySpeed()
    {
        if (GameManager.GameSpeed != 1) //if speed is not normal game speed
            GameManager.GameSpeed = 1f; //set speed to normal game speed
    }
    #endregion
    private static void LoadCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex - 1);
    }
}
