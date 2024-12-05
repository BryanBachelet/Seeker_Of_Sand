using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GuerhoubaGames.GameEnum;

namespace GuerhoubaGames.UI
{
    public class AnvilUIView : MonoBehaviour
    {
        [SerializeField] private GameObject m_panelAnvil;

        [Header("Upgrade Artefact Variables")]
        [SerializeField] public DragReceptacleUI m_receptableUI;
        [SerializeField] public FragmentUIView m_receptableImage;
        [SerializeField] public FragmentUIView m_resultImage;

        private int indexArtecfactUpgradable;
        private CharacterArtefact m_characterArtefact;
        [HideInInspector] public AnvilBehavior anvilBehavior;

        private ArtefactsInfos previousClone;

        private bool hasRecpetacle = false;

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
        }

        public void OnUpgradeFragment()
        {
            if (!hasRecpetacle) return;

            BuyResult result = anvilBehavior.BuyUpgradeFragment();
            if (result != BuyResult.BUY) return;


            anvilBehavior.SetFragmentUpgrade();
            UpdateUpgradeUI(indexArtecfactUpgradable, CharacterObjectType.FRAGMENT, true);
        }


    }
}
