using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardTypologie : MonoBehaviour
{
    public RewardType rewardType;
    public GameObject playerRef;
    private Chosereward choseReward;
    // Start is called before the first frame update

    private void Awake()
    {
        playerRef = this.GetComponent<ExperienceMouvement>().m_playerPosition.gameObject;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
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

            Destroy(this.gameObject);
        }
    }
}
