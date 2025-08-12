using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MapGeneration_Assignation : MonoBehaviour
{
    public int[] probability = new int[4];
    public Color[] elementPerMap = new Color[4];

    public int elementAssigne = -1;
    public float radius;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Image>().color = AssignationElement(transform.GetChild(i).GetComponent<Image>().color);
            Collider[] nearObject = Physics.OverlapSphere(transform.GetChild(i).transform.position, radius);
            for(int j = 0; j < nearObject.Length; j++)
            {
                if(j > 0 && nearObject[j].GetComponent<Image>().color == Color.white)
                {
                    int random = Random.Range(0, 100);
                    if(random <10)
                    {
                        nearObject[j].GetComponent<Image>().enabled = false;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Color AssignationElement(Color currentColor)
    {
        if (currentColor == Color.black) return Color.black;
        int random = Random.Range(0, 101);
        int element = -1;
        if(random <= probability[0])
        {
            element = 0;
        }
        else if(random >= probability[0] && random < probability[1])
        {
            element = 1;
        }
        else if (random >= probability[1] && random < probability[2])
        {
            element = 2;
        }
        else if(random >= probability[2])
        {
            element = 3;
        }
        else
        {
            element = 0;
        }
        elementAssigne = element;
        return elementPerMap[elementAssigne];
    }

    
}
