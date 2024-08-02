using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destructible : MonoBehaviour
{

    public GameObject destroyedversion;
    public bool test=false;
    public void Update()
    {
        if(test==true)
        {
            test=false;
            Activedestruction();
        }
    }


    void Activedestruction ()
     {

        Instantiate(destroyedversion, transform.position, transform.rotation);
        Destroy(gameObject); 
     }   
    
}
