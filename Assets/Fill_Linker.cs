using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Fill_Linker : MonoBehaviour
{
    private Image fillObject;
    private Material fillMat;
    // Start is called before the first frame update
    void Start()
    {
        fillObject = this.GetComponent<Image>();
        fillMat = fillObject.material;
    }

    // Update is called once per frame
    void Update()
    {
        fillObject.material.SetFloat("_Fill", fillObject.fillAmount);
    }
}
