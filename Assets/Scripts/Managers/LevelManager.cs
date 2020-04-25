using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TimeManager))]
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public string[] m_Scenes;

    private string m_ThisScene;

    private void Awake()
    {
        Instance = this;
        m_ThisScene = SceneManager.GetActiveScene().name;
    }

    public void Reload()
    {
        LoadScene(m_ThisScene);
    }

    public void LoadScene(int index)
    {
        LoadScene(m_Scenes[index]);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void LoadScene(string sceneName)
    {
        TimeManager.Instance.Resume();
        SceneManager.LoadScene(sceneName);
    }
}
