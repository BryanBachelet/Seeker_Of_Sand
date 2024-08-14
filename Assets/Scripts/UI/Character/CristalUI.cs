using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace GuerhoubaGames.UI
{

    public class CristalUI : MonoBehaviour
    {

        [Tooltip("Order of cristal display: 0 = Water, 1 = Air, 2 = Fire, 3 = Earth")]
        public GameObject[] cristalDisplay = new GameObject[4]; //0 = Eau, 1 = Elec, 2 = Fire, 3 = Terre
        private Animator[] m_cristalAnimator = new Animator[4];
        [HideInInspector] public TMP_Text[] m_uiTextDisplay = new TMP_Text[4];


        public float timeDisplaying = 1f;


        void Start()
        {
            InitUIComponent();
        }

        public void InitUIComponent()
        {
            for (int i = 0; i < cristalDisplay.Length; i++)
            {
                m_cristalAnimator[i] = cristalDisplay[i].GetComponent<Animator>();
                m_uiTextDisplay[i] = cristalDisplay[i].GetComponentInChildren<TMP_Text>();
            }
        }


        public void UpdateUICristal(int value, int indexElement)
        {

            m_uiTextDisplay[indexElement].text = "" + value;
            StartCoroutine(DisplayUIFeedback(indexElement));
        }


        IEnumerator DisplayUIFeedback(int cristalType)
        {
            m_cristalAnimator[cristalType].SetBool("Open", true);
            yield return new WaitForSeconds(timeDisplaying);
            m_cristalAnimator[cristalType].SetBool("Open", false);
        }
    }

}
