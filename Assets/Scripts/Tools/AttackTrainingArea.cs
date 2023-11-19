using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class AttackTrainingArea : MonoBehaviour
{
    private ObjectState state;
    static public bool debugCollider;
    public bool activeDebugCollider;
    public Vector3 positionOnDestroy;
    public float rangeHit;
    public Vector3 scaleAttack;
    public Transform playerTarget;
    public health_Player hpPlayer;
    private VisualEffect m_Vfx;
    [SerializeField] public float lifeTimeVFX;
    [SerializeField] public LayerMask m_groundLayerMask;
    private DestroyAfterBasic destroyScript;

    public GameObject vfxExplosion;

    public float tempsAvantExplosion = 1;

    public float delayTimeStart = 0;
    private float tempsEcoleDelay;
    // Start is called before the first frame update

    private void OnEnable()
    {
        state = new ObjectState();
        GameState.AddObject(state);
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
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(transform.position + Vector3.up * 3, -Vector3.up, out hit, 10, m_groundLayerMask)) ;

        {
            float angle = Vector3.SignedAngle(Vector3.up, hit.normal, Vector3.forward);

            transform.rotation = Quaternion.Euler(angle, transform.eulerAngles.y, transform.eulerAngles.z);
            transform.position = hit.point;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!state.isPlaying)
        {
            m_Vfx.pause = true;
            return;
        }
        else if(m_Vfx.pause) m_Vfx.pause = false;


        if (debugCollider)
        {
            positionOnDestroy = transform.position;
        }

    }

    private void OnDestroy()
    {
        GameObject vfxExplosionObject = Instantiate(vfxExplosion, transform.position, transform.rotation);
        vfxExplosionObject.transform.localScale = scaleAttack * rangeHit;
        if (playerTarget == null) return;

        positionOnDestroy = transform.position;
        if (Vector3.Distance(positionOnDestroy, playerTarget.position) < rangeHit)
        {
            hpPlayer.GetDamageLeger(5, positionOnDestroy);
            //vfxExplosionObject.GetComponent<VisualEffect>().Play();
            //Debug.Log("Hit at [" + Vector3.Distance(positionOnDestroy, playerTarget.position) + "]");
        }
    }

    private void OnDrawGizmos()
    {
        if (positionOnDestroy != Vector3.zero)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(positionOnDestroy, rangeHit);
        }

    }
}
