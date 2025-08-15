using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.Resources;
using UnityEngine.VFX;

namespace GuerhoubaGames.Enemies
{

    public class NPCAttackProjectile : MonoBehaviour
    {
        public float maxSlopeAngle = 60;
        public int damage = 0;
        public float range = 0;
        public float duration = 0;
        public string attackName;
        public bool isHeavy;
        [HideInInspector] public Vector3 direction;

        private float m_speedProjectile;
        private float m_distanceMoved;
        private bool m_hasDamagePlayer;
        private Vector3 m_normalHit;
        private Vector3 m_hitPoint;
        private float timeInstantiate;
        [Range(0.1f, 3)]
        [SerializeField] private float timeMaxSpeed = 1.5f;
        [SerializeField] private AnimationCurve speedEvolution;

        private PullingMetaData m_pullingMetaData;
        // Start is called before the first frame update
        void Awake()
        {
            m_pullingMetaData = GetComponent<PullingMetaData>();
          

            
        }

        public void Start()
        {
            if (m_pullingMetaData == null) InitComponent(); 
        }



        // Update is called once per frame
        void Update()
        {
            Move();
            CountRangeProjectile();
        }

        public void InitComponent()
        {
            
            m_speedProjectile = range / duration;
            timeInstantiate = Time.time;
        }

        public void CountRangeProjectile()
        {
            if (m_distanceMoved >= range)
            {
                DestroyProjectile();
            }
        }

        public void Move()
        {
            float speed = speedEvolution.Evaluate(Time.time / (timeInstantiate + timeMaxSpeed));
            float speedProjecitle = m_speedProjectile * Time.deltaTime * speed;
            if (Physics.Raycast(transform.position, direction.normalized, speedProjecitle, GameLayer.instance.propsGroundLayerMask))
            {
                DestroyProjectile();
            }
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, GameLayer.instance.groundLayerMask))
            {
                m_normalHit = hit.normal;
                m_hitPoint = hit.point;

                SetSlopeRotation(hit.normal);


            }
            transform.position += transform.forward * speedProjecitle;
            m_distanceMoved += speedProjecitle;
        }

        #region Trigger Function
        public void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                if (!m_hasDamagePlayer)
                {
                    m_hasDamagePlayer = true;
                    HealthPlayerComponent healthPlayer = other.GetComponent<HealthPlayerComponent>();

                    AttackDamageInfo attackDamageInfo = new AttackDamageInfo();
                    attackDamageInfo.attackName = "attackName";
                    attackDamageInfo.position = transform.position;
                    attackDamageInfo.damage = damage;
                    healthPlayer.ApplyDamage(attackDamageInfo);
                }
            }
        }
        public void OnTriggerStay(Collider other)
        {
            if (other.tag == "Player")
            {
                if (!m_hasDamagePlayer)
                {
                    m_hasDamagePlayer = true;
                    HealthPlayerComponent healthPlayer = other.GetComponent<HealthPlayerComponent>();
                    AttackDamageInfo attackDamageInfo = new AttackDamageInfo();
                    attackDamageInfo.attackName = "attackName";
                    attackDamageInfo.position = transform.position;
                    attackDamageInfo.damage = damage;
                    healthPlayer.ApplyDamage(attackDamageInfo);
                }
            }
        }
        #endregion

        public void SetSlopeRotation(Vector3 hitNormal)
        {
            Vector3 axis = Vector3.Cross(transform.right, hitNormal);
            Quaternion rotTest = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            float angle = Vector3.SignedAngle(rotTest * Vector3.forward, axis, transform.right);
            if (Mathf.Abs(angle) > maxSlopeAngle)
                return;
            transform.rotation = Quaternion.Euler(angle, transform.rotation.eulerAngles.y, 0);
        }


        public void DestroyProjectile()
        {
            if (m_pullingMetaData == null)
            {
                Destroy(this.gameObject);
            }
            else if(m_pullingMetaData.isActive)
            {
                int idData = GetComponent<PullingMetaData>().id;
                GamePullingSystem.instance.ResetObject(this.gameObject, idData);
                ResetProjectile();
            }
        }


        public void ResetProjectile()
        {
            VisualEffect vfx = GetComponent<VisualEffect>();
            if (vfx != null)
            {
                vfx.Reinit();
            }

            VisualEffect[] vfxArray = GetComponentsInChildren<VisualEffect>();
            for (int i = 0; i < vfxArray.Length; i++)
            {
                vfxArray[i].Reinit();
            }

            m_distanceMoved = 0;
            m_hasDamagePlayer = false;
        }
    }
}

