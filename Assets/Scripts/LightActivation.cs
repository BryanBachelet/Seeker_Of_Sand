using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LightActivation : MonoBehaviour
{
    [SerializeField] private float rangeActivation;
    [SerializeField] LayerMask layerLight;
    [SerializeField] Collider[] colLight;
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
        for(int i = 0; i < colHit.Length; i++)
        {
            VisualEffect vfxLight = colHit[i].GetComponent<VisualEffect>();
            vfxLight.SendEvent("Active");
            colHit[i].enabled = false;
            //if (vfxLight.isActiveAndEnabled) ;
            Debug.Log(vfxLight.isActiveAndEnabled);
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, rangeActivation);
    }

}
