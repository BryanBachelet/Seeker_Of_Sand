using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuerhoubaGames.UI
{

    public class UI_LifeTarget : MonoBehaviour
    {
        [SerializeField] private Image m_progressBar;
        [SerializeField] private Image m_delayBar;
        [SerializeField] private TMP_Text m_targetNameText;


        public void ActiveLifeTarget(float ratio,string targetName)
        {
            gameObject.SetActive(true);
            m_progressBar.fillAmount = ratio;  
            m_delayBar.fillAmount = ratio;
            m_targetNameText.text = targetName;
        }

        public void DeactiveLifeTarget()
        {
            gameObject.SetActive(false);
        }



        
    }
}