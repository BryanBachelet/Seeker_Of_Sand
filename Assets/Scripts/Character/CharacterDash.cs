using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{
    public class CharacterDash : MonoBehaviour
    {
        [Header("Dash Parameters")]
        [SerializeField] private float m_dashDistance = 5;
        [SerializeField] private float m_dashDuration = .5f;
        [SerializeField] private float m_dashCooldownDuration = 1.0f;
        [SerializeField] private LayerMask m_obstacleLayerMask;
        [SerializeField] private GameObject m_playerMesh;


        private float m_dashTimer = 0.0f;
        private float m_dashCooldownTimer = 0.0f;

        private Vector3 m_startPoint;
        private Vector3 m_endPoint;
        private Character.CharacterMouvement m_characterMouvement;
        private CharacterAim m_characterAim;
        private bool m_isDashValid;
        private bool m_isActiveCooldown;
        public Render.Camera.CameraBehavior m_cameraBehavior;

        // Start is called before the first frame update
        void Start()
        {
            InitComponent();
        }


        private void InitComponent()
        {
            m_characterMouvement = GetComponent<Character.CharacterMouvement>();
            m_characterAim = GetComponent<CharacterAim>();
        }

        // Function that get the dash input
        public void DashInput(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            {
               StartDash();
            }
        }

        private void StartDash() // Function call at the start of the dash
        {
            if (m_characterMouvement.mouvementState == CharacterMouvement.MouvementState.Knockback || m_isActiveCooldown || m_isDashValid) return;

            m_isDashValid = CalculateDashEndPoint();
            if (!m_isDashValid) return;
			GlobalSoundManager.PlayOneShot(28, transform.position);
            m_characterMouvement.ChangeState(CharacterMouvement.MouvementState.Dash);
            m_dashTimer = 0.0f;

        }

        private bool CalculateDashEndPoint() // Function that test where the player should arrive
        {
            Vector3 m_direction = m_characterMouvement.forwardDirection;
            m_startPoint = transform.position;
            RaycastHit hit = new RaycastHit();
            Vector3 frontPoint = transform.position;
            // Check if obstacle in the front of the player 
            if (Physics.Raycast(transform.position, m_direction.normalized, out hit, m_dashDistance, m_obstacleLayerMask))
            {
                frontPoint = hit.point;
                m_endPoint = hit.point + hit.normal * 4.5f; ;
                return true;
            }
            else
            {
                frontPoint += m_direction.normalized * m_dashDistance;
                // Check if a ground exist to dash on it
                if (Physics.Raycast(frontPoint, Vector3.down, out hit, m_dashDistance, m_obstacleLayerMask))
                {
                    m_endPoint = hit.point + hit.normal*4.5f;

                    return true;
                }

            }
            return false;


        }

        private void EndDash() // Function call at the end of dash
        {
            m_characterMouvement.ChangeState(CharacterMouvement.MouvementState.None);
            m_isDashValid = false;
           
            m_isActiveCooldown = true;
            m_dashCooldownTimer = 0.0f;

        }

        private void DashMouvement()
        {
            if (!m_isDashValid) return;

            if (m_dashTimer > m_dashDuration)
            {
               // transform.position = m_endPoint;
                EndDash();
            }
            else
            {
               transform.position = Vector3.Lerp(m_startPoint, m_endPoint, m_dashTimer / m_dashDuration);
                m_dashTimer += Time.deltaTime;
            }
        }


        public void Update()
        {
            DashMouvement();

            if(m_isActiveCooldown)
            {
                if(m_dashCooldownTimer > m_dashCooldownDuration)
                {
                    m_isActiveCooldown = false;
                    
                }
                else
                {
                    m_dashCooldownTimer += Time.deltaTime;
                }
            }
        }


        public void OnDrawGizmos()
        {

            Gizmos.color = Color.red;
            Gizmos.DrawLine(m_startPoint, m_endPoint);
            Gizmos.DrawSphere(m_endPoint, 3);
        }
    }
}
