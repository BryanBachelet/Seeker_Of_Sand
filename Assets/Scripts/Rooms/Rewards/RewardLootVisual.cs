using GuerhoubaGames.GameEnum;
using GuerhoubaTools.Gameplay;
using SeekerOfSand.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace GuerhoubaGames
{

    /// <summary>
    /// This class manage the visual of the RewardLoot gameobject
    /// </summary>
    public class RewardLootVisual : MonoBehaviour
    {

        private RewardInteraction m_rewardInteraction;

        [Header(" Resources Variables")]
        [SerializeField] private Mesh[] m_RewardType = new Mesh[3];
        [SerializeField] private Material[] m_materialRewardType = new Material[5];
        [SerializeField] private Material[] m_materialRewardTypeCristals = new Material[5];
        [SerializeField] private Texture[] m_textureRewardArray;
        [ColorUsage(true, true)] public Color[] colorElemPortal;
        [SerializeField] private Material[] m_materialsRewardChange = new Material[5];

        [Header(" Components Variables")]
        [SerializeField] MeshRenderer m_mainMeshRenderer;
        private MeshFilter m_mainMeshFilter;
        private Material m_mainMaterial;
        [SerializeField] private Material m_materialRewardColor;
        [SerializeField] private Material m_materialRewardChange;


        [Header("Glass Sphere variable")]
        [SerializeField] private GameObject m_glassSphere;
        [SerializeField] private float m_distanceActivationAnimation =50;
        private Animator m_animationGlassSphere;

        #region Unity Functions
        public void Start()
        {
            InitComponent();
            SetupVisualAspect(m_rewardInteraction.rewardType, m_rewardInteraction.rewardElement);
        }

        public void Update()
        {
            UpdateGlassSphere();
        }
        #endregion

        private void InitComponent()
        {
            m_rewardInteraction = GetComponent<RewardInteraction>();
            m_animationGlassSphere = m_glassSphere.GetComponent<Animator>();
            m_mainMeshFilter = m_mainMeshRenderer.GetComponent<MeshFilter>();
        }

        private void SetupVisualAspect(RewardType rewardType , GameElement element)
        {
            int indexRewardType = (int)rewardType;
            int indexElement =  GeneralTools.GetElementalArrayIndex(element);
            
            indexElement = indexElement == -1 ? 3 : indexElement;

            m_mainMaterial = m_mainMeshRenderer .material;
            
            m_mainMaterial.mainTexture = m_textureRewardArray[indexRewardType];
            m_mainMeshFilter.mesh = m_RewardType[indexRewardType];
            m_materialRewardChange = m_materialsRewardChange[indexRewardType];
            m_materialRewardColor.SetColor("_MainColor", colorElemPortal[indexElement]);
            m_materialRewardChange.SetColor("_Color_A", colorElemPortal[indexElement]);

        }

        private void UpdateGlassSphere()
        {
            if (Vector3.Distance(transform.position, RunManager.GetPlayerPosition()) < m_distanceActivationAnimation)
            {
                m_animationGlassSphere.SetBool("PlayerNear", true);
            }
            else
            {
                m_animationGlassSphere.SetBool("PlayerNear", false);
            }
        }


    }

}