using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string[] m_Scenes;

    public void LoadScene(int index)
    {
        if (index >= 0 && index < m_Scenes.Length)
        {
            SceneManager.LoadScene(m_Scenes[index]);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
