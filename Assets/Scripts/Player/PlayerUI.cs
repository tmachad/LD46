using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class PlayerUI : MonoBehaviour
{
    private struct Arrow
    {
        public GameObject arrow;
        public Image arrowImage;
        public GameObject target;
    }

    public GameObject m_BigArrowPrefab;
    public GameObject m_SmallArrowPrefab;

    public float m_Radius = 3f;
    public float m_ArrowOffDistance = 2f;

    private Arrow[] m_Arrows;


    private Canvas m_Canvas;

    private void Awake()
    {
        m_Canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        Breakable[] breakables = FindObjectsOfType<Breakable>();
        m_Arrows = new Arrow[breakables.Length];

        for(int i = 0; i < breakables.Length; i++)
        {
            Breakable b = breakables[i];
            Arrow arrow = new Arrow();
            if (b.IsCritical())
            {
                arrow.arrow = Instantiate(m_BigArrowPrefab, transform);
            } else
            {
                arrow.arrow = Instantiate(m_SmallArrowPrefab, transform);
            }
            arrow.arrowImage = arrow.arrow.GetComponent<Image>();
            arrow.target = b.gameObject;

            b.m_OnBreak.AddListener(() =>
            {
                arrow.arrow.SetActive(true);
            });
            b.m_OnFix.AddListener(() =>
            {
                arrow.arrow.SetActive(false);
            });

            arrow.arrow.SetActive(false);

            m_Arrows[i] = arrow;
        }
    }

    private void Update()
    {
        foreach(Arrow a in m_Arrows)
        {
            if (a.arrow.activeInHierarchy)
            {
                Vector2 direction = a.target.transform.position - transform.position;
                direction.Normalize();

                a.arrow.transform.localPosition = direction * m_Radius * m_Canvas.referencePixelsPerUnit;
                a.arrow.transform.LookAt(a.target.transform, Vector3.forward);

                a.arrowImage.enabled = Vector2.Distance(a.arrow.transform.position, a.target.transform.position) > m_ArrowOffDistance;
            }
        }
    }
}
