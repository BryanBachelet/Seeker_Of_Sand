using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GuerhoubaGames.GameEnum;
using UnityEngine.Rendering;
using UnityEngine.VFX;
using UnityEditor;
using System;
using SeekerOfSand.Tools;
using TMPro;
using GuerhoubaGames.Resources;

namespace GuerhoubaGames.UI
{
    public class AnvilUIView : MonoBehaviour
    {
        [SerializeField] private GameObject m_panelAnvil;

        [Header("Upgrade Artefact Variables")]
        [SerializeField] public DragReceptacleUI m_receptableUI;
        [SerializeField] public FragmentUIView m_receptableImage;
        [SerializeField] public FragmentUIView m_resultImage;
        public UIDispatcher dispatcher;

        private int indexArtecfactUpgradable;
        private CharacterArtefact m_characterArtefact;
        [HideInInspector] public AnvilBehavior anvilBehavior;

        private ArtefactsInfos previousClone;

        private bool hasRecpetacle = false;

        public float tempsEcouleClic;
        public float timeToValidate;
        public Image uiButton;
        public bool actionValidate = false;
        public bool actionOnGoing = false;
        public VisualEffect[] vfxReinforcement;
        public Animator animator;

        [GradientUsage(true)]
        public Gradient[] colorByElement;

        [SerializeField] private TMP_Text m_priceText;
        [SerializeField] private Image m_elementImageCristal;
        #region Unity Function

        public void Start()
        {
            m_characterArtefact = GameState.instance.playerGo.GetComponent<CharacterArtefact>();
            m_receptableUI.OnDropEvent += OnDropInput;
            m_receptableImage.ResetFragmentUIView();
            m_resultImage.ResetFragmentUIView();
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
            m_panelAnvil.SetActive(false);
        }

        public void OnDropInput(int indexObject, CharacterObjectType characterObjectType)
        {
            if (characterObjectType != CharacterObjectType.FRAGMENT)
            {
                Debug.LogWarning(" Anvil UI: This object is not fragment. It be place here");
                return;
            }

            // Check if fragment is already Tier 3;
            if (!anvilBehavior.IsFrgmentCanBeUpgrade(m_characterArtefact.artefactsList[indexArtecfactUpgradable]))
            {
                //TODO : Add Sound feedback for error of placement
                return;
            }

            UpdateUpgradeUI(indexObject, characterObjectType);
        }

        private void UpdateUpgradeUI(int indexObject, CharacterObjectType characterObjectType, bool isUpdate = false)
        {
            // Check if fragment is already Tier 3;
            if (!anvilBehavior.IsFrgmentCanBeUpgrade(m_characterArtefact.artefactsList[indexArtecfactUpgradable]))
            {
                m_receptableImage.ResetFragmentUIView();
                m_resultImage.ResetFragmentUIView();
                return;
            }



            anvilBehavior.currentArtefactReinforce = m_characterArtefact.artefactsList[indexArtecfactUpgradable];
            indexArtecfactUpgradable = indexObject;
            m_receptableImage.UpdateInteface(m_characterArtefact.artefactsList[indexArtecfactUpgradable]);
            ArtefactsInfos clone = m_characterArtefact.artefactsList[indexArtecfactUpgradable].Clone();
            Destroy(previousClone);
            previousClone = clone;
            previousClone.UpgradeTierFragment();
            hasRecpetacle = true;
            m_resultImage.UpdateInteface(previousClone);
            animator.SetBool("isAble", true);
            int indexElementToUse = GeneralTools.GetElementalArrayIndex(m_characterArtefact.artefactsList[indexArtecfactUpgradable].gameElement);
            m_priceText.text = "x" + anvilBehavior.BuyPrice();
            m_elementImageCristal.sprite = GameResources.instance.cristalIconArray[indexElementToUse];
            for (int i = 0; i < vfxReinforcement.Length; i++)
            {
                vfxReinforcement[i].SetGradient("GradientFlare", colorByElement[indexElementToUse]);
                
                
            }
        }

        public void OnUpgradeFragment(Transform transform)
        {
            if (!hasRecpetacle) return;

            BuyResult result = anvilBehavior.BuyUpgradeFragment();
            if (result != BuyResult.BUY) return;

            dispatcher.CreateObject(transform.gameObject);
            anvilBehavior.SetFragmentUpgrade();
            UpdateUpgradeUI(indexArtecfactUpgradable, CharacterObjectType.FRAGMENT, true);
            animator.SetBool("isAble", false);

            m_receptableImage.ResetFragmentUIView();
            m_resultImage.ResetFragmentUIView();

            //m_receptableImage.ResetFragmentUIView();
            //m_resultImage.ResetFragmentUIView();
        }


        public void OnClicButton()
        {
            if(!hasRecpetacle) { return; }
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
    }
}
