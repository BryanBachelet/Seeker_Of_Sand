using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class agaveControl : MonoBehaviour
{
    static DayCyclecontroller dayController;
    public DayCyclecontroller dayControllerAttribution;
    private bool activeNight = false;

    private Animator m_animator;
    // Start is called before the first frame update
    void Start()
    {
        if (dayController == null)
        {
            dayController = dayControllerAttribution;
        }
        m_animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dayController.m_timeOfDay < 5.12f || dayController.m_timeOfDay > 18.5f)
        {
            if(!activeNight)
            {
                activeNight = true;
                m_animator.SetBool("Night", true);
            }
        }
        else
        {
            if(activeNight)
            {
                activeNight = false;
                m_animator.SetBool("Night", false);
            }
        }
    }
}
