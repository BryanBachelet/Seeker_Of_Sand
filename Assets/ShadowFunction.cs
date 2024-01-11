using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFunction : MonoBehaviour
{
    public bool onShadow = false;
    public bool outShadow = false;

    static public bool onShadowStatic = false;
    static public bool outShadowStatic = false;

    [SerializeField] private float m_TimeOnShadow = 0;
    [SerializeField] private float m_TimeBeforeDetection = 2;
    static public bool onShadowSpawnStatic = false;
    private bool onShadowSpawn = false;

    [SerializeField] private float m_TimeOutShadow = 0;
    [SerializeField] private float m_TimeBeforeStopDetection = 2;
    static public bool outShadowSpawnStatic = false;
    private bool outShadowSpawn = false;

    public Animator detectionAnimator;
    public UnityEngine.VFX.VisualEffect vfxDetection;
    // Start is called before the first frame update
    void Start()
    {

    }
    //ShadowDetection
    // Update is called once per frame
    void Update()
    {
        if (onShadow)
        {
            if (m_TimeOnShadow < m_TimeBeforeDetection)
            {
                m_TimeOnShadow += Time.deltaTime;
                vfxDetection.SetInt("Rate", (int)(m_TimeOnShadow / m_TimeOnShadow * 5));
            }
            else
            {
                detectionAnimator.SetBool("ShadowDetection", true);
                m_TimeOutShadow = 0;
                onShadowSpawn = true;
                onShadowSpawnStatic = onShadowSpawn;
                outShadowSpawn = false;
                outShadowSpawnStatic = outShadowSpawn;
            }

        }
        else
        {
            if (m_TimeOutShadow < m_TimeBeforeStopDetection)
            {
                vfxDetection.SetInt("Rate", 0);
                m_TimeOutShadow += Time.deltaTime;
            }
            else
            {
                detectionAnimator.SetBool("ShadowDetection", false);
                m_TimeOnShadow = 0;
                outShadowSpawn = true;
                outShadowSpawnStatic = outShadowSpawn;
                onShadowSpawn = false;
                onShadowSpawnStatic = onShadowSpawn;
            }
        }

    }

    public void OnShadow()
    {
        onShadow = true;
        outShadow = false;
        onShadowStatic = onShadow;
        outShadowStatic = outShadow;
        Debug.Log("On Shadow enter !");
    }

    public void OutShadow()
    {
        onShadow = false;
        outShadow = true;
        onShadowStatic = onShadow;
        outShadowStatic = outShadow;
        Debug.Log("On Shadow out !");
    }
}
