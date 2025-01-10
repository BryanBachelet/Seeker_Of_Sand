using Character;
using GuerhoubaGames.GameEnum;
using SpellSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GuerhoubaGames.DamageCalculData;
namespace GuerhoubaGames
{

    public interface IDamageReceiver
    {
        public void ReceiveDamage(string nameDamage, DamageStatData damageStat, Vector3 direction, float power, int element, int additionnal);
    }
    public enum DamageType
    {
        PERMANENT = 0,
        TEMPORAIRE = 1,
        BONUS = 2,
    }

    [System.Serializable]
    public class DamageCalculData
    {
       
        public int damage;
        public int tempDamage;
        public int bonusDamage;
        public float modificator;
        public GameElement element;



        public DamageCalculData(GameElement specificElement)
        {
            element = specificElement;
        }

        public static DamageCalculData operator +(DamageCalculData a, DamageCalculData b)
        {

            if (a.element != b.element)
            {
                Debug.LogWarning("Warning : You try to add two stats of damage with a different element");
            }
            a.damage += b.damage;
            a.tempDamage += b.tempDamage;
            a.modificator += b.modificator;
            a.bonusDamage += b.bonusDamage;

            return a;
        }
    }


    [System.Serializable]
    public class DamageStats
    {
        

        [HideInInspector] private DamageCalculData m_airDamage;
        [HideInInspector] private DamageCalculData m_fireDamage;
        [HideInInspector] private DamageCalculData m_waterDamage;
        [HideInInspector] private DamageCalculData m_earthDamage;

        [HideInInspector] public int damageBonusGeneral;
        [HideInInspector] public int damageTemporaireGeneral;
        [HideInInspector] public float generalModificator;

        [HideInInspector] public bool OnDeath;
        [HideInInspector] public bool OnContact;
        [HideInInspector] public bool OnHit;

        private DamageCalculData[] m_damageCalculDatas;

        public DamageStats()
        {
            m_airDamage = new DamageCalculData(GameElement.AIR);
            m_fireDamage = new DamageCalculData(GameElement.FIRE);
            m_waterDamage = new DamageCalculData(GameElement.WATER);
            m_earthDamage = new DamageCalculData(GameElement.EARTH);

            m_damageCalculDatas = new DamageCalculData[] { m_airDamage, m_fireDamage, m_waterDamage, m_earthDamage };
        }

        /// <summary>
        /// Add Damage to the element damage data
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="gameElement"></param>
        /// <param name="isTempDamage"> Will be remove after the next calcul of damage. Generally use by spell</param>
        public void AddDamage(int damage, GameElement gameElement, DamageType damageType)
        {
            for (int i = 0; i < m_damageCalculDatas.Length; i++)
            {
                if (gameElement == m_damageCalculDatas[i].element)
                {
                    switch (damageType)
                    {
                        case DamageType.PERMANENT:
                            m_damageCalculDatas[i].damage += damage;
                            break;

                        case DamageType.TEMPORAIRE:
                            m_damageCalculDatas[i].tempDamage += damage;
                            break;
                        case DamageType.BONUS:
                            m_damageCalculDatas[i].bonusDamage += damage;
                            break;
                        default:

                            break;
                    }
                    
                }
            }
        }

        public static DamageStats operator +(DamageStats a, DamageStats b)
        {
            a.m_fireDamage += b.m_fireDamage;
            a.m_airDamage += b.m_airDamage;
            a.m_earthDamage += b.m_earthDamage;
            a.m_waterDamage += b.m_waterDamage;

            a.generalModificator += b.generalModificator;
            a.damageBonusGeneral += b.damageBonusGeneral;
            a.damageTemporaireGeneral += b.damageTemporaireGeneral;

            a.OnContact = a.OnContact || b.OnContact;
            a.OnDeath = a.OnDeath || b.OnDeath;
            a.OnHit = a.OnHit || b.OnHit;

            return a;
        }

        public DamageCalculData[] CountOfDamage()
        {
            List<DamageCalculData> damageCalc = new List<DamageCalculData>();
            for (int i = 0; i < m_damageCalculDatas.Length; i++)
            {
                if (m_damageCalculDatas[i].damage  > 0 || m_damageCalculDatas[i].tempDamage > 0)
                {
                    damageCalc.Add(m_damageCalculDatas[i]);
                }
            }

            return damageCalc.ToArray();
        }

        public void ResetTempDamage()
        {
            for (int i = 0; i < m_damageCalculDatas.Length; i++)
            {
                m_damageCalculDatas[i].tempDamage = 0;
            }
            damageTemporaireGeneral = 0;
        }
    }

    [System.Serializable]
    public class DamageCalculComponent : MonoBehaviour
    {

        public DamageStats damageStats;

        private Character.CharacterShoot m_characterShoot;
        private CharacterArtefact m_characterArtefacts;
        public void Init(Character.CharacterDamageComponent playerDamageComponent, Character.CharacterShoot characterShoot, SpellSystem.SpellProfil spellProfil)
        {
            damageStats = new DamageStats();

            damageStats.OnContact = spellProfil.OnContact;
            damageStats.OnHit = spellProfil.OnHit;
            damageStats.OnDeath = spellProfil.OnDeath;

            damageStats += playerDamageComponent.m_damageStats;
            m_characterShoot = characterShoot;
            m_characterArtefacts = characterShoot.GetComponent<CharacterArtefact>();
            
        }

        public DamageStatData[] CalculDamage(GameElement element, CharacterObjectType typeObj, GameObject target, SpellSystem.SpellProfil spellProfil)
        {
            DamageCalculData[] damageCalculDataArray = damageStats.CountOfDamage();

            DamageStatData[] damageStatDatas = new DamageStatData[damageCalculDataArray.Length];


            GameElement allElement = GameElement.NONE;

            // Fill Damage Stats Data
            for (int i = 0; i < damageCalculDataArray.Length; i++)
            {
                allElement = allElement | damageCalculDataArray[i].element;
                int totalDamage = damageCalculDataArray[i].damage + damageCalculDataArray[i].tempDamage + damageCalculDataArray[i].bonusDamage + damageStats.damageBonusGeneral + damageStats.damageTemporaireGeneral;
                totalDamage = Mathf.RoundToInt(totalDamage *  (1.0f + damageCalculDataArray[i].modificator + damageStats.generalModificator));
                damageStatDatas[i] = new DamageStatData(totalDamage, typeObj);
                damageStatDatas[i].element = damageCalculDataArray[i].element;
                damageCalculDataArray[i].tempDamage = 0;
            }

            if (damageStats.OnHit) m_characterShoot.ActiveOnHit(target.transform.position, EntitiesTrigger.Enemies, target, allElement);
            if (damageStats.OnContact) m_characterArtefacts.ActiveOnContact(target.transform.position, EntitiesTrigger.Enemies, target, allElement);
            if (damageStats.OnDeath) m_characterArtefacts.ActiveOnDeath(target.transform.position, EntitiesTrigger.Enemies, target, Vector3.Distance(m_characterShoot.transform.position, target.transform.position));
            
            return damageStatDatas;
        }




    }
}
