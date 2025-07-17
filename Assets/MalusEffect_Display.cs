using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MalusEffect_Display : MonoBehaviour
{
    [SerializeField] private GameObject m_displayHolder;
    [SerializeField] private Image m_Background;
    [SerializeField] private Image m_Icon;
    [SerializeField] private Image m_IconFill;
    [SerializeField] private TMP_Text m_StackDisplay;
    [SerializeField] private float m_Duration = 0;
    [SerializeField] private float m_remainTime = 0;
    [SerializeField] private int m_stack = 0;
    [SerializeField] public bool m_malusActive = false;

    void Update()
    {
        if (!m_malusActive) return;
        else
        {
            if(m_remainTime - Time.deltaTime > 0)
            {
                m_remainTime -= Time.deltaTime;
                m_IconFill.fillAmount = 1 - (m_remainTime / m_Duration);
            }
            else
            {
                m_stack = 0;
                m_remainTime = 0;
                m_IconFill.fillAmount = 1;
                m_malusActive = false;
                m_displayHolder.SetActive(false);
            }

        }
    }

    public void ActivateMalus(float duration, int additionalStack, Sprite iconUse)
    {
        m_Duration = duration;
        m_remainTime = duration;
        m_IconFill.fillAmount = 0;
        m_stack += additionalStack;
        if(m_stack > 25) m_stack = 25;

        m_StackDisplay.text = "" + m_stack;
        m_malusActive = true;
        m_Icon.sprite = iconUse;
        m_IconFill.sprite = iconUse;
        m_displayHolder.SetActive(true);
    }
}
