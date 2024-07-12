using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemies
{

    public class NPCAttackProjectile : MonoBehaviour
    {
        public float maxSlopeAngle = 60;
        public int damage = 0;
        public float range = 0;
        public float duration = 0;
        public string attackName;
        [HideInInspector] public Vector3 direction;

        private float m_speedProjectile;
        private float m_distanceMoved;
        private bool m_hasDamagePlayer;
        private Vector3 m_normalHit;
        private Vector3 m_hitPoint;
        // Start is called before the first frame update
        void Start()
        {
            InitComponent();
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
        }

        public void CountRangeProjectile()
        {
            if (m_distanceMoved >= range)
            {
                Destroy(this.gameObject);
            }
        }

        public void Move()
        {

            if (Physics.Raycast(transform.position, direction.normalized, m_speedProjectile * Time.deltaTime, GameLayer.instance.propsGroundLayerMask))
            {
                Destroy(this.gameObject);
            }
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, GameLayer.instance.groundLayerMask))
            {
                m_normalHit = hit.normal;
                m_hitPoint = hit.point;

                SetSlopeRotation(hit.normal);


            }
            transform.position += transform.forward * m_speedProjectile * Time.deltaTime;
            m_distanceMoved += m_speedProjectile * Time.deltaTime;
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
                    healthPlayer.GetLightDamage(attackDamageInfo);
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
                    healthPlayer.GetLightDamage(attackDamageInfo);
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

    }
}

