using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuManager : MonoBehaviour
{
    public static GameMenuManager Instance;

    public GameObject m_PauseMenu;

    private bool m_Paused;

    private void Awake()
    {
        Instance = this;

        Time.timeScale = 1;
        m_PauseMenu.SetActive(false);
        m_Paused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_Paused)
            {
                ResumeGame();
            } else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        m_PauseMenu.SetActive(true);
        m_Paused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        m_PauseMenu.SetActive(false);
        m_Paused = false;
    }
}
