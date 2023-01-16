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
                return new Vector3(m_aimInputValue.normalized.x, 0, m_aimInputValue.normalized.y);
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
