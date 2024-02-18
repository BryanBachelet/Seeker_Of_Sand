using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLine : MonoBehaviour
{
    public int numberOfPoint = 1;
    public int distanceBeetwenPoint;
    public int numberOfLigne = 1;
    public GameObject LineHolder;
    public LayerMask groundLayer;

    public GameObject lastLineCreated;
    private LineRenderer m_lastLine;

    public bool m_generateLine = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_generateLine)
        {
            m_generateLine = false;
            GenerateNewPoint();
        }
    }

    public void GenerateNewPoint()
    {

        for(int i = 0; i < numberOfLigne; i++)
        {
            lastLineCreated = Instantiate(LineHolder, transform.position, Quaternion.identity, this.transform);
            m_lastLine = lastLineCreated.GetComponent<LineRenderer>();
            Vector2 rndDirection = Random.insideUnitCircle;
            rndDirection.Normalize();
            Vector3[] position = new Vector3[numberOfPoint];
            for(int j = 0; j < numberOfPoint; j++)
            {
                Vector2 rndVariant = new Vector2(rndDirection.x * 0.85f, rndDirection.y * 0.85f);
                Vector3 nextPosition = this.transform.position + (new Vector3(rndVariant.x, 0, rndVariant.y) * (j * distanceBeetwenPoint)); 
                RaycastHit hit;
                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(nextPosition + new Vector3(0,20,0), Vector3.down, out hit, Mathf.Infinity, groundLayer))
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                    position[j] = hit.point - lastLineCreated.transform.position + new Vector3(0, 3, 0);
                    Debug.Log("Did Hit || " + position[j]);
                }
                else
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                    Debug.Log("Did not Hit");
                }
            }
            m_lastLine.positionCount = position.Length;
            m_lastLine.SetPositions(position);
        }
    }
}
