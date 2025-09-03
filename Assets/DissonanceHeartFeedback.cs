using GuerhoubaGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissonanceHeartFeedback : MonoBehaviour
{
    public float m_distanceActivationAnimation = 50;
    public Animator m_animationGlassSphere;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGlassSphere();
    }

    private void UpdateGlassSphere()
    {
        if (Vector3.Distance(transform.position, RunManager.GetPlayerPosition()) < m_distanceActivationAnimation)
        {
            m_animationGlassSphere.SetBool("PlayerNear", true);
        }
        else
        {
            m_animationGlassSphere.SetBool("PlayerNear", false);
        }
    }
}
