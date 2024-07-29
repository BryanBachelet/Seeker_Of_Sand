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

    public Animator rewardAnimator;
    [SerializeField] private GameObject rootBoneHolder;
    private ExperienceMouvement[] m_bones = new ExperienceMouvement[100];
    private ExperienceMouvement xpMovement;

    public GameObject[] goDestroy;
    public MeshRenderer vfxMesh;
    private Material vfxReward;

    public Material materialRewardChange;
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
        switch (rewardType)
        {
            case RewardType.UPGRADE:
                mat.mainTexture = text_Reward[0];
                break;
            case RewardType.SPELL:
                mat.mainTexture = text_Reward[1];
                break;
            case RewardType.ARTEFACT:
                if (choseReward == null)
                    mat.mainTexture = text_Reward[2];
                break;
            case RewardType.HEAL:
                mat.mainTexture = text_Reward[3];
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
                    other.GetComponent<HealthPlayerComponent>().RestoreHealQuarter(1);
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
