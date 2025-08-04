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
    public Transform playerTransform;
    public HealthPlayerComponent hpPlayer;
    private VisualEffect m_Vfx;
    [SerializeField] public float lifeTimeVFX;
    [SerializeField] public LayerMask m_groundLayerMask;
    private DestroyAfterBasic destroyScript;

    public GameObject vfxExplosion;

    public float tempsAvantExplosion = 1;

    public float delayTimeStart = 0;
    private float tempsEcoleDelay;
    public int damage = 5;

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
        if (playerTransform != null)
        {
            hpPlayer = playerTransform.GetComponent<HealthPlayerComponent>();
            destroyScript.m_DestroyAfterTime = lifeTimeVFX;
            m_Vfx.SetVector3("Size", new Vector3(1,1,1) * rangeHit * 3f);
            m_Vfx.SetFloat("TempsRealese", lifeTimeVFX);
            m_Vfx.SendEvent("ActiveArea");

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
       // vfxExplosionObject.transform.localScale = scaleAttack * rangeHit;
        if (playerTransform == null) return;

        positionOnDestroy = transform.position;
        if (Vector3.Distance(positionOnDestroy, playerTransform.position) < rangeHit)
        {
            AttackDamageInfo attackDamageInfo = new AttackDamageInfo();
            attackDamageInfo.attackName = "Mortar";
            attackDamageInfo.position = positionOnDestroy;
            attackDamageInfo.damage = damage;
            hpPlayer.ApplyDamage(attackDamageInfo);
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
