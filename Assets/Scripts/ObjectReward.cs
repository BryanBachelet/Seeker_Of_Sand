using Character;
using GuerhoubaGames.GameEnum;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectReward : MonoBehaviour
{
    public GameElement element;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            CharacterProfile charaprofil = other.gameObject.GetComponent<CharacterProfile>();
            if (element == GameElement.WATER)
            {
                charaprofil.stats.baseStat.healthMax += 15;
                charaprofil.gameObject.GetComponent<HealthPlayerComponent>().AugmenteMaxHealth(15);
                TerrainGenerator.staticRoomManager.m_enemyManager.m_mainInformationDisplay.DisplayMessage("Water bonus acquiered");
            }
            else if (element == GameElement.AIR)
            {
                charaprofil.stats.baseStat.speed += 5;
                TerrainGenerator.staticRoomManager.m_enemyManager.m_mainInformationDisplay.DisplayMessage("Aerial bonus acquiered");
            }
            else if (element == GameElement.FIRE)
            {
                other.gameObject.GetComponent<CharacterDamageComponent>().m_damageStats.damageBonusGeneral += 1;
                TerrainGenerator.staticRoomManager.m_enemyManager.m_mainInformationDisplay.DisplayMessage("Fire bonus acquiered");
            }
            else if (element == GameElement.EARTH)
            {
                charaprofil.stats.baseStat.armor += 1;
                TerrainGenerator.staticRoomManager.m_enemyManager.m_mainInformationDisplay.DisplayMessage("Earth bonus acquiered");
            }

        }
        Destroy(this.gameObject);
    }
}
        
