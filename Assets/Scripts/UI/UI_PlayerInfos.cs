using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            void Start()
            {
                m_characterShoot = playerTarget.GetComponent<Character.CharacterShoot>();
            }
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

            #endregion
        }
    }
}
