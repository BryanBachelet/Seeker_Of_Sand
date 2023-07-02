using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car
{
    public float speed;
    public Vector3 position;
    public Vector3 limitPosition;
    public Vector3 direction;
    public Vector3 angle;
    public Vector3 upAxis;
};



public class Locomotive : MonoBehaviour
{ 
    private enum Axis
    {
        X= 0,
        Z=2,
    };


    public float distanceBetweenLocomotive = .5f;
    public float halflengthOfLocomotive = 2.0f;
    public float speed = 10.0f;
    public float angularSpeed = 10.0f;
    public int maxCar = 10;
    
    public Transform[] objs = new Transform[0];

    private Transform[] m_obj;
    private Car[] m_cars;
    private float m_distanceLimit;
    private int m_count;

    private RaycastHit hit = new RaycastHit();
    public LayerMask rayGround;

    private Vector3 m_destination;
    private Vector3 m_direction;
    private bool m_isMooving;
    
    private void Start()
    {
        m_obj = new Transform[maxCar];
        m_count = 0;
        for (int i = 0; i < objs.Length && i<maxCar; i++)
        {
            m_obj[i] = objs[i];
            m_count++;
        }
        m_cars = new Car[maxCar];
        m_cars[0] = new Car();
        m_cars[0].position = transform.position;
        m_cars[0].direction = Vector3.right;
        m_cars[0].speed = speed;
        for (int i = 0; i < m_count; i++)
        {
            m_cars[i + 1] = new Car();
            m_cars[i + 1].position = m_obj[i].position;
        }
        m_distanceLimit = (halflengthOfLocomotive + distanceBetweenLocomotive);
        AddDestination( new Vector3(10, 0, 10));
    }

    private void Update()
    {
        MoveLocomotive();
    }

    bool IsMoving() { return m_isMooving; }

    void AddWagonAtEnd(Transform objAdd)
    {
        m_obj[m_count] = objAdd;
        m_cars[m_count + 1] = new Car();
        m_cars[m_count + 1].position = objAdd.position;
        m_count++;
    }

    void AddWagon(Transform objAdd, int objIndex)
    {
      
        m_count++;
        for (int i = m_count; i > objIndex; i--)
        {
            m_obj[i] = m_obj[i-1];
        }
        for (int i = m_count + 1; i > objIndex + 1; i--)
        {
            m_cars[i] = m_cars[i-1];
        }

        m_obj[objIndex] = objAdd;
        m_cars[objIndex + 1] = new Car();
        m_cars[objIndex + 1].position = objAdd.position;
    }

    void RemoveWagon(int objIndex)
    {
        m_obj[objIndex] = null;
        m_cars[objIndex + 1] = null;

        for (int i = objIndex; i < m_count; i++)
        {
            m_obj[i] = m_obj[i + 1];
        }
        for (int i = objIndex+1; i < m_count+1 ; i++)
        {
            m_cars[i] = m_cars[i + 1];
        }
        m_count--;
    }
    void MoveLocomotive()
    {
        m_isMooving = false;
        Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);
        if (Vector3.Distance(m_destination, pos) < 0.5f) return;

        m_direction = m_destination - pos;
        m_isMooving = true;
        m_direction.y = 0;
        m_direction = m_direction.normalized;
        float angle = Vector3.SignedAngle(transform.right, m_direction,Vector3.up);
        angle = Mathf.Clamp(angle, -angularSpeed, angularSpeed);

        float baseSpeed = speed * Time.deltaTime;
        //Pre-Calcul value
        float timeReverser = (1.0f / Time.deltaTime);

        // Calculate Velocity direction
        m_cars[0].angle.y += angle *Time.deltaTime ;
        m_cars[0].upAxis = Vector3.Lerp(transform.up, CalculateTrainSlopeQuat(m_cars[0], transform), 0.20f);
        m_cars[0].direction = Quaternion.Euler(transform.eulerAngles.x, m_cars[0].angle.y, transform.eulerAngles.z) * Vector3.right;

        for (int i = 1; i < m_count + 1; i++)
        {
            m_cars[i].direction = CalculateVector(m_cars[i - 1].position, -m_cars[i].position).normalized;
            m_cars[i].angle.y = Vector3.SignedAngle(Vector3.right, m_cars[i].direction.normalized, Vector3.up);
            m_cars[i].upAxis = Vector3.Lerp(m_obj[i - 1].up, CalculateTrainSlopeQuat(m_cars[i], m_obj[i - 1]), 0.20f);
            m_cars[i].speed = speed;
        }

        // Move locomotive car 
        m_cars[0].position += m_cars[0].direction.normalized * baseSpeed;
        m_cars[0].limitPosition = CalculateVector(m_cars[0].position, -m_cars[1].direction * m_distanceLimit);

        // Update transform when speed has been check
        transform.position = m_cars[0].position;
        transform.up = m_cars[0].upAxis;
        transform.rotation *= Quaternion.Euler(m_cars[0].angle);

        for (int i = 0; i < m_count; i++)
        {
            Car item = m_cars[i + 1];
            // Check is speed is to high
            Vector3 nextPosition1 = CalculateVector(item.position, item.direction * baseSpeed);
            Vector3 nextPosition2 = CalculateVector(m_cars[i].limitPosition, -item.position);
            float distance1 = Vector3.Distance(nextPosition1, item.position);
            float distance2 = nextPosition2.magnitude;
            if (distance1 > distance2)
            {
                item.speed = (distance2 * timeReverser);
            }


            // Actualize item when speed has been check
            item.position += item.direction.normalized * item.speed * Time.deltaTime;
            if (i != m_count - 1) item.limitPosition = CalculateVector(item.position, -m_cars[i + 2].direction * m_distanceLimit);

            // Update transform when speed has been check
            m_obj[i].position = item.position;
            m_obj[i].up = item.upAxis;
            m_obj[i].rotation *= Quaternion.Euler(item.angle);
        }
    }

    void AddDestination(Vector3 destination)
    {

        m_destination = destination;
    }

    Vector3 CalculateVector(Vector3 one, Vector3 two)
    {
        one.x = one.x + two.x;
        one.y = one.y + two.y;
        one.z = one.z + two.z;
        return one;
    }

    Vector3 CalculateTrainSlopeQuat(Car item, Transform obj)
    {
        Vector3 normal = Vector3.zero;

        Vector3 origin = CalculateVector(CalculateVector(item.position, item.direction.normalized * halflengthOfLocomotive), Vector3.up * 2.0f);
       
        if (!Physics.Raycast(origin, Vector3.down, out hit, 20, rayGround)) return Vector3.up;
        normal = hit.normal;
        origin = CalculateVector(origin, -item.direction.normalized * halflengthOfLocomotive * 2);

        if (!Physics.Raycast(origin, Vector3.down, out hit, 20, rayGround)) return Vector3.up;
        normal = CalculateVector(hit.normal, normal.normalized).normalized;

        Quaternion test = Quaternion.FromToRotation(Vector3.up, normal.normalized);
        return normal;

    }
}


