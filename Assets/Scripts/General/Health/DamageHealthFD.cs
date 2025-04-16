using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DamageHealthFD : MonoBehaviour
{
    [HideInInspector] private Vector3 m_startPosition;
    [HideInInspector] private bool m_active;
    [HideInInspector] private TMP_Text m_text;
    [HideInInspector] private Animator m_animation;
    [SerializeField] private float m_animationDuration = 1;
    [HideInInspector] public Camera m_cameraToLook;

    [HideInInspector] private HealthManager healthManager;
    [HideInInspector] private float m_animationTimer;

    public void SetupText(HealthManager healthManager)
    {
        this.healthManager = healthManager;
        if (m_animation == null) { m_animation = this.GetComponent<Animator>(); }
        if (m_text == null) { m_text = this.GetComponentInChildren<TMP_Text>(); }
        m_startPosition = transform.position;
    }

    #region Component Function

    public void StartDamageFeeback(Vector3 position, float damage, Color color)
    {
        m_active = true;

        gameObject.SetActive(true);
        m_animation.SetTrigger("SendDamage");
        gameObject.transform.position = position;
        gameObject.transform.LookAt(transform.position + m_cameraToLook.transform.rotation * Vector3.forward, m_cameraToLook.transform.rotation * Vector3.up);
        m_text.text = damage.ToString("F0");
        m_text.color = color;
      
        StartCoroutine(Animation());
    }

    public IEnumerator Animation()
    {
        while (m_animationTimer< m_animationDuration)
        {
            gameObject.transform.LookAt(transform.position + m_cameraToLook.transform.rotation * Vector3.forward, m_cameraToLook.transform.rotation * Vector3.up);
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
        m_animation.ResetTrigger("SendDamage");
        gameObject.transform.position = m_startPosition;

        healthManager.FinishDamageEvent(this);
    }

    #endregion

}
