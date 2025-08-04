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
        private Animator m_InventoryCristalAnimator;
        [HideInInspector] public TMP_Text[] m_uiTextDisplay = new TMP_Text[4];

        public GameObject cristalDissonance;
        private Animator cristalDissonanceAnimator;
        [HideInInspector] public TMP_Text m_uiTextDissonance;


        public float timeDisplaying = 1f;
        private bool currentInventoryStateOver = false;
        private bool currentInventoryState = false;

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
            m_InventoryCristalAnimator = cristalDissonance.transform.parent.GetComponent<Animator>();
            cristalDissonanceAnimator = cristalDissonance.GetComponent<Animator>();
            m_uiTextDissonance = cristalDissonance.GetComponentInChildren<TMP_Text>();
        }


        public void UpdateUICristal(int value, int indexElement)
        {

            m_uiTextDisplay[indexElement].text = "" + value;
            if(this.gameObject.activeSelf && !currentInventoryState && !currentInventoryStateOver) StartCoroutine(DisplayUIFeedback(indexElement));
        }

        public void UpdateDissonanceCristal(int value)
        {
            m_uiTextDissonance.text = "" + value;
            if (this.gameObject.activeSelf && !currentInventoryState && !currentInventoryStateOver) StartCoroutine(DisplayUIDissonanceFeedback());
        }

        IEnumerator DisplayUIFeedback(int cristalType)
        {
            m_cristalAnimator[cristalType].SetBool("Open", true);
            //m_InventoryCristalAnimator.SetBool("Open", true);
            yield return new WaitForSeconds(timeDisplaying);
            //if(currentInventoryStateOver == false && currentInventoryState == false) 
            //{ 
            //    m_InventoryCristalAnimator.SetBool("Open", false); 
            //}
            m_cristalAnimator[cristalType].SetBool("Open", false);

        }

        IEnumerator DisplayUIDissonanceFeedback()
        {
            cristalDissonanceAnimator.SetBool("Open", true);
            //m_InventoryCristalAnimator.SetBool("Open", true);
            yield return new WaitForSeconds(timeDisplaying);
            //if (currentInventoryStateOver == false && currentInventoryState == false) 
            //{ 
            //    m_InventoryCristalAnimator.SetBool("Open", false); 
            //}
            cristalDissonanceAnimator.SetBool("Open", false);
        }

        public void OpenInventoryOver()
        {
            currentInventoryStateOver = true;
            m_InventoryCristalAnimator.SetBool("Open", true);
            cristalDissonanceAnimator.SetBool("Open", false);
            for(int i = 0; i < m_cristalAnimator.Length; i++)
            {
                m_cristalAnimator[i].SetBool("Open", false);
            }

        }

        public void CloseInventoryOver()
        {
            if(currentInventoryState) { return; }
            currentInventoryStateOver = false;
            m_InventoryCristalAnimator.SetBool("Open", false);

        }

        public void OpenInventory()
        {
            currentInventoryState = true;
            m_InventoryCristalAnimator.SetBool("Open", true);
            cristalDissonanceAnimator.SetBool("Open", false);
            for (int i = 0; i < m_cristalAnimator.Length; i++)
            {
                m_cristalAnimator[i].SetBool("Open", false);
            }
        }

        public void CloseInventory()
        {
            currentInventoryStateOver = false;
            currentInventoryState = false;
            m_InventoryCristalAnimator.SetBool("Open", false);

        }
    }

}
