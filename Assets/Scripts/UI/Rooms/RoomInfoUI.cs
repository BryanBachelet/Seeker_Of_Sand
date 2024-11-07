using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GuerhoubaGames.UI
{
    public class RoomInfoUI : MonoBehaviour
    {
        [Header("Room Top info")]
        public GameObject UiObjectifGO;
        public GameObject UiRewardGO;

        [Tooltip("The array need to follow objectif enum order.  Event = 0, Enemy= 1, Free =2, Merchant = 3")]
        [SerializeField] private Sprite[] m_objectifSpriteArray;
        [Tooltip("The array need to follow objectif enum order. Event = 0, Enemy= 1, Free =2, Merchant = 3")]
        [SerializeField] private string[] m_objectifTextArray;

        [Tooltip("The array need to follow Reward enum order. Upgrade= 0, Spell= 1, Artefact=2, Heal =3,Nothing = 4")]
        [SerializeField] private Sprite[] m_rewardSpriteArray;

        private Animator m_objectifAnimator;
        private Animator m_rewardAnimator;

        private Image m_rewardIcon;
        private Image m_objectifIcon;

        private TMP_Text m_objectifText;

        [Header("Room Major Goal")]
        public GameObject majorGoalGO;
        public Image m_majorGoalSprite;
        public Image m_majorGoalSpriteDelay;

        public float timeBeforeDecreaseDelay = 1;
        public float decreaseSpeed = 0.1f;

        private Animator m_majorGoalAnimator;

        private bool m_isMajorGoalChanging;
        private bool m_hasMajorGoalChange;
        private float m_majorGoalProgress;
        private float m_timeLastUpdate;

        private float m_startDelayValue;

        // General Element
        [HideInInspector] public RoomManager currentRoomManager;

        public Color[] colorUI = new Color[2];
        public RectTransform flareGameObject;
        public RectTransform[] transformsProgression = new RectTransform[2];
        private Animator flareAnimator;

        [SerializeField] private TMP_Text textProgress;
        #region Unity Functions

        public void Awake()
        {

            if (!this.enabled) return;

            // Init Top Goal Interface
            m_objectifAnimator = UiObjectifGO.GetComponent<Animator>();
            m_rewardAnimator = UiRewardGO.GetComponent<Animator>();

            m_objectifIcon = UiObjectifGO.GetComponentsInChildren<Image>()[1];
            m_objectifText = UiObjectifGO.GetComponentInChildren<TMP_Text>();

            m_rewardIcon = UiRewardGO.GetComponentsInChildren<Image>()[1];

            // Init Major Goal Interface
            m_majorGoalAnimator = majorGoalGO.GetComponent<Animator>();

            if (flareGameObject != null)
            {
                flareAnimator = flareGameObject.GetComponent<Animator>();
            }
        }

        public void Update()
        {
            UpdateMajorGoalInterface();
        }
        #endregion

        public void ActualizeRoomInfoInterface()
        {
          if(m_objectifAnimator.isActiveAndEnabled)  m_objectifAnimator.ResetTrigger("ActiveDisplay");
            if (m_rewardAnimator.isActiveAndEnabled) m_rewardAnimator.ResetTrigger("ActiveDisplay");

            // Reward inteface update
            m_rewardIcon.sprite = m_rewardSpriteArray[(int)currentRoomManager.rewardType];

            int indexRoomType = Mathf.Clamp((int)currentRoomManager.currentRoomType, 0, m_objectifSpriteArray.Length - 1);
            // Goal interface update
            m_objectifIcon.sprite = m_objectifSpriteArray[indexRoomType];
            m_objectifText.text = m_objectifTextArray[indexRoomType];

            if (m_objectifAnimator.isActiveAndEnabled) m_objectifAnimator.SetTrigger("ActiveDisplay");
            if (m_rewardAnimator.isActiveAndEnabled) m_rewardAnimator.SetTrigger("ActiveDisplay");
        }

        #region Major Goal Functions

        public void UpdateMajorGoalInterface()
        {
            if (!majorGoalGO.activeSelf || !m_hasMajorGoalChange) return;

            m_majorGoalSprite.fillAmount = 1.0f - m_majorGoalProgress;
            flareGameObject.position = Vector3.Lerp(transformsProgression[0].position, transformsProgression[1].position, 1.0f - m_majorGoalProgress);
            //flareAnimator.SetBool("BufferProgression", true);
            flareAnimator.ResetTrigger("CancelBuffer");
            flareAnimator.SetTrigger("ActiveBuffer");
            if (timeBeforeDecreaseDelay + m_timeLastUpdate > Time.time) { m_majorGoalSprite.color = Color.Lerp(colorUI[0], colorUI[1], 1 - ((timeBeforeDecreaseDelay + m_timeLastUpdate) - Time.time)); return; }

            //flareAnimator.SetBool("BufferProgression", false);
            flareAnimator.ResetTrigger("ActiveBuffer");
            flareAnimator.SetTrigger("CancelBuffer");
            if (m_isMajorGoalChanging)
            {
                m_isMajorGoalChanging = false;

            }

            m_majorGoalSpriteDelay.fillAmount -= decreaseSpeed * Time.deltaTime;

            //m_majorGoalSprite.color = Color.Lerp(m_majorGoalSprite.color, colorUI[1], 0.1f);

            if (m_majorGoalSpriteDelay.fillAmount < m_majorGoalSprite.fillAmount)
            {
                m_majorGoalSpriteDelay.fillAmount = m_majorGoalSprite.fillAmount;
                flareAnimator.SetTrigger("CancelBuffer");
                m_hasMajorGoalChange = false;
                m_majorGoalSprite.color = colorUI[1];
            }

        }

        public void ActualizeMajorGoalProgress(float currentProgress)
        {
            if (currentProgress == m_majorGoalProgress) return;

            m_majorGoalProgress = currentProgress;
            m_timeLastUpdate = Time.time;
            m_isMajorGoalChanging = true;
            m_hasMajorGoalChange = true;
            m_majorGoalSprite.color = colorUI[0];
        }

        public void ActiveMajorGoalInterface()
        {
            majorGoalGO.SetActive(true);
            m_majorGoalSprite.fillAmount = 1;
            m_majorGoalSpriteDelay.fillAmount = 1;
            m_majorGoalProgress = 0;
            m_majorGoalAnimator.SetBool("MajorDisplay", true);
        }

        public void DeactivateMajorGoalInterface()
        {
            m_majorGoalAnimator.SetBool("MajorDisplay", false);
            majorGoalGO.SetActive(false);
        }

        #endregion

        public void UpdateTextProgression(int currentProgress, int currentGoal)
        {
            textProgress.text = "" + currentProgress + "/" + currentGoal;
        }
    };
}