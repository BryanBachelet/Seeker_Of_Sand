using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;

public class RewardTypologie : MonoBehaviour
{
    public RewardType rewardType;
    private Chosereward choseReward;
    public RewardDistribution rewardDistribution;
    public Texture[] text_Reward;
    public Material mat;
    public MeshRenderer meshDisplayReward;
    public HealthReward healthReward;
    public Animator rewardAnimator;
    public SkinnedMeshRenderer m_skinMeshRender;
    [SerializeField] private GameObject rootBoneHolder;
    [SerializeField] private MeshRenderer meshToChangeMaterial;
    [SerializeField] private MeshFilter meshToChange;
    [SerializeField] private Mesh[] m_RewardType = new Mesh[5];
    [SerializeField] private Material[] m_materialRewardType = new Material[5];
    [SerializeField] private Material[] m_materialRewardTypeCristals = new Material[5];
    private ExperienceMouvement[] m_bones = new ExperienceMouvement[100];
    private ExperienceMouvement xpMovement;

    public GameObject[] goDestroy;
    public MeshRenderer vfxMesh;
    private Material vfxReward;

    public Material materialRewardChange;
    public Material[] materialsRewardChange = new Material[5];
    // Start is called before the first frame update

    public void Update()
    {
        if(TerrainGenerator.staticRoomManager.isRoomHasBeenValidate)
        {
            vfxMesh.material = materialRewardChange;
        }
    }
    private void Start()
    {
        xpMovement = this.GetComponent<ExperienceMouvement>();
        vfxReward = vfxMesh.material;
        mat = meshDisplayReward.material;
        meshToChangeMaterial = meshToChange.GetComponent<MeshRenderer>();
        //m_skinMeshRender = rewardAnimator.GetComponentInChildren<SkinnedMeshRenderer>();
        switch (rewardType)
        {
            case RewardType.UPGRADE:
                mat.mainTexture = text_Reward[0];
                meshToChange.mesh = m_RewardType[0];
                meshToChangeMaterial.material = m_materialRewardType[0];
                m_skinMeshRender.material = m_materialRewardTypeCristals[0];
                materialRewardChange = materialsRewardChange[0];
                break;
            case RewardType.SPELL:
                mat.mainTexture = text_Reward[1];
                meshToChange.mesh = m_RewardType[1];
                meshToChangeMaterial.material = m_materialRewardType[1];
                m_skinMeshRender.material = m_materialRewardTypeCristals[1];
                materialRewardChange = materialsRewardChange[1];
                break;
            case RewardType.ARTEFACT:
                if (choseReward == null)
                    mat.mainTexture = text_Reward[2];
                    meshToChange.mesh = m_RewardType[2];
                    meshToChangeMaterial.material = m_materialRewardType[2];
                m_skinMeshRender.material = m_materialRewardTypeCristals[2];
                materialRewardChange = materialsRewardChange[2];
                break;
            case RewardType.HEAL:
                mat.mainTexture = text_Reward[3];
                meshToChange.mesh = m_RewardType[3];
                meshToChangeMaterial.material = m_materialRewardType[3];
                m_skinMeshRender.material = m_materialRewardTypeCristals[3];
                materialRewardChange = materialsRewardChange[3];
                break;
            case RewardType.MERCHANT:
                mat.mainTexture = text_Reward[4];
                meshToChange.mesh = m_RewardType[4];
                meshToChangeMaterial.material = m_materialRewardType[4];
                m_skinMeshRender.material = m_materialRewardTypeCristals[4];
                materialRewardChange = materialsRewardChange[4];
                break;
            default:
                break;
        }
        for(int i = 0; i < rootBoneHolder.transform.childCount; i++)
        {
            m_bones[i] = rootBoneHolder.transform.GetChild(i).GetComponent<ExperienceMouvement>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && TerrainGenerator.staticRoomManager.isRoomHasBeenValidate)
        {
            rewardDistribution.RewardValidate();

            switch (rewardType)
            {
                case RewardType.UPGRADE:
                    other.GetComponent<Character.CharacterUpgrade>().GiveUpgradePoint(3);
                    other.GetComponent<Character.CharacterUpgrade>().ShowUpgradeWindow();
                    break;
                case RewardType.SPELL:
                    other.GetComponent<Character.CharacterUpgrade>().ShowSpellChoiceInteface();
                    break;
                case RewardType.ARTEFACT:
                    if (choseReward == null)
                        choseReward = FindAnyObjectByType<Chosereward>();
                    choseReward.GiveArtefact();
                    break;
                case RewardType.HEAL:
                   if(healthReward == HealthReward.QUARTER)
                        other.GetComponent<HealthPlayerComponent>().RestoreHealQuarter();
                   else
                        other.GetComponent<HealthPlayerComponent>().RestoreFullLife();
                    break;
                default:
                    break;
            }
            DestroyAssociateObject();

        }
    }

    public void ActivationDistribution()
    {
        rewardAnimator.enabled = false;
        for (int i = 0; i < m_bones.Length; i++)
        {
            m_bones[i].ActiveExperienceParticule(xpMovement.m_playerPosition);
        }
    }

    private void DestroyAssociateObject()
    {
        for(int i = 0; i < goDestroy.Length; i++)
        {
            Destroy(goDestroy[i]);
        }
        Destroy(this.gameObject);
    }
}
