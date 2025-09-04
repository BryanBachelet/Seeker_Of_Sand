using GuerhoubaGames.Character;
using GuerhoubaGames.GameEnum;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private GameObject m_healthUiFeedbackHolder;
    [SerializeField] private List<DamageHealthFD> m_damageHealthFDs = new List<DamageHealthFD>();
    [HideInInspector] private List<DamageHealthFD> m_inactiveText;
    [HideInInspector] private List<DamageHealthFD> m_activeText;

    [HideInInspector] private int m_damageFDMax;
    [HideInInspector] private int m_textActiveCount = 1;

    [HideInInspector] private Camera m_cameraReference;

    [HideInInspector] public CharacterShoot characterShoot;

    [SerializeField] private Color[] elementDamageColor = new Color[4];
    [SerializeField] private GameObject prefab_DamageFD;
    private int currentDamageFDNumber = -1;

    public Vector2 offSet;
    private void Start()
    {
        m_cameraReference = Camera.main;
        GetDamageHealthFDObject(m_healthUiFeedbackHolder);
        characterShoot = GameObject.Find("Player").GetComponent<CharacterShoot>();
    }

    public int GenerateNewDamageFD()
    {
        GameObject newDamage = Instantiate(prefab_DamageFD, transform.position, Quaternion.identity, m_healthUiFeedbackHolder.transform);
        DamageHealthFD newDamageFD = newDamage.GetComponent<DamageHealthFD>();
        m_damageHealthFDs.Add(newDamageFD);
        currentDamageFDNumber += 1;
        InitTextFeedback();
        return currentDamageFDNumber;
    }

    public void GetDamageHealthFDObject(GameObject gameObjectDamageFeedbackHolder)
    {
        //m_damageHealthFDs = new DamageHealthFD[gameObjectDamageFeedbackHolder.transform.childCount];
        //for (int i = 0; i < gameObjectDamageFeedbackHolder.transform.childCount; i++)
        //{
        //    m_damageHealthFDs[i] = m_healthUiFeedbackHolder.transform.GetChild(i).GetComponent<DamageHealthFD>();
        //}
        InitTextFeedback();
    }
    public void InitTextFeedback()
    {
        //m_damageFDMax = m_damageHealthFDs.Count;
        //m_inactiveText = new List<DamageHealthFD>(m_damageFDMax);
        //m_activeText = new List<DamageHealthFD>(m_damageFDMax);
        for (int i = 0; i < m_damageHealthFDs.Count; i++)
        {
            if (m_damageHealthFDs[i] == null) continue;

            m_damageHealthFDs[i].SetupText(this);
            m_damageHealthFDs[i].m_cameraToLook = m_cameraReference;
            //m_inactiveText.Add(m_damageHealthFDs[i]);
            //m_activeText.Add(null);
        }
    }

    public void CallDamageEvent(Vector3 position, float damage, int colorElementType, int ID, Vector2 additionalOffset)
    {
        //OLD if (m_textActiveCount == m_damageFDMax) return;

        DamageHealthFD currentDamageFD = m_damageHealthFDs[ID];
        Tool_DamageMeter.AddDamage(damage);
        float cameraDistance = Vector3.Distance(m_cameraReference.transform.position, position);
        //OLD if (m_textActiveCount % 2 == 0) { position += new Vector3(3, ((100 - cameraDistance) / 100) + 1 * m_textActiveCount, 0); }
        //OLD else { position += new Vector3(-3, ((100 - cameraDistance) / 100) + 1 * m_textActiveCount, 0); }

        if(ID%2 == 0) { position += new Vector3(5 + offSet.x + additionalOffset.x, ((100 - cameraDistance) / 100) + offSet.y + additionalOffset.y, 0); }
        else { position += new Vector3(-5 - offSet.x - additionalOffset.x, ((100 - cameraDistance) / 100) + offSet.y + additionalOffset.y, 0); }

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
        //OLD m_activeText[m_textActiveCount] = currentDamageFD;
        //OLD m_inactiveText[m_damageFDMax - m_textActiveCount] = null;
        //OLD 
        //OLD m_textActiveCount++;
    }

    public void FinishDamageEvent(DamageHealthFD damageText)
    {
        //OLD m_textActiveCount--;
        //OLD m_activeText[m_textActiveCount] = null;
        //OLD 
        //OLD m_inactiveText[m_damageFDMax - m_textActiveCount] = damageText;
    }

}
