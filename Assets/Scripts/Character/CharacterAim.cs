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
        private Vector2 m_aimValueTransform;
        private Vector3 m_aimDirection;
        private CharacterMouvement m_characterMouvement;
        private CharacterShoot m_characterShoot;
        [SerializeField] private Texture2D m_cursorTex;
        [SerializeField] private LineRenderer m_lineRenderer;
        [SerializeField] private Transform m_transformHead;
        [SerializeField] private LayerMask m_aimLayer;

        public GameObject projectorVisorObject;
        private Ray cameRay;
        private void Start()
        {
            Cursor.SetCursor(m_cursorTex, Vector2.zero, CursorMode.Auto);
            m_playerInput = GetComponent<PlayerInput>();
            m_characterMouvement = GetComponent<CharacterMouvement>();
            m_characterShoot = GetComponent<CharacterShoot>();
        }


        private void Update()
        {
            AimFeedback();
        }

        public Transform GetTransformHead()
        {
            return m_transformHead;
        }
        private void AimFeedback()
        {
            m_lineRenderer.SetPosition(0, transform.position);

            Vector3 direction2d = new Vector3(m_aimDirection.x, 0, m_aimDirection.z);
            float angleDir = Vector3.SignedAngle(m_transformHead.forward, direction2d.normalized, Vector3.up);
            m_transformHead.rotation *= Quaternion.AngleAxis(angleDir, Vector3.up);
            RaycastHit hit = new RaycastHit();
            float distance = m_characterShoot.weaponStat.range;
            if (Physics.Raycast(transform.position, m_aimDirection.normalized * distance, out hit, m_aimLayer))
            {
                distance = (hit.point - transform.position).magnitude;
            }

            m_lineRenderer.SetPosition(1, transform.position + m_aimDirection.normalized * distance);
            Cursor.visible = true;

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
            return (GetAimDestination() - transform.position).normalized;
        }

        public float GetAimMagnitude() { return m_aimValueTransform.magnitude; }


        public Vector3 GetAimDestination()
        {

            Vector3 aimValue = new Vector2(m_aimInputValue.x, m_aimInputValue.y);
            Ray aimRay = Camera.main.ScreenPointToRay(aimValue);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(aimRay, out hit, 150.0f, m_aimLayer.value))
            {
                return hit.point;
            }
            else
            {
                return  transform.position+  GetAim() * m_characterShoot.weaponStat.range;
            }
        }

   

        private Vector3 GetAimPoint()
        {
            if (!IsGamepad())
            {
                Resolution currentResolution = Screen.currentResolution;
                m_aimValueTransform = new Vector2(m_aimInputValue.x, m_aimInputValue.y);
                Ray aimRay = Camera.main.ScreenPointToRay(m_aimValueTransform);
                m_aimValueTransform = new Vector2(m_aimValueTransform.x - (currentResolution.width / 2.0f), m_aimValueTransform.y - (currentResolution.height / 2.0f));
                m_aimValueTransform = new Vector2(m_aimValueTransform.x / (currentResolution.width / 2.0f), m_aimValueTransform.y / (currentResolution.height / 2.0f));


                cameRay = aimRay;
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(cameRay, out hit, 150.0f, m_aimLayer.value))
                {
                    if (projectorVisorObject)                                               // Decal Projector for visor positionning 
                    {                                                                       //
                        projectorVisorObject.transform.position = hit.point;                // 
                    }                                                                       //
                    return ((hit.point + Vector3.up) - transform.position);

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
            Gizmos.DrawLine(transform.position,GetAimDestination());
        }
    }
}
