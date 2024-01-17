using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMouvement : MonoBehaviour
{
    public float speedFlying = 0;
    public float speedMax = 10;
    public float accelerationTimeSpeed = 1;
    public int rangeDetectPlayer;
    public bool changeSpot = false;
    private bool activeAcceleration = false;
    public int currentSpot;

    public Vector3 nextPosition;
    public LayerMask groundLayer;

    private Animator m_animator;

    public int groundAddOffset;
    public float currendGroundOffset;
    public int groundAddOffsetInitial;

    private Vector3 positionToGoSansY = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        m_animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(changeSpot)
        {
            RaycastHit hit;
            int groundHeight = 0;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, groundLayer))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                groundHeight = (int)hit.point.y;
                Debug.Log("Did Hit");
            }
            if (speedFlying < speedMax)
            {
                speedFlying += Time.deltaTime * accelerationTimeSpeed;
                if(currendGroundOffset < groundAddOffset)
                {
                    currendGroundOffset += Time.deltaTime * accelerationTimeSpeed * (groundAddOffset / 10);
                }
                else
                {
                    currendGroundOffset = groundAddOffset;
                }

            }
            else
            {
                speedFlying = speedMax;
                currendGroundOffset = groundAddOffset;
            }
            Vector3 positionSansY = new Vector3(transform.position.x, 0, transform.position.z);
            float remainDistance = Vector3.Distance(positionSansY, positionToGoSansY);
            if (remainDistance > 10)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPosition, speedFlying * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, groundHeight + currendGroundOffset, transform.position.z);
                transform.LookAt(-nextPosition);
            }
            else if(remainDistance < 30 && remainDistance > 3)
            {
                //if(currendGroundOffset > 0)
                //{
                //    currendGroundOffset -= Time.deltaTime * accelerationTimeSpeed;
                //}
                transform.position = Vector3.MoveTowards(transform.position, nextPosition, speedFlying * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, groundHeight + currendGroundOffset, transform.position.z);
                transform.LookAt(-nextPosition);

            }
            else if(remainDistance < 3)
            {
                transform.position = nextPosition;
                changeSpot = false;
                m_animator.SetBool("flying", false);
                currendGroundOffset = 0;
            }

        }
    }

    public void GetNewPositionToGo(Vector3 newPosition)
    {
        nextPosition = Vector3.zero;
        Vector3 newPositionToGo = newPosition + Random.insideUnitSphere * 30;
        RaycastHit hit;
        int tryRandomPosition = 0;
        while(nextPosition == Vector3.zero && tryRandomPosition < 10)
        {
            if (Physics.Raycast(newPositionToGo + new Vector3(0, 50, 0), Vector3.down, out hit, Mathf.Infinity, groundLayer))
            {
                nextPosition = hit.point;

            }
            else
            {
                tryRandomPosition++;
            }
        }
        if(tryRandomPosition >= 10)
        {
            nextPosition = newPosition;
        }

        currendGroundOffset = 0;
        changeSpot = true;
        speedFlying = 0;
        activeAcceleration = true;
        m_animator.SetBool("flying", true);
        float randomAnimationSpeed = Random.Range(0.75f, 1.25f);
        m_animator.speed = randomAnimationSpeed;
        groundAddOffset = (int)Random.Range(groundAddOffsetInitial * 0.75f, groundAddOffsetInitial * 1.25f);
        positionToGoSansY = new Vector3(nextPosition.x, 0, nextPosition.z);
    }
}
