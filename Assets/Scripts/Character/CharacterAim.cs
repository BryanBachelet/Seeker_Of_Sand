using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{

    public class CharacterAim : MonoBehaviour
    {

        [SerializeField] [Range(0.0f, 1.0f)] private float inputAim = 0.6f;
        private PlayerInput m_playerInput;
        private Vector2 m_aimInputValue;
        private Vector3 m_aimDirection;
        private CharacterMouvement m_characterMouvement;
        [SerializeField] private Texture2D m_cursorTex;
        [SerializeField] private LineRenderer m_lineRenderer;
        [SerializeField] private Transform m_transformHead;
        [SerializeField] private LayerMask m_aimLayer;

        private Ray cameRay;
        private void Start()
        {
            Cursor.SetCursor(m_cursorTex, Vector2.zero, CursorMode.Auto);
            m_playerInput = GetComponent<PlayerInput>();
            m_characterMouvement = GetComponent<CharacterMouvement>();
        }


        private void Update()
        {
            AimFeedback();
        }


        private void AimFeedback()
        {
            m_lineRenderer.SetPosition(0, transform.position);
            if (m_aimDirection.magnitude > inputAim)
            {
                Vector3 direction2d = new Vector3(m_aimDirection.x, 0, m_aimDirection.z);
                float angleDir = Vector3.SignedAngle(m_transformHead.forward, direction2d.normalized, Vector3.up);
                m_transformHead.rotation *= Quaternion.AngleAxis(angleDir, Vector3.up);
                m_lineRenderer.SetPosition(1, transform.position + m_aimDirection * 70);
                Cursor.visible = true;
            }
            else
            {
                Cursor.visible = false;
                m_transformHead.localRotation = Quaternion.identity;
                m_lineRenderer.SetPosition(1, transform.position + transform.forward * 70);
            }
        }

        public void AimInput(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                m_aimInputValue = ctx.ReadValue<Vector2>();
               
            }
            if (ctx.canceled) m_aimInputValue = Vector2.zero;
            
            m_aimDirection = GetAimPoint();
        }

        public Vector3 GetAim()
        {
            if (m_aimInputValue.magnitude > inputAim)
            {
                return m_aimDirection;
            }

            // GetCloserTarget

            return m_characterMouvement.currentDirection;
        }

        private Vector3 GetAimPoint()
        {
            if (!IsGamepad())
            {
                Resolution currentResolution = Screen.currentResolution;
                m_aimInputValue = new Vector2(m_aimInputValue.x , m_aimInputValue.y);
               
                Ray aimRay = Camera.main.ScreenPointToRay(m_aimInputValue);
                cameRay = aimRay;
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(cameRay, out hit,150.0f,m_aimLayer.value))
                {
                    return ((hit.point+Vector3.up) - transform.position).normalized;
                }
                return m_characterMouvement.currentDirection;
            }
            return m_aimInputValue;
        }

        private bool IsGamepad()
        {
            return m_playerInput.currentControlScheme == "Gamepad";
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(cameRay.origin,cameRay.direction*100.0f);
        }
    }
}
