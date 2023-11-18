using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleContainer : MonoBehaviour
{
    public int capsuleIndex = 7;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log("Capsule Add");
            other.GetComponent<Character.CharacterShoot>().AddSpell(capsuleIndex);
            Destroy(gameObject);

        }
    }
}
