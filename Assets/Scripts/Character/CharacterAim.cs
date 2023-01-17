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
        private CharacterMouvement m_characterMouvement;
        [SerializeField] private Texture2D m_cursorTex;
        [SerializeField] private LineRenderer m_lineRenderer;
        [SerializeField] private Transform m_transformHead;
        private void Start()
        {
            Cursor.SetCursor(m_cursorTex, Vector2.zero, CursorMode.Auto);
            m_playerInput = GetComponent<PlayerInput>();
            m_characterMouvement = GetComponent<CharacterMouvement>();
        }


        private void Update()
        {
            if (m_aimInputValue.magnitude < inputAim)
            {
                Cursor.visible = false;
            }
            else
            {
                Cursor.visible = true;
            }
           

            m_lineRenderer.SetPosition(0, transform.position);
            if (m_aimInputValue.magnitude > inputAim)
            {
                    Vector3 direction = new Vector3(m_aimInputValue.normalized.x, 0, m_aimInputValue.normalized.y);
                float angleDir = Vector3.SignedAngle(m_transformHead.forward, direction.normalized, Vector3.up);
                Quaternion rot = Quaternion.AngleAxis(angleDir, Vector3.up);
                direction = rot * direction.normalized;
                m_transformHead.rotation *= Quaternion.AngleAxis(angleDir, Vector3.up);
                    // Orient Aim
                m_lineRenderer.SetPosition(1, transform.position +direction * 70);
            }else
            {
                m_transformHead.localRotation = Quaternion.identity;
                m_lineRenderer.SetPosition(1, transform.position +  transform.forward * 70);
            }
        }

        public void AimInput(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) m_aimInputValue = ctx.ReadValue<Vector2>();
            if (ctx.canceled) m_aimInputValue = Vector2.zero;

            if (!IsGamepad())
            {
                Resolution currentResolution = Screen.currentResolution;
                m_aimInputValue = new Vector2(m_aimInputValue.x - (currentResolution.width / 2.0f), m_aimInputValue.y - (currentResolution.height / 2.0f));
                m_aimInputValue = new Vector2(m_aimInputValue.x / (currentResolution.width / 2.0f), m_aimInputValue.y / (currentResolution.height / 2.0f));
            }

        }

        public Vector3 GetAim()
        {
            if (m_aimInputValue.magnitude > inputAim)
            {
                // Orient Aim
                Vector3 direction = new Vector3(m_aimInputValue.normalized.x, 0, m_aimInputValue.normalized.y);
                return direction;
            }

            // GetCloserTarget
            
            return m_characterMouvement.currentDirection;
        }

        private bool IsGamepad()
        {
            return m_playerInput.currentControlScheme == "Gamepad";
        }
    }
}
