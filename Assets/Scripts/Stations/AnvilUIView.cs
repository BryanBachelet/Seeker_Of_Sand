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

        [Header("Upgrade Fragment Variables")]
        [SerializeField] public DragReceptacleUI m_receptableUI;
        [SerializeField] public FragmentUIView m_receptableImage;
        [SerializeField] public FragmentUIView m_resultUpgradeImage;

        // Private variable Upgrade fragment
        private bool m_hasRecpetacle = false;
        private int m_indexArtecfactUpgradable;
        private ArtefactsInfos m_upgradePreviousClone;

        [Header("Merge Fragment Variables")]
        [SerializeField] public DragReceptacleUI[] receptacleUIs;
        [SerializeField] public FragmentUIView[] receptacleViews;
        [SerializeField] public FragmentUIView resultMergeImage;

        private ArtefactsInfos m_mergeFragmentClone;

        private CharacterArtefact m_characterArtefact;
        [HideInInspector] public AnvilBehavior anvilBehavior;


        #region Unity Function

        public void Start()
        {
            m_characterArtefact = GameState.instance.playerGo.GetComponent<CharacterArtefact>();
            m_receptableUI.OnDropEvent += OnDropInputUpgrade;
            m_receptableImage.ResetFragmentUIView();
            m_resultUpgradeImage.ResetFragmentUIView();
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
            // Check if fragment is already Tier 3;
            if (isUpdate && !anvilBehavior.IsFrgmentCanBeUpgrade(m_characterArtefact.artefactsList[m_indexArtecfactUpgradable]))
            {
                m_receptableImage.ResetFragmentUIView();
                m_resultUpgradeImage.ResetFragmentUIView();
                return;
            }

            anvilBehavior.currentArtefactReinforce = m_characterArtefact.artefactsList[m_indexArtecfactUpgradable];
            m_indexArtecfactUpgradable = indexObject;
            m_receptableImage.UpdateInteface(m_characterArtefact.artefactsList[m_indexArtecfactUpgradable]);
            ArtefactsInfos clone = m_characterArtefact.artefactsList[m_indexArtecfactUpgradable].Clone();
            Destroy(m_upgradePreviousClone);
            m_upgradePreviousClone = clone;
            m_upgradePreviousClone.UpgradeTierFragment();
            m_hasRecpetacle = true;
            m_resultUpgradeImage.UpdateInteface(m_upgradePreviousClone);
        }

        public void OnUpgradeFragment()
        {
            if (!m_hasRecpetacle) return;

            BuyResult result = anvilBehavior.BuyUpgradeFragment();
            if (result != BuyResult.BUY) return;


            anvilBehavior.SetFragmentUpgrade();
            UpdateUpgradeUI(m_indexArtecfactUpgradable, CharacterObjectType.FRAGMENT, true);
        }
        #endregion

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
            anvilBehavior.currentFragmentMergeArray[receptableData.indexReceptacle] = m_characterArtefact.artefactsList[currentFragmentToMergeIndex];
            
            // Update UI
            receptacleViews[receptableData.indexReceptacle].UpdateInteface(m_characterArtefact.artefactsList[currentFragmentToMergeIndex]);
            m_mergeFragmentClone = anvilBehavior.MergeFragmentClone();
            resultMergeImage.UpdateInteface(m_mergeFragmentClone);

        }

        private void CleanMergeInterface()
        {
            for (int i = 0; i < receptacleViews.Length; i++)
            {
                receptacleViews[i].ResetFragmentUIView();
            }

            resultMergeImage.ResetFragmentUIView();
        }


        public void InputMergeFragment()
        {
            if(!anvilBehavior.CanFragmentBeMerge())
            {
                return;
            }

            BuyResult result = anvilBehavior.BuyMergeFragment();
            if (result != BuyResult.BUY) return;


            anvilBehavior.MergeFragment();
            CleanMergeInterface();

        }

    }
}
