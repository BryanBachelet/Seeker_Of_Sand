using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFunction : MonoBehaviour
{
    public bool onShadow = false;
    public bool outShadow = false;

    static public bool onShadowStatic = false;
    static public bool outShadowStatic = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnShadow()
    {
        onShadow = true;
        outShadow = false;
        onShadowStatic = onShadow;
        outShadowStatic = outShadow;
        Debug.Log("On Shadow enter !");
    }

    public void OutShadow()
    {
        onShadow = false;
        outShadow = true;
        onShadowStatic = onShadow;
        outShadowStatic = outShadow;
        Debug.Log("On Shadow out !");
    }
}
