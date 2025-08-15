using GuerhoubaGames.Enemies;
using GuerhoubaGames.GameEnum;
using GuerhoubaGames.Resources;
using GuerhoubaTools;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.Character
{


    public class CharacterArtefact : MonoBehaviour
    {
        public List<ArtefactsInfos> artefactsList;
        [HideInInspector] private Enemies.EnemyManager m_enemyManager;

        public bool activeDebug = false;
        private Character.CharacterShoot m_characterShoot;
        private CharacterProfile m_characterProfil;
        private CharacterDamageComponent m_characterDamageComponent;
        private HealthPlayerComponent m_healthComponent;

        [HideInInspector] private GameResources m_gameResources;
        public GameObject targetObjectAround;
        [HideInInspector] private List<GameObject> artefactAround_List = new List<GameObject>();

        [HideInInspector] private float rangeRandom = 5f;

        private int pullingQuantity = 100;

        public UI_Fragment_Tooltip uiFragmentTooltip;
        private Vector3 positionRandom;

        public delegate void OnContact(Vector3 position, EntitiesTrigger tag, GameObject objectHit, GameElement element);
        public event OnContact OnContactEvent = delegate { };


        public delegate void OnDeath(Vector3 position, EntitiesTrigger tag, GameObject objectHit, float distance);
        public event OnDeath OnDeathEvent = delegate { };




        public void Start()
        {
            m_characterShoot = GetComponent<Character.CharacterShoot>();
            m_characterProfil = GetComponent<CharacterProfile>();
            m_characterDamageComponent = GetComponent<Character.CharacterDamageComponent>();
            m_enemyManager = GameObject.Find("General_Manager").GetComponent<EnemyManager>();
            m_healthComponent = GetComponent<HealthPlayerComponent>();
            positionRandom = Random.insideUnitSphere * rangeRandom;
            m_gameResources = GameResources.instance;
            List<ArtefactsInfos> cloneList = new List<ArtefactsInfos>(artefactsList.ToArray());
            artefactsList.Clear();
            for (int i = 0; i < cloneList.Count; i++)
            {
                AddArtefact(cloneList[i]);
            }

        }

        private void SetupArtefact(ArtefactsInfos artefacts)
        {
            artefacts.characterGo = gameObject;
            if (artefacts.artefactType == ArtefactType.Buff)
            {
                m_characterProfil.AddStat(artefacts.generalStatData.CharacterStat);
                m_characterProfil.UpdateStats();
                return;
            }

            artefacts.isDebugActive = activeDebug;
            switch (artefacts.conditionsTrigger)
            {
                case ConditionsTrigger.OnHit:
                    m_characterShoot.onHit += artefacts.ActiveArtefactOnHit;
                    break;
                case ConditionsTrigger.OnDeath:
                    m_enemyManager.OnDeathEvent += artefacts.ActiveArtefactOnDeath;
                    OnDeathEvent += artefacts.ActiveArtefactOnDeath;
                    break;
                case ConditionsTrigger.Contact:
                    m_healthComponent.OnContactEvent += artefacts.ActiveArtefactOnHit;
                    OnContactEvent += artefacts.ActiveArtefactOnHit;
                    break;
                case ConditionsTrigger.StatGeneral:
                    m_characterDamageComponent.AddDamage(artefacts.damageToApply, artefacts.gameElement, artefacts.damageTypeBonus);
                    break;
                default:
                    break;
            }

            // artefacts.characterGo = gameObject;

        }

        public void ActiveOnContact(Vector3 position, EntitiesTrigger tag, GameObject objectHit, GameElement element)
        {
            OnContactEvent(position, tag, objectHit, element);
        }

        public void ActiveOnDeath(Vector3 position, EntitiesTrigger tag, GameObject objectHit, float distance)
        {
            OnDeathEvent(position, tag, objectHit, distance);
        }

        private void UnSetupArtefact(ArtefactsInfos artefacts)
        {
            artefacts.isDebugActive = activeDebug;
            switch (artefacts.conditionsTrigger)
            {
                case ConditionsTrigger.OnHit:
                    m_characterShoot.onHit -= artefacts.ActiveArtefactOnHit;
                    break;
                case ConditionsTrigger.OnDeath:
                    m_enemyManager.OnDeathEvent -= artefacts.ActiveArtefactOnDeath;
                    OnDeathEvent -= artefacts.ActiveArtefactOnDeath;
                    break;
                case ConditionsTrigger.Contact:
                    m_healthComponent.OnContactEvent -= artefacts.ActiveArtefactOnHit;
                    OnContactEvent -= artefacts.ActiveArtefactOnHit;
                    break;
                case ConditionsTrigger.StatGeneral:
                    m_characterDamageComponent.AddDamage(-artefacts.damageToApply, artefacts.gameElement, artefacts.damageTypeBonus);
                    break;
                default:
                    break;
            }

            artefacts.characterGo = gameObject;
        }


        public ArtefactsInfos[] GetMostDamageArtefactInfo(int count)
        {

            ArtefactsInfos[] artefactsInfosArray;
            if (artefactsList.Count <= count)
            {
                artefactsInfosArray = artefactsList.ToArray();
                return artefactsInfosArray;
            }

            artefactsInfosArray = new ArtefactsInfos[count];
            int[] artefactDamage = new int[count];

            for (int i = 0; i < artefactsList.Count; i++)
            {
                if (!GameStats.instance.IsContains(artefactsList[i].nameArtefact))
                    continue;

                int damageOccur = GameStats.instance.GetDamage(artefactsList[0].nameArtefact);

                for (int j = 0; j < artefactsInfosArray.Length; j++)
                {
                    if (damageOccur > artefactDamage[j])
                    {
                        artefactsInfosArray[j] = artefactsList[i];
                        artefactDamage[j] = damageOccur;
                        break;
                    }
                }
            }

            return artefactsInfosArray;
        }

        public void AddArtefact(ArtefactsInfos artefacts)
        {

            // Verify if the player doesn't already the artefact
            for (int i = 0; i < artefactsList.Count; i++)
            {
                if (artefactsList[i].IsSameFragment(artefacts))
                {
                    UnSetupArtefact(artefactsList[i]);
                    artefactsList[i].AddAdditionalFragment(artefacts);
                    SetupArtefact(artefactsList[i]);
                    uiFragmentTooltip.UpdateFragmentStack(i, artefactsList[i].additionialItemCount + 1);
                    return;
                }
            }



            ArtefactsInfos clone = artefacts.Clone();

            artefactsList.Add(clone);

            CreatePull(clone);

            LogSystem.LogMsg("Artefact add  is " + clone.name);
            GenerateNewArtefactAround(clone);
            SetupArtefact(clone);
            uiFragmentTooltip.AddNewFragment(clone);
        }

        public void CreatePull(ArtefactsInfos instance)
        {
            if (instance.m_artefactToSpawn == null) return;
            PullConstructionData pullConstructionData = new PullConstructionData(instance.m_artefactToSpawn, pullingQuantity);

            if (GamePullingSystem.instance == null) return;
            GamePullingSystem.instance.CreatePull(pullConstructionData);

            if (instance.m_artefactProjectile != null)
            {
                PullConstructionData pullConstructionData1 = new PullConstructionData(instance.m_artefactProjectile, pullingQuantity);
                if (GamePullingSystem.instance == null) return;
                GamePullingSystem.instance.CreatePull(pullConstructionData1);

            }

        }

        public void GenerateNewArtefactAround(ArtefactsInfos artefactInfo)
        {
            //GameObject newArtefactAround = Instantiate(artefactAround_Prefab[GeneralTools.GetElementalArrayIndex( artefacts.gameElement,true)], transform.position, transform.rotation);
            GameObject newArtefactAround = Instantiate(m_gameResources.artefactAround_Prefab[(int)artefactInfo.levelTierFragment], transform.position, transform.rotation);
            fragmentMiniElemental fragMiniElement = newArtefactAround.GetComponent<fragmentMiniElemental>();
            fragMiniElement.m_artefactInfo = artefactInfo;
            fragMiniElement.SelectElement(artefactInfo.gameElement);
            newArtefactAround.GetComponent<Klak.Motion.SmoothFollow>().target = targetObjectAround.transform;
            artefactAround_List.Add(newArtefactAround);
        }

        public void RemoveFragment(int indexFragmentAround)
        {
            GameObject fragmentToRemove = artefactAround_List[indexFragmentAround];
            artefactAround_List.Remove(fragmentToRemove);
            Destroy(fragmentToRemove);
        }
        public void RemoveArtefact(int index)
        {
            artefactsList.RemoveAt(index);
            uiFragmentTooltip.RemoveFragment(index);
        }


        public void RemoveArtefact(ArtefactsInfos artefactsInfos)
        {
            int indexTargetArtefact = artefactsList.IndexOf(artefactsInfos);
            uiFragmentTooltip.RemoveFragment(indexTargetArtefact);
            RemoveFragment(indexTargetArtefact);

            UnSetupArtefact(artefactsInfos);
            artefactsList.Remove(artefactsInfos);
        }

        public void RemoveSpecificFragment(ArtefactsInfos artefactInfo)
        {
            int indexTargetArtefact = artefactsList.IndexOf(artefactInfo);
            RemoveFragment(indexTargetArtefact);
        }



    }
}