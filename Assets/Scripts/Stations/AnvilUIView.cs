using UnityEngine;
using UnityEngine.UI;
using GuerhoubaGames.GameEnum;
using UnityEngine.VFX;
using SeekerOfSand.Tools;
using TMPro;
using GuerhoubaGames.Resources;

namespace GuerhoubaGames.UI
{
    public class AnvilUIView : MonoBehaviour
    {
        [SerializeField] private GameObject m_panelAnvil;

        [Header("Upgrade Fragment Variables")]
        [SerializeField] public DragReceptacleUI m_receptableUI;
        [SerializeField] public FragmentUIView m_receptableImage;
        [SerializeField] public Artefact_UI_View m_receptacleImage_Artefact;
        [SerializeField] public FragmentUIView m_resultUpgradeImage;
        [SerializeField] public Artefact_UI_View m_resultUpgradeImage_Artefact;

        // Private variable Upgrade fragment
        private bool m_hasRecpetacle = false;
        private int m_indexArtecfactUpgradable;
        private ArtefactsInfos m_upgradePreviousClone;

        [Header("Merge Fragment Variables")]
        [SerializeField] public DragReceptacleUI[] receptacleUIs;
        [SerializeField] public FragmentUIView[] receptacleViews;
        [SerializeField] public Artefact_UI_View[] receptacleViews_Artefact;
        [SerializeField] public FragmentUIView resultMergeImage;
        [SerializeField] public Artefact_UI_View resultMergeImage_Artefact;


        private ArtefactsInfos m_mergeFragmentClone;

        [SerializeField] public FragmentUIView m_resultImage;
        [SerializeField] public Artefact_UI_View m_resultImage_Artefact;
        public UIDispatcher dispatcher;

        private CharacterArtefact m_characterArtefact;
        [HideInInspector] public AnvilBehavior anvilBehavior;


        public float tempsEcouleClic;
        public float timeToValidate;
        public Image uiButton;
        public bool actionValidate = false;
        public bool actionOnGoing = false;
        public VisualEffect[] vfxReinforcement;
        public Animator animator;

        [GradientUsage(true)]
        public Gradient[] colorByElement;

        [SerializeField] private TMP_Text[] m_priceText;
        //[SerializeField] private Image[] m_elementImageCristal;
        #region Unity Function


        public FragmentFusionUiDispatcher fragmentFusionManager;
        public void Start()
        {
            m_characterArtefact = GameState.instance.playerGo.GetComponent<CharacterArtefact>();
            m_receptableUI.OnDropEvent += OnDropInputUpgrade;

            for (int i = 0; i < receptacleUIs.Length; i++)
            {
                receptacleUIs[i].OnDropEvent += OnDropInputMerge;
                receptacleUIs[i].OnCtrlClick += CleanMergeFragment;
            }
            m_receptableImage.ResetFragmentUIView();
            m_receptacleImage_Artefact.ResetFragmentUIView();
            m_resultUpgradeImage.ResetFragmentUIView();
            m_resultUpgradeImage_Artefact.ResetFragmentUIView();
            CleanMergeInterface();
        }

        #endregion

        public void OpenUiAnvil()
        {
            m_panelAnvil.SetActive(true);
        }

        public void Update()
        {
            if (!actionOnGoing) return;
            else
            {
                tempsEcouleClic += Time.deltaTime;
                if (tempsEcouleClic > timeToValidate)
                {
                    actionValidate = true;
                    for (int i = 0; i < vfxReinforcement.Length; i++)
                    {
                        vfxReinforcement[i].SendEvent("Activation");

                    }

                    actionOnGoing = false;
                }
                else
                {
                    float progress = tempsEcouleClic / timeToValidate;
                    uiButton.fillAmount = progress;
                    for (int i = 0; i < vfxReinforcement.Length; i++)
                    {
                        vfxReinforcement[i].SetInt("Rate", (int)(progress * 100));

                    }
                }
            }
        }


        public void CloseUIAnvil()
        {
            CleanMergeInterface();
            anvilBehavior.ClearAnvil();
            m_panelAnvil.SetActive(false);
            
        }

        // Add this behavior for all actions of the anvil
        public void OnClicButton()
        {
            if (!m_hasRecpetacle) { return; }
            if (!actionOnGoing)
            {
                actionOnGoing = true;
                tempsEcouleClic = 0;
                for (int i = 0; i < vfxReinforcement.Length; i++)
                {
                    vfxReinforcement[i].Play();

                }
            }
            else return;
        }

        #region Upgrade Fragment Functions
        public void OnDropInputUpgrade(ReceptableData receptableData)
        {
            if (receptableData.objectType != CharacterObjectType.FRAGMENT)
            {
                Debug.LogWarning(" Anvil UI: This object is not fragment. It be place here");
                return;
            }

            // Check if fragment is already Tier 3;
            if (!anvilBehavior.IsFrgmentCanBeUpgrade(m_characterArtefact.artefactsList[m_indexArtecfactUpgradable]))
            {
                //TODO : Add Sound feedback for error of placement
                return;
            }

            UpdateUpgradeUI(receptableData.indexObject, receptableData.objectType);
        }

        private void UpdateUpgradeUI(int indexObject, CharacterObjectType characterObjectType, bool isUpdate = false)
        {
            // Check if fragment is already
            // 3;
            if (isUpdate && !anvilBehavior.IsFrgmentCanBeUpgrade(m_characterArtefact.artefactsList[m_indexArtecfactUpgradable]))
            {
                m_receptableImage.ResetFragmentUIView();
                m_receptacleImage_Artefact.ResetFragmentUIView();
                m_resultUpgradeImage.ResetFragmentUIView();
                m_resultUpgradeImage_Artefact.ResetFragmentUIView();
                return;
            }
            for (int i = 0; i < m_priceText.Length; i++)
            {
                m_priceText[i].text = "x" + 0;
            }


            m_indexArtecfactUpgradable = indexObject;
            anvilBehavior.currentArtefactReinforce = m_characterArtefact.artefactsList[m_indexArtecfactUpgradable];
            m_receptableImage.UpdateInteface(m_characterArtefact.artefactsList[m_indexArtecfactUpgradable]);
            m_receptacleImage_Artefact.UpdateInteface(m_characterArtefact.artefactsList[m_indexArtecfactUpgradable]);
            ArtefactsInfos clone = m_characterArtefact.artefactsList[m_indexArtecfactUpgradable].Clone();
            Destroy(m_upgradePreviousClone);
            m_upgradePreviousClone = clone;
            m_upgradePreviousClone.UpgradeTierFragment();
            m_hasRecpetacle = true;
            m_resultImage.UpdateInteface(m_upgradePreviousClone);
            m_resultImage_Artefact.UpdateInteface(m_upgradePreviousClone);
            //animator.SetBool("isAble", true);
            GameElement[] indexElementToUse = GeneralTools.GetBaseElementsArray(m_characterArtefact.artefactsList[m_indexArtecfactUpgradable].gameElement);
            for(int i = 0; i < indexElementToUse.Length;i++)
            {
                int indexGameElement = GeneralTools.GetElementalArrayIndex(indexElementToUse[i]);
                m_priceText[indexGameElement].text = "x" + (anvilBehavior.BuyPrice() / indexElementToUse.Length);
            }

            //m_elementImageCristal.sprite = GameResources.instance.cristalIconArray[indexGameElement];
            //for (int i = 0; i < vfxReinforcement.Length; i++)
            //{
            //    vfxReinforcement[i].SetGradient("GradientFlare", colorByElement[indexGameElement]);
            //}

        }
        public void OnMergeFragment(Transform transform)
        {
            dispatcher.CreateObject(transform.gameObject);
            m_receptableImage.ResetFragmentUIView();
            m_receptacleImage_Artefact.ResetFragmentUIView();
            m_resultImage.ResetFragmentUIView();
            m_resultImage_Artefact.ResetFragmentUIView();
        }
        public void OnUpgradeFragment(Transform transform)
        {
            if (!m_hasRecpetacle) return;

            BuyResult result = anvilBehavior.BuyUpgradeFragment();
            if (result != BuyResult.BUY) return;

            dispatcher.CreateObject(transform.gameObject);
            anvilBehavior.SetFragmentUpgrade();
            UpdateUpgradeUI(m_indexArtecfactUpgradable, CharacterObjectType.FRAGMENT, true);
            //animator.SetBool("isAble", false);

            m_receptableImage.ResetFragmentUIView();
            m_receptacleImage_Artefact.ResetFragmentUIView();
            m_resultImage.ResetFragmentUIView();
            m_resultImage_Artefact.ResetFragmentUIView();   

            for (int i = 0; i < m_priceText.Length; i++)
            {
                m_priceText[i].text = "x" + 0;
            }

        }
        #endregion

        #region Merge Functions
        public void OnDropInputMerge(ReceptableData receptableData)
        {
            if (receptableData.objectType != CharacterObjectType.FRAGMENT)
            {
                Debug.LogWarning(" Anvil UI: This object is not fragment. It be place here");
                return;
            }

            // TODO : Check if a fragment can be merge
            if (m_characterArtefact.artefactsList[m_indexArtecfactUpgradable].gameElement == GameElement.CHAOS)
            {
                //TODO : Add Sound feedback for error of placement
                return;
            }

            UpdateUIMerge(receptableData);

        }

        private void UpdateUIMerge(ReceptableData receptableData)
        {
            int currentFragmentToMergeIndex = receptableData.indexObject;
            // Update Anvil Behavior
            anvilBehavior.LockFragment(currentFragmentToMergeIndex, receptableData.indexReceptacle);
            fragmentFusionManager.ChangeFill(receptableData.indexReceptacle, m_characterArtefact.artefactsList[currentFragmentToMergeIndex].gameElement);
            // Update UI
            receptacleViews[receptableData.indexReceptacle].UpdateInteface(m_characterArtefact.artefactsList[currentFragmentToMergeIndex]);
            receptacleViews_Artefact[receptableData.indexReceptacle].UpdateInteface(m_characterArtefact.artefactsList[currentFragmentToMergeIndex]);

            if (!anvilBehavior.CanFragmentBeMerge()) return;

            m_mergeFragmentClone = anvilBehavior.MergeFragmentClone();
            resultMergeImage.UpdateInteface(m_mergeFragmentClone);
            resultMergeImage_Artefact.UpdateInteface(m_mergeFragmentClone);

        }

        private void CleanMergeInterface()
        {
            for (int i = 0; i < receptacleViews.Length; i++)
            {
                receptacleViews[i].ResetFragmentUIView();
                receptacleViews_Artefact[i].ResetFragmentUIView();
            }

            resultMergeImage.ResetFragmentUIView();
            resultMergeImage_Artefact.ResetFragmentUIView();
            fragmentFusionManager.ResetFill();

        }


        public void InputMergeFragment()
        {
            if (!anvilBehavior.CanFragmentBeMerge())
            {
                return;
            }

            BuyResult result = anvilBehavior.BuyMergeFragment();
            if (result != BuyResult.BUY) return;


            anvilBehavior.MergeFragment();
            CleanMergeInterface();
            anvilBehavior.ClearAnvil();
            //m_characterArtefact.SelectElement(fragment_List[currentFragmentNumber], artefactInfo);

        }

        public void CleanMergeFragment(ReceptableData data)
        {
           bool isValid = anvilBehavior.UnlockFragment(data.indexObject, data.indexReceptacle);

            if (!isValid) return;

            receptacleViews[data.indexReceptacle].ResetFragmentUIView();
            receptacleViews_Artefact[data.indexReceptacle].ResetFragmentUIView();
            fragmentFusionManager.RemoveFill(data.indexReceptacle);
            

            if (!anvilBehavior.CanFragmentBeMerge())
            {
                m_mergeFragmentClone = null;
                resultMergeImage.ResetFragmentUIView();
                resultMergeImage_Artefact.ResetFragmentUIView();
                return;
            }
            m_mergeFragmentClone = anvilBehavior.MergeFragmentClone();

            resultMergeImage.UpdateInteface(m_mergeFragmentClone);
            resultMergeImage_Artefact.UpdateInteface(m_mergeFragmentClone);

        }

        #endregion



    }
}
