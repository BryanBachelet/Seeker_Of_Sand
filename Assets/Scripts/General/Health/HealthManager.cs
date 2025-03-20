using GuerhoubaGames.GameEnum;
using SeekerOfSand.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private GameObject m_healthUiFeedbackHolder;
    [HideInInspector] private DamageHealthFD[] m_damageHealthFDs;
    [HideInInspector] private List<DamageHealthFD> m_inactiveText;
    [HideInInspector] private List<DamageHealthFD> m_activeText;

    [HideInInspector] private int m_damageFDMax;
    [HideInInspector] private int m_textActiveCount = 1;

    [HideInInspector] private Camera m_cameraReference;

    [HideInInspector] public Character.CharacterShoot characterShoot;

    [SerializeField] private Color[] elementDamageColor = new Color[4];
    private void Start()
    {
        m_cameraReference = Camera.main;
        GetDamageHealthFDObject(m_healthUiFeedbackHolder);
        characterShoot = GameObject.Find("Player").GetComponent<Character.CharacterShoot>();
    }


    public void GetDamageHealthFDObject(GameObject gameObjectDamageFeedbackHolder)
    {
        m_damageHealthFDs = new DamageHealthFD[gameObjectDamageFeedbackHolder.transform.childCount];
        for (int i = 0; i < gameObjectDamageFeedbackHolder.transform.childCount; i++)
        {
            m_damageHealthFDs[i] = m_healthUiFeedbackHolder.transform.GetChild(i).GetComponent<DamageHealthFD>();
        }
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

        DamageHealthFD currentDamageFD = m_inactiveText[m_damageFDMax - m_textActiveCount];
        Tool_DamageMeter.AddDamage(damage);
        float cameraDistance = Vector3.Distance(m_cameraReference.transform.position, position);
        if (m_textActiveCount % 2 == 0) { position += new Vector3(3, ((100 - cameraDistance) / 100) + 1 * m_textActiveCount, 0); }
        else { position += new Vector3(-3, ((100 - cameraDistance) / 100) + 1 * m_textActiveCount, 0); }
        //position += new Vector3(0, ((100 - cameraDistance) / 100) + 1 * m_textActiveCount, 0);
          
        if (colorElementType == (int)GameElement.AIR)
        {
            currentDamageFD.StartDamageFeeback(position, damage, elementDamageColor[1]);
        }
        else if (colorElementType == (int)GameElement.FIRE)
        {
            currentDamageFD.StartDamageFeeback(position, damage, elementDamageColor[2]);
        }
        else if (colorElementType == (int)GameElement.WATER)
        {
            currentDamageFD.StartDamageFeeback(position, damage, elementDamageColor[3]);
        }
        else if (colorElementType == (int)GameElement.EARTH)
        {
            currentDamageFD.StartDamageFeeback(position, damage, elementDamageColor[4]);
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
