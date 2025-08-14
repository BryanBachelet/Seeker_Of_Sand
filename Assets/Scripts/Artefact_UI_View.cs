using GuerhoubaGames.GameEnum;
using SeekerOfSand.Tools;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GuerhoubaGames.UI
{
    public class Artefact_UI_View : MonoBehaviour
    {
        [SerializeField] private MeshRenderer[] m_MainColor_Mesh;
        public Material[] m_MainColor_mat;
        [SerializeField] private MeshRenderer[] m_DarkerBorder_Mesh;
        public Material[] m_darkerBorder_mat;
        [SerializeField] private Image m_nameImg; // ?????
        [SerializeField] private MeshRenderer m_IconArtefact_Mesh;
        public Material m_IconArtefact_mat;
        [SerializeField] private MeshRenderer m_Glass_Mesh;
        private Material m_glass_mat;
        //[SerializeField] private Image m_elementImg;
        [SerializeField] private Image[] m_corner; // ?????
        [SerializeField] private TMPro.TMP_Text m_nameText;
        public TooltipTrigger tooltipTrigger;

        [Header("Lock Variables")]
        public Color greyLockColor;

        public FragmentCornerElemental m_fragmentCorner; // ?????

        public Material materialRef;
        private Material m_usedMaterial_Corner;

        public Image cadreArtefact;
        public Material materialCadre;

        public GameObject artefactDisplay;
        public GameObject placeHolderDisplay;
        private void Start()
        {
            //ActiveDisplay(false);
        }
        public void ActiveModeRestreint(bool isLock)
        {
            if (m_MainColor_mat.Length < 1) { setupComponent(); }
            for (int i = 0; i < m_MainColor_mat.Length; i++)
            {
                
                if (isLock)
                {
                    m_MainColor_mat[i].SetColor("_Color01", greyLockColor);
                    m_darkerBorder_mat[i].SetColor("_Color01", greyLockColor);
                    //m_IconArtefact_mat.SetColor.color = greyLockColor;
                    //m_elementImg.color = greyLockColor;
                }
                else
                {
                    m_MainColor_mat[i].SetColor("_Color01", Color.white);
                    m_darkerBorder_mat[i].SetColor("_Color01", Color.white);
                    //m_IconArtefact_sprite.color = Color.white;
                    //m_elementImg.color = Color.white;
                }
            }

        }

        public void UpdateInteface(ArtefactsInfos artefactsInfos)
        {
            FragmentUIRessources instanceResources = FragmentUIRessources.instance;
            if (m_MainColor_mat.Length < 1) { setupComponent(); }
            ActiveDisplay(true);
            // Index that point the base element (Water,Air,Fire,Earth)
            int indexBaseElement = GeneralTools.GetElementalArrayIndex(artefactsInfos.gameElement, true);
            int indexElement = (int)artefactsInfos.gameElement;
            if (indexBaseElement <= 0)
            {
                GameElement firstElement = GeneralTools.GetFirstBaseElement(artefactsInfos.gameElement);
                indexBaseElement = GeneralTools.GetElementalArrayIndex(firstElement, true);
            }


            //Debug.Assert(indexBaseElement != 0, "Artefact " + artefactsInfos.nameArtefact + " doesn't have element");
            if (m_fragmentCorner == null)
            {
                m_fragmentCorner = this.GetComponent<FragmentCornerElemental>();
            }
            m_fragmentCorner.ChangeFragmentDisplay(artefactsInfos);
            m_IconArtefact_Mesh.gameObject.SetActive(true);
            for (int i = 0; i < m_MainColor_mat.Length; i++)
            {
                GenerateGradient(m_MainColor_mat[i], m_darkerBorder_mat[i],artefactsInfos.gameElement);
            }
            m_glass_mat.SetColor("_Color", instanceResources.colorBackground[indexBaseElement]);
            cadreArtefact.material.SetTexture("_Gradient", instanceResources.backgroundSprite[indexElement].texture);
            m_IconArtefact_mat.SetTexture("_MainTex", artefactsInfos.icon.texture);
            //m_IconArtefact_sprite.sprite = artefactsInfos.icon.te;
            //m_IconArtefact_sprite.color = Color.white;
            m_nameText.text = artefactsInfos.nameArtefact;

            tooltipTrigger.header = "<u>" + artefactsInfos.nameArtefact + "</u>" + "<br><#A6A6A6>Quantity: " + (artefactsInfos.additionialItemCount + 1) + "<#FFFFFF>";
            tooltipTrigger.content = artefactsInfos.description;
            tooltipTrigger.IsActive = true;
        }

        public void ResetFragmentUIView()
        {
            FragmentUIRessources instanceResources = FragmentUIRessources.instance;
            if (m_MainColor_mat.Length < 1 ) { setupComponent(); }
            for (int i = 0; i < m_MainColor_mat.Length; i++)
            {
                GenerateGradient(m_MainColor_mat[i], m_darkerBorder_mat[i], GameElement.WATER);
                m_glass_mat.SetColor("_Color", instanceResources.colorBackground[0]);
            }
            m_IconArtefact_mat.SetTexture("_MainTex", null);
            m_IconArtefact_Mesh.gameObject.SetActive(false);
            m_nameText.text = "";
            ActiveDisplay(false);
            tooltipTrigger.IsActive = false;
            tooltipTrigger.HideTooltip();
        }

        public void setupComponent()
        {
            m_MainColor_mat = new Material[m_MainColor_Mesh.Length];
            m_darkerBorder_mat = new Material[m_DarkerBorder_Mesh.Length];
            for (int i = 0; i < m_MainColor_Mesh.Length; i++)
            {
                m_MainColor_mat[i] = m_MainColor_Mesh[i].material;
            }
            for (int i = 0; i < m_DarkerBorder_Mesh.Length; i++)
            {
                m_darkerBorder_mat[i] = m_DarkerBorder_Mesh[i].material;
            }
            m_IconArtefact_mat = m_IconArtefact_Mesh.material;
            m_glass_mat = m_Glass_Mesh.material;
            materialCadre = new Material(cadreArtefact.material);
            cadreArtefact.material = materialCadre;
        }

        public void GenerateGradient(Material mainColorMat, Material darkColorMat, GameElement element)
        {
            GameElement[] elements = GeneralTools.GetBaseElementsArray(element);
            Color mainColor = FragmentUIRessources.instance.colorBackground[(int)elements[0]];
            Color darkColor = Color.Lerp(mainColor, Color.black, 0.95f);
            mainColorMat.SetColor("_Color01", mainColor);
            darkColorMat.SetColor("_Color01", darkColor);
            mainColorMat.SetColor("_Color02", mainColor);
            darkColorMat.SetColor("_Color02", darkColor);
            mainColorMat.SetColor("_Color03", mainColor);
            darkColorMat.SetColor("_Color03", darkColor);
            for (int i = 0; i < elements.Length; i++)
            {
                int indexElement = (int)elements[i];
                mainColorMat.SetColor("_Color0" + i,FragmentUIRessources.instance.colorBackground[indexElement]);
                darkColorMat.SetColor("_Color0" + i, Color.Lerp(FragmentUIRessources.instance.colorBackground[indexElement], Color.black, 0.95f));
            }


        }

        public void ActiveDisplay(bool stateDisplay) //True = active le visuel de l'artefact (objet appelé Artefact). False = desactive le visuel de l'artefact et active de placeHolder
        {
            artefactDisplay.SetActive(stateDisplay);
            placeHolderDisplay.SetActive(!stateDisplay);
        }
    }
}
