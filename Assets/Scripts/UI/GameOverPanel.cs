using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    public void Pause()
    {
        Time.timeScale = 0;
    }
}
