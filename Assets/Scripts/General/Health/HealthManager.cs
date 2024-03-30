using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{

    [SerializeField] public DamageHealthFD[] m_damageHealthFDs;
    private List<DamageHealthFD> m_inactiveText;
    private List<DamageHealthFD> m_activeText;

    private int m_damageFDMax;
    private int m_textActiveCount =1;

    [SerializeField] public Camera m_cameraReference;
    [SerializeField] public SerieController m_serieController;
    private void Start()
    {
        InitTextFeedback();
    }


  
    public void InitTextFeedback()
    {
        m_damageFDMax = m_damageHealthFDs.Length;
        m_inactiveText = new List<DamageHealthFD>(m_damageFDMax);
        m_activeText = new List<DamageHealthFD>(m_damageFDMax);
        for (int i = 0; i < m_damageHealthFDs.Length; i++)
        {
            if (m_damageHealthFDs[i] == null) continue;

            m_damageHealthFDs[i].SetupText(this);
            m_damageHealthFDs[i].m_cameraToLook = m_cameraReference;
            m_inactiveText.Add(m_damageHealthFDs[i]);
            m_activeText.Add(null);
        }
    }

    public void CallDamageEvent(Vector3 position, float damage)
    {
        if (m_textActiveCount == m_damageFDMax) return;

       if(m_serieController) m_serieController.RefreshSeries(true);
        DamageHealthFD currentDamageFD = m_inactiveText[m_damageFDMax - m_textActiveCount];
        currentDamageFD.StartDamageFeeback(position,damage);

        m_activeText[ m_textActiveCount] = currentDamageFD;
        m_inactiveText[m_damageFDMax - m_textActiveCount] = null;

        m_textActiveCount++;
    }

    public void FinishDamageEvent(DamageHealthFD damageText)
    {
        m_textActiveCount--;
        m_activeText[m_textActiveCount] = null;
        m_inactiveText[m_damageFDMax - m_textActiveCount] = damageText;
    }

}
