using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace SeekerOfSand
{
    namespace UI
    {
        public class UI_PlayerInfos : MonoBehaviour
        {
            public GameObject playerTarget;
            private Character.CharacterShoot m_characterShoot;

            [Header("Spell Canalisation Objects")]
            public Image m_canalisationBar;
            [Header("Spell Stacking Object")]
            public GameObject stackingUIHolder;
            public GameObject clockUIHolder;
            private TMP_Text[] m_stackingText;
            private Image[] m_stackingImageClock;


            void Start()
            {
                m_characterShoot = playerTarget.GetComponent<Character.CharacterShoot>();
                InitStackingObjects();
            }

            #region Spell Stacking

            public void InitStackingObjects()
            {
                m_stackingText = stackingUIHolder.GetComponentsInChildren<TMP_Text>();
                m_stackingImageClock = clockUIHolder.GetComponentsInChildren<Image>();
            }

            public void UpdateStackingObjects(int index, int value)
            {
                m_stackingText[index].text = value.ToString();
            }

            public void UpdateStackingObjects(int[] value)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    m_stackingText[i].text = value[i].ToString();
                }
            }
            #endregion

            #region Spell Canalisation
            public void ActiveSpellCanalisationUI()
            {
                m_canalisationBar.gameObject.SetActive(true);
            }

            public void UpdateSpellCanalisationUI(float value)
            {
                m_canalisationBar.fillAmount = value;
            }
            public void DeactiveSpellCanalisation()
            {
                m_canalisationBar.gameObject.SetActive(false);
            }

            public Image[] ReturnClock()
            {
                return m_stackingImageClock;
            }

            #endregion
        }
    }
}
