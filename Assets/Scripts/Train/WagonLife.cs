using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WagonLife : MonoBehaviour
{

    public float maxLife =10;
    public float currentLife = 10;
    bool isAlive;

    public void Start()
    {
        currentLife = maxLife;
    }

    public void AddLife(float lifeAdd )
    {
        if (lifeAdd + currentLife >= maxLife) currentLife = maxLife;
        else currentLife += lifeAdd;
    }

    public void RemoveLife(float lifeRemove)
    {
        if (lifeRemove - currentLife <= 0)
        {
            isAlive = false;
            currentLife = 0.0f;
        }
        else
        {
            currentLife -= lifeRemove;
        }
        }
    }
