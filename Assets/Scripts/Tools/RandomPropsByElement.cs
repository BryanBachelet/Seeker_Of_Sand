using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPropsByElement : MonoBehaviour
{
    [SerializeField] private GameObject[] rndLargeDecorElement;
    [Range(0,100)]
    public int probability = 25; //Probability

    // Start is called before the first frame update
    void Start()
    {

        Transform[] gameObjectChild = transform.GetComponentsInChildren<Transform>();
        rndLargeDecorElement = new GameObject[gameObjectChild.Length];
        for(int i = 0; i < gameObjectChild.Length; i++)
        {
            rndLargeDecorElement[i] = gameObjectChild[i].gameObject;
        }
        GenerateRandomLargeElement();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateRandomLargeElement()
    {
        if(rndLargeDecorElement.Length > 0)
        {
            for(int i = 0; i < rndLargeDecorElement.Length; i++)
            {
                int rnd = Random.Range(0, 100);
                if (rnd < probability)
                {
                    rndLargeDecorElement[i].SetActive(true);
                }
                else
                {
                    rndLargeDecorElement[i].SetActive(false);
                }
            }

        }
    }

    private void OnEnable()
    {

    }
}
