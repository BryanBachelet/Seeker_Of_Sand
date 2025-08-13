using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SpellSystem;
using UnityEngine.VFX;
using SeekerOfSand.Tools;
using System.Threading.Tasks;
namespace SeekerOfSand
{
    namespace UI
    {
        public class UI_PlayerInfos : MonoBehaviour
        {
            [HideInInspector] private GameObject playerTarget;
            [HideInInspector] private Character.CharacterShoot m_characterShoot;

            [Header("Spell Canalisation Objects")]
            [SerializeField] private Image m_canalisationBar;
            [SerializeField] private Image m_canalisationBarSegment;
            [SerializeField] private Image m_canalisationSpell;
            [Header("Spell Stacking Object")]
            [SerializeField] private TMP_Text[] m_stackingText;
            [SerializeField] private Image[] m_stackingImageClock;

            [SerializeField] private TMP_Text levelTaken;
            [HideInInspector] private int m_level;

            [SerializeField] private Animator canalisationBarDisplay;

            [SerializeField] private Sprite[] canalisationBarreSprites;

            [HideInInspector] private bool bufferFill = false;
            [HideInInspector] private float newFillAmount = 0;
            [HideInInspector] private float lastFillAmount = 0;
            [HideInInspector] private Color lastColor;
            [HideInInspector] private Sprite lastSprite;

            [SerializeField] private Animator upgradeScreenAnimator;
            [SerializeField] private TMP_Text textTier;
            [SerializeField] private GameObject tierUpEffect;
            [SerializeField] private Animator tierUpAnimation;
            [SerializeField] private SpellAttribution spellAttribution;
            [HideInInspector] private VisualEffect[] vfxTierUp = new VisualEffect[4];
            [GradientUsage(true)]
            [HideInInspector] private Gradient[] gradient1vfx = new Gradient[4];
            [GradientUsage(true)]
            [HideInInspector] private Gradient[] gradient2vfx = new Gradient[4];
            [HideInInspector] private MeshRenderer imgSpriteSpell;
            void Start()
            {
                playerTarget = GameObject.Find("Player");
                m_characterShoot = playerTarget.GetComponent<Character.CharacterShoot>();
                canalisationBarDisplay = m_canalisationBar.transform.parent.GetComponent<Animator>();
                //InitStackingObjects();
                m_level = 0;
            }

            #region Spell Stacking

            //public void InitStackingObjects()
            //{
            //    //m_stackingText = stackingUIHolder.GetComponentsInChildren<TMP_Text>();
            //    //m_stackingImageClock = clockUIHolder.GetComponentsInChildren<Image>();
            //}

            public void UpdateStackingObjects(int index, int value)
            {
                m_stackingText[index].text = value.ToString();
            }

            public void UpdateLevelSpell(int index, SpellProfil spellprofil)
            {
                //imgSpriteSpell.material = spellprofil.matToUse;
                tierUpEffect.SetActive(true);
                spellAttribution.AcquireSpellData(spellprofil);
                if (spellprofil.currentSpellTier == 1)
                {
                    
                    int indexElement = GeneralTools.GetElementalArrayIndex(spellprofil.TagList.element, true);
                    //for (int i = 0; i < vfxTierUp.Length; i++)
                    //{
                    //    vfxTierUp[i].SetGradient("Gradient1", gradient1vfx[indexElement]);
                    //    vfxTierUp[i].SetGradient("Gradient2", gradient2vfx[indexElement]);
                    //}
                    //tierUpAnimation.SetTrigger("UpgradeTo1");
                    if (spellprofil.levelSpellsProfiles[0] != null)
                    {
                        textTier.text = spellprofil.levelSpellsProfiles[0].description;
                    }
                    upgradeScreenAnimator.ResetTrigger("Reset");
                    upgradeScreenAnimator.SetTrigger("TierUpActivation");

                    GlobalSoundManager.PlayOneShot(59, Vector3.zero);
                    
                }
                else if (spellprofil.currentSpellTier == 2)
                {
                    tierUpEffect.SetActive(true);
                    //for (int i = 0; i < vfxTierUp.Length; i++)
                    //{
                    //    vfxTierUp[i].SetGradient("Gradient1", gradient1vfx[(int)spellprofil.tagData.element]);
                    //    vfxTierUp[i].SetGradient("Gradient2", gradient2vfx[(int)spellprofil.tagData.element]);
                    //}
                    //tierUpAnimation.SetTrigger("UpgradeTo2");
                    if (spellprofil.levelSpellsProfiles[1] != null)
                    {
                        textTier.text = spellprofil.levelSpellsProfiles[1].description;
                    }
                    upgradeScreenAnimator.ResetTrigger("Reset");
                    upgradeScreenAnimator.SetTrigger("TierUpActivation");
                    GlobalSoundManager.PlayOneShot(59, Vector3.zero);
                    
                }
                else if (spellprofil.currentSpellTier == 3)
                {
                    tierUpEffect.SetActive(true);
                    //for (int i = 0; i < vfxTierUp.Length; i++)
                    //{
                    //    vfxTierUp[i].SetGradient("Gradient1", gradient1vfx[(int)spellprofil.tagData.element]);
                    //    vfxTierUp[i].SetGradient("Gradient2", gradient2vfx[(int)spellprofil.tagData.element]);
                    //}
                    //tierUpAnimation.SetTrigger("UpgradeTo3");
                    if (spellprofil.levelSpellsProfiles[2] != null) 
                    { 
                        textTier.text = spellprofil.levelSpellsProfiles[2].description; 
                    }
                    upgradeScreenAnimator.ResetTrigger("Reset");
                    upgradeScreenAnimator.SetTrigger("TierUpActivation");
                    GlobalSoundManager.PlayOneShot(59, Vector3.zero);
                    
                }
                else
                {
                    //tierUpAnimation.ResetTrigger("UpgradeTo1");
                    //tierUpAnimation.ResetTrigger("UpgradeTo2");
                    //tierUpAnimation.ResetTrigger("UpgradeTo3");
                    upgradeScreenAnimator.SetTrigger("Reset");
                    upgradeScreenAnimator.ResetTrigger("TierUpActivation");
                    
                }
                //m_levelText[index].text = value.ToString();
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
                
                return;
                if (bufferFill)
                {
                    lastFillAmount = Mathf.Lerp(lastFillAmount, newFillAmount, 0.75f);

                    if (lastFillAmount - newFillAmount < 0.01f)
                    {
                        lastFillAmount = newFillAmount;
                        //bufferFill = false;
                        lastColor = GetRandomColorByPixel(lastSprite);
                    }
                    m_canalisationBar.color = lastColor;
                    m_canalisationBar.fillAmount = lastFillAmount;

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
                if (stack < 8 && stack > 0)
                {
                    m_canalisationBarSegment.sprite = canalisationBarreSprites[stack - 1];
                }
                else
                {
                    m_canalisationBarSegment.sprite = canalisationBarreSprites[6];
                }
            }


            public void ActiveSpellCanalisationUIv2(int maxStack, Image spell)
            {
                m_canalisationBar.gameObject.SetActive(true);
                m_canalisationBar.transform.parent.gameObject.SetActive(true);
                lastSprite = spell.sprite;
                m_canalisationSpell.sprite = lastSprite;
                lastColor = GetRandomColorByPixel(lastSprite);
                m_canalisationBar.color = lastColor;
                canalisationBarDisplay.SetBool("Canalisation", true);
                if (maxStack < 8 && maxStack > 0)
                {
                    m_canalisationBarSegment.sprite = canalisationBarreSprites[maxStack -1];
                }
                else
                {
                    m_canalisationBarSegment.sprite = canalisationBarreSprites[6];
                }
            }

            public void UpdateSpellCanalisationUI(float value, int stack)
            {
                newFillAmount = value;
                if (newFillAmount != lastFillAmount) { bufferFill = true; }
                //m_canalisationBar.fillAmount = value;
            }
            public void DeactiveSpellCanalisation()
            {
                //m_canalisationBar.gameObject.SetActive(false);
                m_canalisationBar.transform.parent.gameObject.SetActive(false);
                canalisationBarDisplay.SetBool("Canalisation", false);
                lastColor = new Vector4(0.25f, 0.25f, 0.25f, 1);
            }

            public void AddLevelTaken()
            {
                m_level += 1;
                levelTaken.text = "" + m_level;
            }

            public void CalculateRatio(int currentStack, int maxStack, float stackTimerRatio)
            {
                float step = 1 / (float)maxStack;
                float ratio = (currentStack * step) + (stackTimerRatio * step);
                m_canalisationBar.fillAmount = ratio;
               
            }

            public void MinusLevelTaken()
            {
                m_level -= 1;
                levelTaken.text = "" + m_level;
            }
            public Image[] ReturnClock()
            {
                //InitStackingObjects();
                return m_stackingImageClock;
            }

            public TMP_Text[] ReturnStack()
            {
                return m_stackingText;
            }

            public Color GetRandomColorByPixel(Sprite sprite)
            {
                float widht = sprite.rect.width;
                float height = sprite.rect.height;
                Vector2 rndPosition = new Vector2(widht / 2, height / 2);
                Color colorByPixel = sprite.texture.GetPixel((int)rndPosition.x, (int)rndPosition.y);

                return colorByPixel;
            }
            #endregion
        }
    }
}
