using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace SeekerOfSand
{
    namespace UI
    {
        /// <summary>
        /// Manage all UI Element from events
        /// </summary>
        public class UI_EventManager : MonoBehaviour
        {
            [SerializeField] private GameObject[] m_punketoneLifeBar;
            [SerializeField] private Image[] m_punketoneLifeBarfill;
            [SerializeField] private TMP_Text[] m_punketonLifeRemain;
            private Animator[] m_punketonLifeBarAnimator ;

            [Header("Event UI")]
            public Image[] m_imageLifeEvents = new Image[3];
            public GameObject[] m_imageLifeEventsObj = new GameObject[3];
            public TMP_Text[] m_textProgressEvent = new TMP_Text[3];
            public Image[] m_sliderProgressEvent = new Image[3];

            public void Start()
            {
             
                //if (m_punketoneLifeBar.Length > 0)
                //{
                //    m_punketonLifeBarAnimator = new Animator[m_punketoneLifeBar.Length];
                //    for (int i = 0; i < m_punketoneLifeBar.Length; i++)
                //    {
                //        m_punketonLifeBarAnimator[i] = m_punketoneLifeBar[i].GetComponent<Animator>();
                //    }
                //}
            }


            #region UI Boss
            // Allow to attribute a UI for a Punketon
            public void SetupUIBoss(int index)
            {
                m_punketoneLifeBar[index].SetActive(true);
                //m_punketonLifeBarAnimator[index].SetBool("Open", true);
            }

            // Update Punketon UI Event
            public void UpdateUIBossLifebar(int index, float value,float currentHP)
            {
                m_punketoneLifeBarfill[index].fillAmount = value;
                m_punketonLifeRemain[index].text = "" + currentHP;
            }

            public void RemoveUIBoss(int index)
            {
                m_punketoneLifeBar[index].SetActive(false);
                //m_punketonLifeBarAnimator[index].SetBool("Open", false);
            }
            #endregion

            public void SetupEventUI(ObjectHealthSystem objectHealthSystem,int index)
            {
                //objectHealthSystem.indexUIEvent = index;
                //objectHealthSystem.m_eventLifeUIFeedback = m_imageLifeEvents[index];
                //objectHealthSystem.m_eventLifeUIFeedbackObj = m_imageLifeEventsObj[index];
                //objectHealthSystem.m_eventProgressUIFeedback = m_textProgressEvent[index];

                objectHealthSystem.GetComponent<AltarBehaviorComponent>().m_eventProgressionSlider = m_sliderProgressEvent[index];
                //m_sliderProgressEvent[index].gameObject.SetActive(true);

                //m_imageLifeEventsObj[index].SetActive(true);
                //m_imageLifeEvents[index].gameObject.SetActive(true);
                //m_textProgressEvent[index].gameObject.SetActive(true);
            }

            public void RemoveEventUI(int index)
            {
                m_imageLifeEventsObj[index].SetActive(false);
                m_imageLifeEventsObj[index].SetActive(false);
                m_imageLifeEvents[index].gameObject.SetActive(false);
                m_textProgressEvent[index].gameObject.SetActive(false);
                m_sliderProgressEvent[index].gameObject.SetActive(false);
            }
        }


    }
}