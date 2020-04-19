using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public string m_MainMenuScene;

    private string m_ThisScene;

    private void Awake()
    {
        Instance = this;
        m_ThisScene = SceneManager.GetActiveScene().name;
    }

    public void Reload()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(m_ThisScene);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(m_MainMenuScene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
