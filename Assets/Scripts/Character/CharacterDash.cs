using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GuerhoubaGames.Character
{
    public class CharacterDash : MonoBehaviour
    {
        [Header("Dash Parameters")]
        [HideInInspector] private int m_currentStack = 1;
        [SerializeField] private int m_maxStack = 1;
        [SerializeField] private float m_dashDistance = 70;
        [SerializeField] private float m_dashDuration = .25f;
        [SerializeField] private float m_dashCooldownDuration = 4.0f;
        [HideInInspector] private GameLayer m_gameLayer;

        [Header("Dash UI Feedback")]
        [SerializeField] private Image m_dashUI;

        [HideInInspector] private float m_dashTimer = 0.0f;
        [HideInInspector] private float m_dashCooldownTimer = 0.0f;

        private Vector3 m_startPoint;
        private Vector3 m_endPoint;
        private Character.CharacterMouvement m_characterMouvement;
        private CharacterAim m_characterAim;
        private bool m_isDashValid;
        private bool m_isActiveCooldown;

        public GameObject[] characterModel;
        public GameObject vfxDash;

        [HideInInspector] private Material m_dashDecalFeedback;
        public UnityEngine.Rendering.HighDefinition.DecalProjector m_dashHolderDecal;

        [HideInInspector] private bool m_isDashFinish;
        [HideInInspector] private bool m_isSpellDash;
        [HideInInspector] private float m_spellDashDistance;
        [HideInInspector] private float m_spellDashDuration;

        private GameObject dashHolder;
        // Start is called before the first frame update
        void Start()
        {
            InitComponent();
        }


        private void InitComponent()
        {
            m_characterMouvement = GetComponent<CharacterMouvement>();
            m_characterAim = GetComponent<CharacterAim>();
            m_dashDecalFeedback = m_dashHolderDecal.material;
            dashHolder = m_dashUI.transform.parent.gameObject;
            Image[] tempsSpriteRef = dashHolder.GetComponentsInChildren<Image>();
            m_gameLayer = GameLayer.instance;
            //dashHolder.gameObject.SetActive(false);
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
            if (m_characterMouvement.mouvementState == CharacterMouvement.MouvementState.Knockback || m_characterMouvement.mouvementState == CharacterMouvement.MouvementState.SpecialSpell || m_currentStack <= 0 || m_isDashValid) return;
            m_currentStack--;
            Instantiate(vfxDash, transform.position, transform.rotation);
            characterModel[0].SetActive(false);
            //characterModel[1].SetActive(false);
            //characterModel[2].SetActive(true);
            //characterModel[2].transform.localScale = new Vector3(4f, 4f, 4f);
            m_isDashValid = CalculateDashEndPoint(m_dashDistance);
            if (!m_isDashValid) return;
            GlobalSoundManager.PlayOneShot(28, transform.position);
            m_characterMouvement.ChangeState(CharacterMouvement.MouvementState.Dash);
            if (m_characterMouvement.m_slidingEffectVfx.HasFloat("Rate")) m_characterMouvement.m_slidingEffectVfx.SetFloat("Rate", 100);
            m_dashTimer = 0.0f;

        }

        private bool CalculateDashEndPoint(float dashDistance, bool isCollisionEnemy = false, bool isDetermineByMouse = false) // Function that test where the player should arrive
        {
            Vector3 m_direction = Vector3.zero;
            if (!isDetermineByMouse)
            {

                m_direction = m_characterMouvement.GetDirection();
            }
            else
            {
                m_direction = m_characterAim.GetAimDirection();
                m_direction = m_characterMouvement.OrientateWithSlopeDirection(m_direction);
            }


            m_startPoint = transform.position;
            RaycastHit hit = new RaycastHit();
            Vector3 frontPoint = transform.position;

            LayerMask raycastLayerMaskUse = m_gameLayer.propsGroundLayerMask;
            if (isCollisionEnemy)
            {
                raycastLayerMaskUse = m_gameLayer.propsGroundLayerMask + m_gameLayer.enemisLayerMask;
            }
            // Check if obstacle in the front of the player 
            if (Physics.Raycast(transform.position, m_direction.normalized, out hit, dashDistance, raycastLayerMaskUse))
            {
                frontPoint = hit.point;
                m_endPoint = hit.point + hit.normal * 4.5f; ;
                return true;
            }
            else
            {
                frontPoint += m_direction.normalized * dashDistance;
                // Check if a ground exist to dash on it
                if (Physics.Raycast(frontPoint, Vector3.down, out hit, dashDistance, raycastLayerMaskUse))
                {
                    m_endPoint = hit.point + hit.normal * 4.5f;

                    return true;
                }

            }
            return false;


        }

        public bool CalculChargeEndPoint(float dashDistance, bool isCollisionEnemy = false, bool isDetermineByMouse = false)
        {
            Vector3 m_direction = Vector3.zero;
            if (!isDetermineByMouse)
            {

                m_direction = m_characterMouvement.GetDirection();
            }
            else
            {
                m_direction = m_characterAim.GetAimDirection();
                m_direction = m_characterMouvement.OrientateWithSlopeDirection(m_direction);
            }


            m_startPoint = transform.position;
            RaycastHit hit = new RaycastHit();
            Vector3 frontPoint = transform.position;

            LayerMask raycastLayerMaskUse = m_gameLayer.propsGroundLayerMask;
            if (isCollisionEnemy)
            {
                raycastLayerMaskUse = m_gameLayer.propsGroundLayerMask + m_gameLayer.enemisLayerMask;
            }

            CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();

            Vector3 p1 = capsuleCollider.center - new Vector3(0, capsuleCollider.height, 0);
            Vector3 p2 = capsuleCollider.center + new Vector3(0, capsuleCollider.height, 0);
            if (Physics.CapsuleCast(transform.position + p1, transform.position + p2, capsuleCollider.radius, m_direction.normalized, out hit, dashDistance, raycastLayerMaskUse))
            {
                frontPoint = hit.point;
                m_endPoint = hit.point + hit.normal * 4.5f + -m_direction.normalized * 2.0f;

                if (Physics.Raycast(m_endPoint, Vector3.down, out hit, dashDistance, m_gameLayer.propsGroundLayerMask))
                {
                    m_endPoint = hit.point + hit.normal * 2;

                    return true;
                }
                return true;
            }
            else
            {
                frontPoint += m_direction.normalized * dashDistance;
                // Check if a ground exist to dash on it
                if (Physics.Raycast(frontPoint, Vector3.down, out hit, dashDistance, raycastLayerMaskUse))
                {
                    m_endPoint = hit.point + hit.normal * 4.5f;

                    return true;
                }

            }
            return false;


        }

        private void EndDash() // Function call at the end of dash
        {
            m_characterMouvement.ChangeState(CharacterMouvement.MouvementState.None);
            m_isDashValid = false;
            if (m_characterMouvement.m_slidingEffectVfx.HasFloat("Rate")) m_characterMouvement.m_slidingEffectVfx.SetFloat("Rate", 0);
            if (!m_isSpellDash) m_isActiveCooldown = true;
            m_dashCooldownTimer = 0.0f;
            characterModel[0].SetActive(true);
            //characterModel[1].SetActive(true);
            //characterModel[2].SetActive(false);
            //characterModel[2].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }

        private void DashMouvement(float dashDuration)
        {
            if (!m_isDashValid) return;

            if (m_dashTimer > dashDuration)
            {
                EndDash();

                if (m_isSpellDash)
                {
                    m_isSpellDash = false;
                    m_isDashFinish = true;
                }
            }
            else
            {
                transform.position = Vector3.Lerp(m_startPoint, m_endPoint, m_dashTimer / dashDuration);
                m_dashTimer += Time.deltaTime;

            }
        }

        public void Update()
        {
            //m_maxStack = DayCyclecontroller.m_nightCountGlobal;
            if (!m_isSpellDash)
            {
                DashMouvement(m_dashDuration);
            }
            else
            {
                DashMouvement(m_spellDashDuration);
            }


            if (m_isActiveCooldown)
            {
                float dashFillFeedback = m_dashCooldownTimer / m_dashCooldownDuration;
                if (m_dashCooldownTimer > m_dashCooldownDuration)
                {

                    m_currentStack++;
                    m_dashCooldownTimer = 0.0f;
                    if (m_currentStack >= m_maxStack)
                    {
                        m_isActiveCooldown = false;
                        if (m_dashUI != null) m_dashUI.fillAmount = 0;
                    }

                }
                else
                {
                    m_dashCooldownTimer += Time.deltaTime;
                    if (m_dashUI != null) m_dashUI.fillAmount = 1-dashFillFeedback;
                }

                m_dashDecalFeedback.SetFloat("_Loading", dashFillFeedback);
            }

        }
        public void SpellDash(float duration, float distance)
        {

            Instantiate(vfxDash, transform.position, transform.rotation);
            characterModel[0].SetActive(false);
            //characterModel[1].SetActive(false);
            //characterModel[2].SetActive(true);
            //characterModel[2].transform.localScale = new Vector3(4f, 4f, 4f);
            m_isDashValid = CalculChargeEndPoint(distance, true, true);
            if (!m_isDashValid) return;
            m_characterMouvement.ChangeState(CharacterMouvement.MouvementState.SpecialSpell);
            m_spellDashDistance = distance;
            m_spellDashDuration = duration;
            m_isSpellDash = true;
            m_dashTimer = 0.0F;
            m_isDashFinish = false;
        }

        public void gainDash(int gainNumber, bool state)
        {
            if (state)
            {
                dashHolder.SetActive(true);
            }
            else
            {

            }

            m_maxStack++;
            m_currentStack = m_maxStack;
        }
    }

   

}
