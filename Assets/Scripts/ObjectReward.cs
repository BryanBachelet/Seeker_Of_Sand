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
            }
            else if (element == GameElement.AIR)
            {
                charaprofil.stats.baseStat.speed += 5;
            }
            else if (element == GameElement.FIRE)
            {
                charaprofil.stats.baseStat.damage += 1;
            }
            else if (element == GameElement.EARTH)
            {
                charaprofil.stats.baseStat.armor += 1;
            }

        }
        Destroy(this.gameObject);
    }
}
        
