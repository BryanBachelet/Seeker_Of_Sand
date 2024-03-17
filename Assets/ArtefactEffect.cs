using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtefactEffect : MonoBehaviour
{
    public GameObject rootBone;
    // Start is called before the first frame update

    public void ActiveEffect()
    {
        foreach (Rigidbody item in rootBone.transform.GetComponentsInChildren<Rigidbody>())
        {
            item.useGravity = true;
        }
    }
}
