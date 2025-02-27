using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpAndDownMovement : MonoBehaviour
{
    private Vector3 initialPosition;
    public AnimationCurve movementY;
    public AnimationCurve movementX;
    public AnimationCurve movementZ;

    public float tempsLoop;
    private float tempsEcoule;

    [Range(0, 1)]
    public float offSetStart;


    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        offSetStart = Random.Range(0, 0.25f);
        int offSetUpOrDown = Random.Range(0, 100);
        if(offSetUpOrDown < 50)
        {
            tempsEcoule = offSetStart * tempsLoop;
        }
        else
        {
            tempsEcoule = (0.5f + offSetStart) * tempsLoop;
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = initialPosition + UpdatePosition();
    }

    public Vector3 UpdatePosition()
    {
        if (tempsEcoule < tempsLoop)
        {
            tempsEcoule += Time.deltaTime;
        }
        else
        {
            tempsEcoule = 0;
        }
        float progress = tempsEcoule / tempsLoop;
        Vector3 movement = new Vector3(movementX.Evaluate(progress), movementY.Evaluate(progress), movementZ.Evaluate(progress));
        return movement;
    }
}
