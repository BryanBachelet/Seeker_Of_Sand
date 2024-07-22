using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool_DamageMeter : MonoBehaviour
{
    public float dps = 0;
    public float dpsCumule = 0;
    private float lastDpsCompare = 0;

    private static float dpsRecent = 0;

    private float time;
    private float lastTimeCheck;
    // Start is called before the first frame update
    void Start()r
    {
        dps = 0;
        dpsCumule = 0;
        lastDpsCompare = 0;
        dpsRecent = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.time;
        if(lastTimeCheck + 1 < time)
        {
            lastTimeCheck = time;
            dps = dpsRecent / time;
        }
    }

    public static void AddDamage(float damage)
    {
        dpsRecent += damage;
    }
}
