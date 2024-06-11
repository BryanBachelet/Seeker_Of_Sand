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
            public Image m_canalisationBarSegment;
            public Image m_canalisationSpell;
            [Header("Spell Stacking Object")]
            public GameObject stackingUIHolder;
            public GameObject clockUIHolder;
            private TMP_Text[] m_stackingText;
            private Image[] m_stackingImageClock;

            public TMP_Text levelTaken;
            private int m_level;

            public Animator canalisationBarDisplay;

            public Sprite[] canalisationBarreSprites;

            private bool bufferFill = false;
            private float newFillAmount = 0;
            private float lastFillAmount = 0;
            private Color lastColor;
            private Sprite lastSprite;
            void Start()
            {
                m_characterShoot = playerTarget.GetComponent<Character.CharacterShoot>();
                canalisationBarDisplay = m_canalisationBar.transform.parent.GetComponent<Animator>();
                InitStackingObjects();
                m_level = 0;
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
            private void Update()
            {
                if(bufferFill)
                {
                    lastFillAmount = Mathf.Lerp(lastFillAmount, newFillAmount, 0.75f);

                    if (lastFillAmount - newFillAmount < 0.01f)
                    {
                        lastFillAmount = newFillAmount;
                        bufferFill = false;
                        lastColor = GetRandomColorByPixel(lastSprite);
                    }
                    m_canalisationBar.fillAmount = lastFillAmount;
                    m_canalisationBar.color = lastColor;
                }
            }
            #region Spell Canalisation
            public void ActiveSpellCanalisationUI(int stack, Image spell)
            {
                m_canalisationBar.gameObject.SetActive(true);
                m_canalisationBar.transform.parent.gameObject.SetActive(true);
                lastSprite = spell.sprite;
                m_canalisationSpell.sprite = lastSprite;
                canalisationBarDisplay.SetBool("Canalisation", true);
                if (stack < 8 && stack >0 )
                {
                    m_canalisationBarSegment.sprite = canalisationBarreSprites[stack - 1];
                }
                else
                {
                    m_canalisationBarSegment.sprite = canalisationBarreSprites[6];
                }
            }

            public void UpdateSpellCanalisationUI(float value, int stack)
            {
                newFillAmount = value;
                if(newFillAmount != lastFillAmount) { bufferFill = true;  }
                //m_canalisationBar.fillAmount = value;
            }
            public void DeactiveSpellCanalisation()
            {
                //m_canalisationBar.gameObject.SetActive(false);
                m_canalisationBar.transform.parent.gameObject.SetActive(false);
                canalisationBarDisplay.SetBool("Canalisation", false);
                lastColor = new Vector4(0.25f,0.25f,0.25f,1);
            }

            public void AddLevelTaken()
            {
                m_level += 1;
                levelTaken.text = "" + m_level;
            }

            public void MinusLevelTaken()
            {
                m_level -= 1;
                levelTaken.text = "" + m_level;
            }
            public Image[] ReturnClock()
            {
                InitStackingObjects();
                return m_stackingImageClock;
            }

            public Color GetRandomColorByPixel(Sprite sprite)
            {
                float widht = sprite.rect.width;
                float height = sprite.rect.height;
                Vector2 rndPosition = new Vector2(widht/2, height/2);
                Color colorByPixel = sprite.texture.GetPixel((int)rndPosition.x, (int)rndPosition.y);

                return colorByPixel;
            }
            #endregion
        }
    }
}
