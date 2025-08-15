using GuerhoubaGames.Character;
using GuerhoubaGames.GameEnum;
using GuerhoubaGames.Resources;
using UnityEngine;

public class ObjectReward : MonoBehaviour
{
    public GameElement element;
    static GameResources ressources;
    // Start is called before the first frame update
    void Start()
    {
        if(ressources == null) { ressources = GameResources.instance; }
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
                charaprofil.stats.baseDamage.statsValue += 15;
                charaprofil.gameObject.GetComponent<HealthPlayerComponent>().AugmenteMaxHealth(15);
                TerrainGenerator.s_currentRoomManager.m_enemyManager.m_mainInformationDisplay.DisplayMessage("Water bonus acquiered", ressources.textureGradient_Ornement[0]);
            }
            else if (element == GameElement.AIR)
            {
                charaprofil.stats.runSpeed.statsValue += 5;
                TerrainGenerator.s_currentRoomManager.m_enemyManager.m_mainInformationDisplay.DisplayMessage("Aerial bonus acquiered", ressources.textureGradient_Ornement[1]);
            }
            else if (element == GameElement.FIRE)
            {
                other.gameObject.GetComponent<CharacterDamageComponent>().m_damageStats.damageBonusGeneral += 1;
                TerrainGenerator.s_currentRoomManager.m_enemyManager.m_mainInformationDisplay.DisplayMessage("Fire bonus acquiered", ressources.textureGradient_Ornement[2]);
            }
            else if (element == GameElement.EARTH)
            {
                charaprofil.stats.armor.statsValue += 1;
                TerrainGenerator.s_currentRoomManager.m_enemyManager.m_mainInformationDisplay.DisplayMessage("Earth bonus acquiered", ressources.textureGradient_Ornement[3]);
            }

        }
        Destroy(this.gameObject);
    }
}
        
