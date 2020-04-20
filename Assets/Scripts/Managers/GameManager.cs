using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Stress")]
    public float m_MaxStress;
    public float m_Stress;
    public float m_StressDecayRate;
    public StressProgress m_StressProgressBar;


    [Header("Stress Source Failure Rate")]
    public float m_MinStressFailureDelay;
    public float m_MaxStressFailureDelay;
    public float m_TimeToNextStressFailure;
    private List<Breakable> m_StressSources;

    [Header("Utility Failure Rate")]
    public float m_MinUtilityFailureDelay;
    public float m_MaxUtilityFailureDelay;
    public float m_TimeToNextUtilityFailure;
    private List<Breakable> m_Utilities;


    private void Awake()
    {
        Instance = this;

        List<Breakable> breakables = new List<Breakable>(FindObjectsOfType<Breakable>());
        m_StressSources = breakables.FindAll((obj) => obj.GetComponent<StressSource>() != null);
        m_Utilities = breakables.FindAll((obj) => obj.GetComponent<StressSource>() == null);
    }

    private void Update()
    {
        if (m_TimeToNextStressFailure <= 0)
        {
            // Cause a failure somewhere and reset timer
            List<Breakable> workingSources = m_StressSources.FindAll((source) => !source.IsBroken());
            if (workingSources.Count > 0)
            {
                workingSources[Random.Range(0, workingSources.Count)].Break();
            }
            m_TimeToNextStressFailure = Random.Range(m_MinStressFailureDelay, m_MaxStressFailureDelay);
        }
        else
        {
            m_TimeToNextStressFailure -= Time.deltaTime;
        }

        if (m_StressSources.TrueForAll((source) => !source.IsBroken()))
        {
            // All stress sources are inactive, stress should decay
            m_Stress = Mathf.Max(0, m_Stress - m_StressDecayRate * Time.deltaTime);
        }

        if (m_TimeToNextUtilityFailure <= 0)
        {
            List<Breakable> workingUtilities = m_Utilities.FindAll((util) => !util.IsBroken());
            if (workingUtilities.Count > 0)
            {
                workingUtilities[Random.Range(0, workingUtilities.Count)].Break();
            }
            m_TimeToNextUtilityFailure = Random.Range(m_MinUtilityFailureDelay, m_MaxUtilityFailureDelay);
        }
        else
        {
            m_TimeToNextUtilityFailure -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        // Do all work related to the current amount of stress in late update so all the stress sources can contribute
        m_StressProgressBar.SetProgress(m_Stress / m_MaxStress);

        if (m_Stress >= m_MaxStress)
        {
            GetComponent<ExplosiveSource>().Explode();
            GameMenuManager.Instance.GameOver();
        }
    }

}
