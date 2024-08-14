using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintStartActivation : MonoBehaviour
{
    [SerializeField] private Animator m_animator;
    // Start is called before the first frame update
    void Start()
    {
        if(m_animator != null)
        {
            m_animator.SetBool("ActiveHint", true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
