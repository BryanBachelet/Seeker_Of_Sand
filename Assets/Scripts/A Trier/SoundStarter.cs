using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundStarter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GlobalSoundManager.PlayOneShot(0, Vector3.zero);
    }
}
