using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class AttackTrainingArea : MonoBehaviour
{
    static public bool debugCollider;
    public bool activeDebugCollider;
    public Vector3 positionOnDestroy;
    public float rangeHit;
    public Transform playerTarget;
    public health_Player hpPlayer;
    private VisualEffect m_Vfx;
    public float tempsVie;
    // Start is called before the first frame update

    private void OnEnable()
    {

        m_Vfx = GetComponentInChildren<VisualEffect>();
        m_Vfx.SetFloat("TempsRealese", tempsVie);
        m_Vfx.SetFloat("Size", rangeHit * 3.33f);
        DestroyAfterBasic destroyScript = this.gameObject.AddComponent<DestroyAfterBasic>();
        destroyScript.m_DestroyAfterTime = tempsVie;

    }
    void Start()
    {

        debugCollider = activeDebugCollider;

    }

    // Update is called once per frame
    void Update()
    {
        if(playerTarget != null)
        {
            hpPlayer = playerTarget.GetComponent<health_Player>();
            m_Vfx.SendEvent("ActiveArea");
        }
        if(debugCollider)
        {
            positionOnDestroy = transform.position;
        }


    }

    private void OnDestroy()
    {
        positionOnDestroy = transform.position;
        if (Vector3.Distance(positionOnDestroy, playerTarget.position) < rangeHit)
        {
            hpPlayer.GetDamageLeger(5, positionOnDestroy);
            Debug.Log("Hit at [" + Vector3.Distance(positionOnDestroy, playerTarget.position) + "]");
        }
    }

    private void OnDrawGizmos()
    {
        if(positionOnDestroy != Vector3.zero)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(positionOnDestroy, rangeHit);
        }

    }
}
