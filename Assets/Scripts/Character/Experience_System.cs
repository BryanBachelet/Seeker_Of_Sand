using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Experience_System : MonoBehaviour, CharacterComponent
{
    [SerializeField] private AnimationCurve m_ExperienceQuantity;
    [SerializeField] private float m_NumberEnemyKilled = 0;
    [SerializeField] private int m_CurrentLevel = 1;

    [SerializeField] private Image m_LevelDisplayFill;
    [SerializeField] private float m_RadiusPickupXp;
    [SerializeField] private bool m_ActiveGizmo;
    [SerializeField] private LayerMask m_ExperienceLayer;
    [SerializeField] private float m_posXInit = -930;
    [SerializeField] private float m_posXFinal = 950;
    [SerializeField] private RectTransform m_xpPointer;

    private CharacterUpgrade m_characterUpgrade;
    private CharacterProfile m_characterProfile;
    // Start is called before the first frame update
    void Start()
    {
        m_characterUpgrade = GetComponent<CharacterUpgrade>();
        m_characterProfile = GetComponent<CharacterProfile>();
    }

    public void InitComponentStat(CharacterStat stat)
    {
        m_RadiusPickupXp = stat.attrackness;
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] experienceTouched = Physics.OverlapSphere(transform.position, m_RadiusPickupXp, m_ExperienceLayer);
        if (experienceTouched.Length > 0)
        {
            for (int i = 0; i < experienceTouched.Length; i++)
            {
                experienceTouched[i].GetComponent<ExperienceMouvement>().playerPosition = this.transform;
            }
        }
    }

    public void OnEnemyKilled()
    {
        m_NumberEnemyKilled += 1;
        float levelProgress = m_ExperienceQuantity.Evaluate(m_NumberEnemyKilled);
        if (levelProgress > m_CurrentLevel + 1)
        {
            LevelUp((int)m_ExperienceQuantity.Evaluate(m_NumberEnemyKilled));
            GlobalSoundManager.PlayOneShot(7, Vector3.zero);
        }
        else
        {
            //Debug.Log("Progression is : " + (levelProgress - m_CurrentLevel) + "%");
            m_LevelDisplayFill.fillAmount = (levelProgress - m_CurrentLevel);
            //m_xpPointer.anchoredPosition = new Vector3(Mathf.Lerp(m_posXInit, m_posXFinal, (levelProgress - m_CurrentLevel)), -520, 0);
        }
    }

    public void LevelUp(int newLevel)
    {
        for (int i = 0; i < newLevel - m_CurrentLevel; i++)
        {
            ChooseUpgrade(m_CurrentLevel + i);
        }
        m_CurrentLevel = newLevel;

    }

    public void ChooseUpgrade(int level)
    {
        //Debug.Log("Add new upgrade : " + level);
        m_characterUpgrade.GainLevel();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, m_RadiusPickupXp);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Experience")
        {
            Destroy(collision.gameObject);
            GlobalSoundManager.PlayOneShot(3, Vector3.zero);
            OnEnemyKilled();
        }
    }

    private void UpdateMagnet(ref CharacterStat playerStat)
    {
        m_RadiusPickupXp = playerStat.attrackness;
    }
}
