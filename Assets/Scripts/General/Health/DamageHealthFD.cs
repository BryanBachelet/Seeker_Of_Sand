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
    [SerializeField] private Animator m_animation;
    [SerializeField] private float m_animationDuration = 1;
    [SerializeField] public Camera m_cameraToLook;

    public HealthManager m_healthManager;
    private float m_animationTimer;

    public void SetupText(HealthManager healthManager)
    {
        m_healthManager = healthManager;
        m_animation = this.GetComponent<Animator>();
        m_startPosition = transform.position;
    }

    #region Component Function

    public void StartDamageFeeback(Vector3 position, float damage, Color color)
    {
        m_active = true;

        gameObject.SetActive(true);
        gameObject.transform.position = position;
        gameObject.transform.LookAt(transform.position + m_cameraToLook.transform.rotation * Vector3.forward, m_cameraToLook.transform.rotation * Vector3.up);
        m_text.text = damage.ToString("F0");
        m_text.color = color;
        //m_animation.Play("DamageNumberDisplaying");
        StartCoroutine(Animation());
    }

    public IEnumerator Animation()
    {
        while (m_animationTimer< m_animationDuration)
        {
            gameObject.transform.LookAt(transform.position + m_cameraToLook.transform.rotation * Vector3.forward, m_cameraToLook.transform.rotation * Vector3.up);
            //m_text.rectTransform.anchoredPosition += Vector2.up * m_speed * Time.deltaTime;
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
