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


        #region Unity Functions

        public void Awake()
        {
            // Init Top Goal Interface
            m_objectifAnimator = UiObjectifGO.GetComponent<Animator>();
            m_rewardAnimator = UiRewardGO.GetComponent<Animator>();

            m_objectifIcon = UiObjectifGO.GetComponentsInChildren<Image>()[1];
            m_objectifText = UiObjectifGO.GetComponentInChildren<TMP_Text>();

            m_rewardIcon = UiRewardGO.GetComponentsInChildren<Image>()[1];

            // Init Major Goal Interface
            m_majorGoalAnimator = majorGoalGO.GetComponent<Animator>();
        }

        public void Update()
        {
            UpdateMajorGoalInterface();
        }
        #endregion

        public void ActualizeRoomInfoInterface()
        {
            m_objectifAnimator.ResetTrigger("ActiveDisplay");
            m_rewardAnimator.ResetTrigger("ActiveDisplay");

            // Reward inteface update
            m_rewardIcon.sprite = m_rewardSpriteArray[(int)currentRoomManager.rewardType];

            int indexRoomType = Mathf.Clamp((int)currentRoomManager.currentRoomType, 0, m_objectifSpriteArray.Length-1);
            // Goal interface update
            m_objectifIcon.sprite = m_objectifSpriteArray[indexRoomType];
            m_objectifText.text = m_objectifTextArray[indexRoomType];

            m_objectifAnimator.SetTrigger("ActiveDisplay");
            m_rewardAnimator.SetTrigger("ActiveDisplay");
        }

        #region Major Goal Functions

        public void UpdateMajorGoalInterface()
        {
            if (!majorGoalGO.activeSelf || !m_hasMajorGoalChange) return;

            m_majorGoalSprite.fillAmount = 1.0f - m_majorGoalProgress;

            if (timeBeforeDecreaseDelay + m_timeLastUpdate > Time.time) return;

            if (m_isMajorGoalChanging)
            {
                  m_isMajorGoalChanging = false;
            }

            m_majorGoalSpriteDelay.fillAmount -= decreaseSpeed *Time.deltaTime;

            if(m_majorGoalSpriteDelay.fillAmount < m_majorGoalSprite.fillAmount)
            {
                m_majorGoalSpriteDelay.fillAmount = m_majorGoalSprite.fillAmount;
                m_hasMajorGoalChange = false;
            }

        }

        public void ActualizeMajorGoalProgress(float currentProgress)
        {
            if (currentProgress == m_majorGoalProgress) return;

            m_majorGoalProgress = currentProgress;
            m_timeLastUpdate = Time.time;
            m_isMajorGoalChanging = true;
            m_hasMajorGoalChange = true;
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
    };
}