using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerAnimation : MonoBehaviour
{
    public GameObject[] corruptedObject;
    public AnimationCurve sizeOverTime;
    public float speed;
    public float[] tempsEcouleGroth = new float[3];
    [Range(1,20)]
    public float timeGrothMax;
    public bool[] growing;
    public bool debug = false;
    public float[] progression = new float[3];
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < tempsEcouleGroth.Length; i++)
        {
            tempsEcouleGroth[i] = Random.Range(0, timeGrothMax);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!debug) return;
        for(int i = 0; i < tempsEcouleGroth.Length; i++ )
        {
            if (growing[i])
            {
                tempsEcouleGroth[i] += Time.deltaTime * speed;
                if (tempsEcouleGroth[i] > timeGrothMax)
                {
                    growing[i] = false;
                }
            }
            else
            {
                tempsEcouleGroth[i] -= Time.deltaTime * speed;
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
}
