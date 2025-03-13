using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnerAnimation : MonoBehaviour
{
    public GameObject[] corruptedObject;
    public AnimationCurve sizeOverTime;
    private float speed;
    private float[] tempsEcouleGroth = new float[3];
    [Range(1, 20)]
    private float timeGrothMax = 20;
    private bool[] growing = new bool[3];
    private float[] progression = new float[3];
    private Animator m_animator;

    public AnimationCurve ac_pulseFrequency;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < tempsEcouleGroth.Length; i++)
        {
            tempsEcouleGroth[i] = Random.Range(0, timeGrothMax);
        }
        m_animator = this.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        float delTime = Time.deltaTime;
        for (int i = 0; i < tempsEcouleGroth.Length; i++ )
        {
            if (growing[i])
            {
                tempsEcouleGroth[i] += delTime * speed;
                if (tempsEcouleGroth[i] > timeGrothMax)
                {
                    growing[i] = false;
                }
            }
            else
            {
                tempsEcouleGroth[i] -= delTime * speed;
                if (tempsEcouleGroth[i] <= 0.1f)
                {
                    growing[i] = true;
                }
            }
            progression[i] = tempsEcouleGroth[i] / timeGrothMax;
        }
        


        Growing();
    }

    private void Growing()
    {
        for (int i = 0; i < corruptedObject.Length; i++)
        {
            corruptedObject[i].transform.localScale = new Vector3(0.1f,0.1f,0.1f) + (Vector3.one * 20 * sizeOverTime.Evaluate(progression[i]));
        }
    }

    public void UpdatePulseFrequency(float ratio)
    {
        m_animator.speed = ac_pulseFrequency.Evaluate(ratio);
    }
}
