using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMouvement : MonoBehaviour {
    public float _speed = 1.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(_speed * Input.GetAxis("Horizontal") * Time.deltaTime, 0F, _speed * Input.GetAxis("Vertical") * Time.deltaTime);
	}
}
