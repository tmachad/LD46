using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public float m_NormalTimeScale = 1.0f;
    public KeyCode m_PauseHotkey = KeyCode.Escape;
    public UnityEvent m_OnPause;
    public UnityEvent m_OnResume;

    private bool m_Paused = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(m_PauseHotkey))
        {
            TogglePause();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0;
        m_Paused = true;
        m_OnPause.Invoke();
    }

    public void Resume()
    {
        Time.timeScale = m_NormalTimeScale;
        m_Paused = false;
        m_OnResume.Invoke();
    }

    public void TogglePause()
    {
        if (m_Paused)
        {
            Resume();
        } else
        {
            Pause();
        }
    }

    public bool Paused()
    {
        return m_Paused;
    }
}
