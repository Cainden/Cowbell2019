using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using MySpace;
using System.Runtime.InteropServices;

public class PauseGameScreen : MonoBehaviour
{
    public GameObject PauseGamePanel;

    private void Update()
    {
        OpenPause();
    }

    private void OpenPause()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            PauseGamePanel.gameObject.SetActive(true);
            PauseSpeed();
        }
    }

    public void ClosePause()
    {
        PauseGamePanel.gameObject.SetActive(false);
        PlaySpeed();
    }

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
}
