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
    [SerializeField] public float lifeTimeVFX;
    private DestroyAfterBasic destroyScript;
    // Start is called before the first frame update

    private void OnEnable()
    {

        m_Vfx = GetComponentInChildren<VisualEffect>();
        destroyScript = this.gameObject.AddComponent<DestroyAfterBasic>();

    }
    void Start()
    {

        debugCollider = activeDebugCollider;
        if (playerTarget != null)
        {
            m_Vfx.SetFloat("Size", rangeHit * 3.33f);
            hpPlayer = playerTarget.GetComponent<health_Player>();
            destroyScript.m_DestroyAfterTime = lifeTimeVFX;
            m_Vfx.SetFloat("TempsRealese", lifeTimeVFX);
            m_Vfx.SendEvent("ActiveArea");
        }
    }

    // Update is called once per frame
    void Update()
    {

        if(debugCollider)
        {
            positionOnDestroy = transform.position;
        }


    }

    private void OnDestroy()
    {
        if (playerTarget == null) return;

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
