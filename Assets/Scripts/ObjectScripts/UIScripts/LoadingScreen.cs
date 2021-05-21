using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LoadingScreen : MonoBehaviour
{
    private Camera m_camera;
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        ShowLoadingScreen(false);
    }

    /// <summary>
    /// Show/Hide the loading screen.
    /// </summary>
    /// <param name="shouldShow">True to show the screen. False to hide it.</param>
    public void ShowLoadingScreen(bool shouldShow)
    {
        if(m_camera == null)
        {
            m_camera = GetComponent<Camera>();
        }

        m_camera.enabled = shouldShow;
        transform.GetChild(0).gameObject.SetActive(shouldShow);
    }
}
