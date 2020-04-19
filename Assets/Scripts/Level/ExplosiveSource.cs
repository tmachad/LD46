using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveSource : MonoBehaviour
{
    [Header("Explosives")]
    public GameObject m_ExplosionPrefab;
    public int m_ExplosionCount;
    public float m_BurstDuration;
    public Bounds m_SpawnArea;
    public int m_EnableSoundInterval;
    public float m_RandomPitchChange;

    [Header("Camera Shake")]
    public bool m_EnableCameraShake;
    public float m_CameraShakeStrength;
    public float m_CameraShakeMaxDistance;

    private GameObject[] m_Explosions;
    private Coroutine m_SpawnCoroutine;

    private void Awake()
    {
        m_Explosions = new GameObject[m_ExplosionCount];
        for (int i = 0; i < m_ExplosionCount; i++)
        {
            m_Explosions[i] = Instantiate(m_ExplosionPrefab, transform);
            m_Explosions[i].SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.TransformPoint(m_SpawnArea.center), m_SpawnArea.size);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_CameraShakeMaxDistance);
    }

    public void Explode()
    {
        if (m_SpawnCoroutine != null)
        {
            StopCoroutine(m_SpawnCoroutine);
        }

        m_SpawnCoroutine = StartCoroutine(SpawnExplosions());

        if (m_EnableCameraShake)
        {
            CameraFollow[] cameraShakers = FindObjectsOfType<CameraFollow>();
            foreach (CameraFollow cs in cameraShakers)
            {
                float distance = Vector2.Distance(transform.position, cs.transform.position);
                cs.m_ShakeStrength = Mathf.Lerp(m_CameraShakeStrength, 0, Mathf.Clamp01(distance / m_CameraShakeMaxDistance));
            }
        }
    }

    private IEnumerator SpawnExplosions()
    {
        float interval = m_BurstDuration / m_Explosions.Length;

        for(int i = 0; i < m_Explosions.Length; i++)
        {
            Vector2 pos = new Vector2(
                Random.Range(m_SpawnArea.min.x, m_SpawnArea.max.x),
                Random.Range(m_SpawnArea.min.y, m_SpawnArea.max.y)
            );
            m_Explosions[i].transform.localPosition = pos;
            AudioSource source = m_Explosions[i].GetComponent<AudioSource>();

            if (i % m_EnableSoundInterval != 0)
            {
                source.enabled = false;
            } else
            {
                source.enabled = true;
                source.pitch = 1 + Random.Range(-m_RandomPitchChange, m_RandomPitchChange);
            }

            m_Explosions[i].SetActive(true);

            yield return new WaitForSeconds(interval);
        }
    }
}
