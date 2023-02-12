using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DamageHealthFD : MonoBehaviour
{
    [SerializeField] private Vector3 m_startPosition;
    [SerializeField] private bool m_active;
    [SerializeField] private TMP_Text m_text;
    [SerializeField] private Animation m_animation;
    [SerializeField] private float m_speed = 2;
    [SerializeField] private float m_animationDuration = 1;

    private HealthManager m_healthManager;
    private float m_animationTimer;

    public void SetupText(HealthManager healthManager)
    {
        m_healthManager = healthManager;
        m_startPosition = transform.position;
    }

    #region Component Function

    public void StartDamageFeeback(Vector3 position, float damage)
    {
        m_active = true;

        gameObject.SetActive(true);
        gameObject.transform.position = position;
        m_text.text = damage.ToString("F0");

        m_animation.Play();
        StartCoroutine(Animation());
    }

    public IEnumerator Animation()
    {
        while (m_animationTimer< m_animationDuration)
        {
            m_text.rectTransform.anchoredPosition += Vector2.up * m_speed * Time.deltaTime;
            m_animationTimer += Time.deltaTime;
            yield return Time.deltaTime;
        }
        FinishDamageFeedback();
    }

    public void FinishDamageFeedback()
    {   
        m_active = false;
        m_animationTimer = 0;

        gameObject.SetActive(false);
        gameObject.transform.position = m_startPosition;

        m_healthManager.FinishDamageEvent(this);
    }

    #endregion

}
