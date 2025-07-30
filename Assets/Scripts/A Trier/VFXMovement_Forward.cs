using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXMovement_Forward : MonoBehaviour
{
    public float speedMovement;
    public Vector3 nextPosition;
    public Vector3 startingPos;
    public float range;

    public LayerMask layerMask;

    public Vector3 targetPosition = Vector3.zero;
    public float timeToReach = 1;
    private float distance;
    private float speed;
    // Start is called before the first frame update
    void Start()
    {
        distance = Vector3.Distance(transform.position, targetPosition);
        speed = distance / timeToReach;

    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, nextPosition) < 1)
        {

        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
    }

    public void ChooseNextPosition()
    {
        Vector3 newPosition = targetPosition + Random.insideUnitSphere * range;
        nextPosition = startingPos + new Vector3(newPosition.x, startingPos.y, newPosition.z);

    }
}
