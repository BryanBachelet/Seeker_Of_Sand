using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoussoleComponent : MonoBehaviour
{
    public float valueTilling;
    public Vector2 offsetBoussoleTilling;
    public Renderer rendBoussole;

    private Transform m_transformCamera;
    private Vector2 currentTilling;
    // Start is called before the first frame update
    void Start()
    {
        m_transformCamera = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(rendBoussole != null)
        {
            Debug.Log(m_transformCamera.rotation.eulerAngles.y);
            valueTilling = (m_transformCamera.rotation.eulerAngles.y + 180) / 360;
            //valueTilling = valueTilling /2;
            rendBoussole.material.mainTextureOffset = offsetBoussoleTilling + new Vector2(valueTilling, 0.68f);
        }

    }
}
