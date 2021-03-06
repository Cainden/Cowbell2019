﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using MySpace;

public class TimeMenu : MonoBehaviour
{
    //[SerializeField] TMP_Text timerTextComponent;

    [SerializeField] TMP_Text gameSpeedText;

    private void OnEnable()
    {
        GameManager.OnGameSpeedChanged -= SpeedChanged;
        GameManager.OnGameSpeedChanged += SpeedChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameSpeedChanged -= SpeedChanged;
    }

    void SpeedChanged(float timeScale)
    {
        gameSpeedText.text = "x" + Mathf.Round(Mathf.Round(timeScale * 100) * 0.01f);
    }

    public void IncreaseSpeed()
    {
        if (GameManager.GameSpeed >= 1)
            GameManager.GameSpeed += 0.25f;
        else
            GameManager.GameSpeed += 0.1f;
    }

    public void DecreaseSpeed()
    {
        if (GameManager.GameSpeed > 1)
            GameManager.GameSpeed -= 0.25f;
        else
            GameManager.GameSpeed -= 0.1f;
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