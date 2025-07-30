using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveForward : MonoBehaviour
{
    public float speedMovement;
    public Vector3 nextPosition;
    public Vector3 startingPos;
    public float range;

    public LayerMask layerMask;

    public Vector3 targetPosition = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.localPosition;
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
            startingPos = new Vector3(startingPos.x, hit.point.y - 15, startingPos.z);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
        ChooseNextPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, nextPosition) < 1)
        {
            ChooseNextPosition();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, nextPosition, speedMovement * Time.deltaTime);
        }


    }

    public void ChooseNextPosition()
    {
        Vector3 newPosition = targetPosition + Random.insideUnitSphere * range;
        nextPosition = startingPos + new Vector3(newPosition.x, startingPos.y, newPosition.z);

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
