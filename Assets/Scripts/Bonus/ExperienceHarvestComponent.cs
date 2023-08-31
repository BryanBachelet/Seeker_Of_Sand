using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceHarvestComponent : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<Experience_System>().ActiveExpHarvest();
            Destroy(this.gameObject);
        }
    }
}
