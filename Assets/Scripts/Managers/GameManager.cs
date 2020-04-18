using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Stress")]
    public float m_MaxStress;
    public float m_Stress;
    public StressProgress m_StressProgressBar;
    private List<StressSource> m_StressSources;

    [Header("Failure Rate")]
    public float m_MinFailureDelay;
    public float m_MaxFailureDelay;
    public float m_TimeToNextFailure;

    private void Awake()
    {
        Instance = this;
        m_StressSources = new List<StressSource>(FindObjectsOfType<StressSource>());
    }

    private void Update()
    {
        if (m_TimeToNextFailure <= 0)
        {
            // Cause a failure somewhere and reset timer
            List<StressSource> inactiveSources = m_StressSources.FindAll((source) => !source.m_Active);
            if (inactiveSources.Count > 0)
            {
                inactiveSources[Random.Range(0, inactiveSources.Count)].Break();
            }
            m_TimeToNextFailure = Random.Range(m_MinFailureDelay, m_MaxFailureDelay);
        } else
        {
            m_TimeToNextFailure -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        // Do all work related to the current amount of stress in late update so all the stress sources can contribute
        m_StressProgressBar.SetProgress(m_Stress / m_MaxStress);

        if (m_Stress >= m_MaxStress)
        {
            Debug.Log("Game Over");
        }
    }
}
