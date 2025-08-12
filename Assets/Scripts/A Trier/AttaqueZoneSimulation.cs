using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttaqueZoneSimulation : MonoBehaviour
{
    public int[] areaSize = new int[3];
    public int[] maxAttaquePerArea = new int[3];
    public int[] idealAttaquePerArea = new int[3];
    public GameObject[] prefabAttaque = new GameObject[3];
    public GameObject holderAttack;

    public bool activeSimulation = false;
    public bool resetAttack = false;
    public enum difficulty
    {
        Minimum,
        Entre_Mini_Mid,
        Mid,
        Entre_Mid_Maxi,
        Maximum
    };

    public difficulty difficultySelected;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(activeSimulation)
        {
            activeSimulation = false;
            SimulateAttaqueInSameTime();
        }
        if(resetAttack)
        {
            resetAttack = false;
            DeleteGeneratadeAttack();
        }

    }

    public void SimulateAttaqueInSameTime()
    {
        if (areaSize.Length > 0)
        {
            for (int i = 0; i < areaSize.Length; i++)
            {
                int nombreAttaque = 0;
                switch (difficultySelected)
                {
                    case difficulty.Minimum:
                        {

                            nombreAttaque = 1;
                            break;
                        }
                    case difficulty.Entre_Mini_Mid:
                        {
                            nombreAttaque = Random.Range(1, idealAttaquePerArea[i]);
                            break;
                        }
                    case difficulty.Mid:
                        {
                            nombreAttaque = idealAttaquePerArea[i];
                            break;
                        }
                    case difficulty.Entre_Mid_Maxi:
                        {
                            nombreAttaque = Random.Range(idealAttaquePerArea[i], maxAttaquePerArea[i]);
                            break;
                        }
                    case difficulty.Maximum:
                        {
                            nombreAttaque = maxAttaquePerArea[i];
                            break;
                        }
                }
                Debug.Log(nombreAttaque);
                for(int j = 0; j < nombreAttaque; j++)
                {
                    int randomAttack = Random.Range(0, 3);
                    Vector3 randomPosition = Vector3.zero;
                    int randomPositifNegatif = Random.Range(0, 4);
                    if (j == 0)
                    {
                        if(randomPositifNegatif == 0)
                        {
                            randomPosition = new Vector3(Random.Range(0, areaSize[0]), 0, Random.Range(0, areaSize[0]));
                        }
                        else if(randomPositifNegatif == 1)
                        {
                            randomPosition = new Vector3(Random.Range(0, -areaSize[0]), 0, Random.Range(0, areaSize[0]));
                        }
                        else if (randomPositifNegatif == 2)
                        {
                            randomPosition = new Vector3(Random.Range(0, areaSize[0]), 0, Random.Range(0, -areaSize[0]));
                        }
                        else if (randomPositifNegatif == 3)
                        {
                            randomPosition = new Vector3(Random.Range(0, -areaSize[0]), 0, Random.Range(0, -areaSize[0]));
                        }

                    }
                    else if (j == 1)
                    {
                        if(randomPositifNegatif == 0)
                        {
                            randomPosition = new Vector3(Random.Range(areaSize[0], areaSize[1]), 0, Random.Range(areaSize[0], areaSize[1]));
                        }
                        else if (randomPositifNegatif == 1)
                        {
                            randomPosition = new Vector3(Random.Range(-areaSize[0], -areaSize[1]), 0, Random.Range(areaSize[0], areaSize[1]));
                        }
                        else if (randomPositifNegatif == 2)
                        {
                            randomPosition = new Vector3(Random.Range(areaSize[0], areaSize[1]), 0, Random.Range(-areaSize[0], -areaSize[1]));
                        }
                        else if (randomPositifNegatif == 3)
                        {
                            randomPosition = new Vector3(Random.Range(-areaSize[0], -areaSize[1]), 0, Random.Range(-areaSize[0], -areaSize[1]));
                        }

                    }
                    else if (j == 2)
                    {
                        if (randomPositifNegatif == 0)
                        {
                            randomPosition = new Vector3(Random.Range(areaSize[1], areaSize[2]), 0, Random.Range(areaSize[1], areaSize[2]));
                        }
                        else if (randomPositifNegatif == 1)
                        {
                            randomPosition = new Vector3(Random.Range(-areaSize[1], -areaSize[2]), 0, Random.Range(areaSize[1], areaSize[2]));
                        }
                        else if (randomPositifNegatif == 2)
                        {
                            randomPosition = new Vector3(Random.Range(areaSize[1], areaSize[2]), 0, Random.Range(-areaSize[1], -areaSize[2]));
                        }
                        else if (randomPositifNegatif == 3)
                        {
                            randomPosition = new Vector3(Random.Range(-areaSize[1], -areaSize[2]), 0, Random.Range(-areaSize[1], -areaSize[2]));
                        }

                    }

                    Instantiate(prefabAttaque[randomAttack], randomPosition, transform.rotation, holderAttack.transform);
                }
            }
        }
        
    }

    public void DeleteGeneratadeAttack()
    {
        if(holderAttack.transform.childCount > 0)
        {
            for(int i = holderAttack.transform.childCount-1; i > 0; i--)
            {
                Destroy(holderAttack.transform.GetChild(i).gameObject);
            }
        }
    }
    private void OnDrawGizmos()
    {
       
    }
}
