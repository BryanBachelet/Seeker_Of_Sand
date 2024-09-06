using GuerhoubaGames.GameEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{

    [SerializeField] public DamageHealthFD[] m_damageHealthFDs;
    private List<DamageHealthFD> m_inactiveText;
    private List<DamageHealthFD> m_activeText;

    private int m_damageFDMax;
    private int m_textActiveCount = 1;

    [SerializeField] public Camera m_cameraReference;
    [SerializeField] public SerieController m_serieController;

    public Character.CharacterShoot characterShoot;

    public Color[] elementDamageColor = new Color[4];
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

    public void CallDamageEvent(Vector3 position, float damage, int colorElementType)
    {
        if (m_textActiveCount == m_damageFDMax) return;

        if (m_serieController) m_serieController.RefreshSeries(true);
        DamageHealthFD currentDamageFD = m_inactiveText[m_damageFDMax - m_textActiveCount];
        Tool_DamageMeter.AddDamage(damage);


        if (colorElementType == (int)GameElement.AIR)
        {
            currentDamageFD.StartDamageFeeback(position, damage, elementDamageColor[0]);
        }
        else if (colorElementType == (int)GameElement.FIRE)
        {
            currentDamageFD.StartDamageFeeback(position, damage, elementDamageColor[1]);
        }
        else if (colorElementType == (int)GameElement.WATER)
        {
            currentDamageFD.StartDamageFeeback(position, damage, elementDamageColor[2]);
        }
        else if (colorElementType == (int)GameElement.EARTH)
        {
            currentDamageFD.StartDamageFeeback(position, damage, elementDamageColor[3]);
        }
        else
        {
            currentDamageFD.StartDamageFeeback(position, damage, elementDamageColor[0]);
        }




        m_activeText[m_textActiveCount] = currentDamageFD;
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
