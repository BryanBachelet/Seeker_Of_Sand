using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Rendering;

public class LightActivation : MonoBehaviour
{
    [SerializeField] private float rangeActivation;
    [SerializeField] LayerMask layerLight;
    [SerializeField] Collider[] colLight;

    [SerializeField] private Color m_lightColor;
    [ColorUsage(true, true)]
    [SerializeField] private Color m_materialLightingColor;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, rangeActivation, layerLight);
        colLight = col;
        if (col.Length > 0)
        {
            ActiveLight(col);
        }
    }

    public void ActiveLight(Collider[] colHit)
    {
        for (int i = 0; i < colHit.Length; i++)
        {
            colHit[i].GetComponent<Animator>().SetBool("Active", true);
            colHit[i].enabled = false;
            //if (vfxLight.isActiveAndEnabled) ;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, rangeActivation);
    }

}
