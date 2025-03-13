using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class UiSpellGrimoire : MonoBehaviour
{
    public Character.CharacterShoot m_characterShoot;
    public Character.CharacterSpellBook m_characterInventory;
    public GameObject m_inGameUIObj;
    public GameObject mainUIObject;
    public GameObject spellGalerieObj;
    public GameObject spellEquipObj;
    public GameObject imageDragAndDrop;
    public GameObject arrowUp;
    public GameObject arrowDown;
    public GameObject spellGalerieObj_Button;
    public GameObject spellEquipObj_Button;
    public GameObject imageDragAndDrop_DecalUi;
    public GameObject arrowUp_DecalUi;
    public GameObject arrowDown_DecalUi;

    [HideInInspector] public bool isOpen;

    private uint m_currentPage = 0;
    private uint m_offset = 0;
    private uint m_maxPage = 0;

    private Image m_imageComponentDragAndDrop;
    private Vector2 m_mousePosition;
    private bool m_isSpellSelected;
    private UnityEngine.Events.UnityAction<int> m_spellIndexFunction;
    private UnityEngine.Events.UnityAction<int> m_spellBarIndexFunction;
    private int m_spellEquipIndex;
    private int m_indexSpellDrag;

    private Image[] m_spellsOwnImage;
    private Sprite[] m_currentSpritsSpell;
    private Image[] m_playerSpellEquip;

    // Spell Description Variable
    public GameObject spellDescriptionObj;
    private bool m_isSpellFocus;
    private Image m_iconSpellSelected;
    private TMP_Text[] m_textDescription;

    public Scene sceneUIBook;
    static public GameObject bookDisplayRoot;
    public GameObject bookDisplayRootDisplay;
    private UpgradeUIDecal m_uiDecalUpdaterDisplay;

    public ClicOnBook clicOnBook;
    public void Start()
    {
        //sceneUIBook = SceneManager.GetSceneByBuildIndex(1);
        //if (sceneUIBook != null)
        //{
        //    GameObject[] otherSceneGameObject = sceneUIBook.GetRootGameObjects();
        //}
        bookDisplayRoot = bookDisplayRootDisplay;
        //for (int i = 0; i < otherSceneGameObject.Length; i++)
        //{
        //   if(otherSceneGameObject[i].name  == "UIBookDisplay")
        //    {
        //        bookDisplayRoot = otherSceneGameObject[i];
        //        break;
        //    }
        //
        //}
        //Debug.Assert(bookDisplayRoot != null);

        bookDisplayRoot.SetActive(true);
        m_uiDecalUpdaterDisplay = bookDisplayRoot.GetComponent<UpgradeUIDecal>();
        spellGalerieObj = GameObject.FindWithTag("SpellIcon");
        spellEquipObj = GameObject.FindWithTag("SpellBar");
        FindUIElement();
        SetupSpellButtonFunction();
        if (m_uiDecalUpdaterDisplay) m_uiDecalUpdaterDisplay.ChangeStateDisplay(false);
        if (bookDisplayRoot) bookDisplayRoot.SetActive(false);

    }


    private void SetupSpellButtonFunction()
    {
        Button[] m_spellButton = spellGalerieObj_Button.GetComponentsInChildren<Button>();
        m_spellIndexFunction += ActiveFocusSpell;
        for (int i = 0; i < m_spellButton.Length; i++) // Setup function on event for the spell galery
        {
            int value = i;
            m_spellButton[i].onClick.AddListener(() => m_spellIndexFunction.Invoke(value));
        }

        m_spellBarIndexFunction += ActiveFocusSpellBar;
        m_spellButton = spellEquipObj_Button.GetComponentsInChildren<Button>();
        for (int i = 0; i < m_spellButton.Length; i++) // Setup function on event for the spell bar
        {
            int value = i;
            m_spellButton[i].onClick.AddListener(() => m_spellBarIndexFunction.Invoke(value));
        }
    }

    // This function is here to find all the UI Element to use
    private void FindUIElement()
    {
        if(spellGalerieObj) m_spellsOwnImage = spellGalerieObj.GetComponentsInChildren<Image>();
        if (spellEquipObj) m_playerSpellEquip = spellEquipObj.GetComponentsInChildren<Image>();
        if (imageDragAndDrop) m_imageComponentDragAndDrop = imageDragAndDrop.GetComponent<Image>();
        if (spellDescriptionObj) m_iconSpellSelected = spellDescriptionObj.GetComponentInChildren<Image>();
        if (spellDescriptionObj) m_textDescription = spellDescriptionObj.GetComponentsInChildren<TMP_Text>();
    }

    // Function to open the spell book ui
    public void OpenUI(Sprite[] spriteSpell, int[] indexSpellEquip)
    {

        GameState.ChangeState();

        spellDescriptionObj.SetActive(false);
        m_inGameUIObj.SetActive(false);
        mainUIObject.SetActive(true);
        bookDisplayRoot.SetActive(true);
        m_uiDecalUpdaterDisplay.ChangeStateDisplay(true);
        GlobalSoundManager.PlayOneShot(6, transform.position);

        isOpen = true;
        m_currentSpritsSpell = spriteSpell;

        m_maxPage = (uint)(m_currentSpritsSpell.Length / m_spellsOwnImage.Length);
        m_offset = 0;
        // ------ Setup spell image galery --------

        int index = 0;
        for (; index < m_currentSpritsSpell.Length && index < m_spellsOwnImage.Length; index++)
        {
            m_spellsOwnImage[index].enabled = true;
            m_spellsOwnImage[index].sprite = m_currentSpritsSpell[index];
        }

        for (; index < m_spellsOwnImage.Length; index++)
        {
            m_spellsOwnImage[index].enabled = false;
        }

        if (spriteSpell.Length > m_spellsOwnImage.Length)
        {
            arrowUp.SetActive(true);
        }

        // -------------

        for (int i = 0; i < m_playerSpellEquip.Length; i++)
        {
            int indexSpell = indexSpellEquip[i];
            m_playerSpellEquip[i].sprite = m_currentSpritsSpell[indexSpell];
        }

    }

    public void ActiveFocusSpell(int index)
    {
        spellDescriptionObj.SetActive(true);
        m_iconSpellSelected.sprite = m_currentSpritsSpell[m_offset + index];
        SpellSystem.SpellProfil info = m_characterInventory.GetSpecificSpell(index);
        m_textDescription[0].text = info.name;
        m_textDescription[1].text = info.description;
        m_uiDecalUpdaterDisplay.SpellFocusDisplay(info);
    }
    public void ActiveFocusSpellBar(int indexSpellSlot)
    {
        spellDescriptionObj.SetActive(true);
        int index = m_characterShoot.GetIndexFromSpellBar(indexSpellSlot);
        SpellSystem.SpellProfil info = m_characterInventory.GetSpecificSpell(index);
        m_iconSpellSelected.sprite = info.spell_Icon;
        m_textDescription[0].text = info.name;
        m_textDescription[1].text = info.description;
    }


    // Drag and Drop function
    public void ActiveDragAndDrop(int index)
    {
        m_imageComponentDragAndDrop.enabled = true;
        m_imageComponentDragAndDrop.sprite = m_currentSpritsSpell[m_offset + index];
        m_indexSpellDrag = (int)m_offset + index;
        m_isSpellSelected = true;
    }

    public void GetMouseLeftClick(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled && isOpen)
        {
            m_isSpellSelected = false;
            m_imageComponentDragAndDrop.enabled = false;

            //  Set Change of spell;
            if (m_spellEquipIndex != -1 && !m_characterShoot.IsSpellAlreadyUse(m_indexSpellDrag))
            {
                m_characterShoot.ChangeSpell(m_spellEquipIndex, m_indexSpellDrag);
                m_playerSpellEquip[m_spellEquipIndex].sprite = m_imageComponentDragAndDrop.sprite;
                m_spellEquipIndex = -1;
            }
        }
    }

    public void UpdateDragImage()
    {
        if (!m_isSpellSelected) return;

        m_mousePosition = Input.mousePosition;
        Vector2 imageSize = m_imageComponentDragAndDrop.rectTransform.rect.size / 4;
        imageSize.y *= -1;
        Vector2 resolution = new Vector2(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2);
        Vector2 pos = m_mousePosition - resolution + imageSize;
        m_imageComponentDragAndDrop.rectTransform.anchoredPosition = pos;
    }

    public void Update()
    {
        UpdateDragImage();
    }

    public void CloseUI()
    {
        GameState.ChangeState();
        mainUIObject.SetActive(false);
        bookDisplayRoot.SetActive(false);
        m_uiDecalUpdaterDisplay.ChangeStateDisplay(false);
        isOpen = false;
        arrowUp.SetActive(false);
        arrowDown.SetActive(false);
        m_inGameUIObj.SetActive(true);
        m_maxPage = m_currentPage = 0;
        GlobalSoundManager.PlayOneShot(30, transform.position);
    }


    #region Page Functions
    public void AdvancePage()
    {
        m_currentPage++;
        UpdateSpellGaleryImages();
    }

    public void BackOffPage()
    {
        m_currentPage--;
        UpdateSpellGaleryImages();
    }

    public void GetRelease(int index)
    {
        m_spellEquipIndex = index;
    }

    public void UnselectSpellEquip()
    {
        m_spellEquipIndex = -1;
    }

    public void UpdateSpellGaleryImages()
    {
        m_offset = m_currentPage * (uint)m_spellsOwnImage.Length;
        uint index = m_offset;

        // ------ Setup spell image galery --------
        arrowUp.SetActive(true);
        arrowDown.SetActive(true);
        for (index = 0; index < m_currentSpritsSpell.Length || index < (m_spellsOwnImage.Length + m_offset); index++)
        {
            m_spellsOwnImage[index].enabled = true;
            m_spellsOwnImage[index].sprite = m_currentSpritsSpell[index];
        }
        if (m_maxPage == m_currentPage)
        {
            arrowUp.SetActive(false);
            for (; index < m_spellsOwnImage.Length; index++)
            {
                m_spellsOwnImage[index].enabled = false;
            }
        }
        if (m_maxPage == 0)
        {
            arrowDown.SetActive(false);
        }
    }

    #endregion

    public void ClicInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            clicOnBook.getClicEffect(Input.mousePosition);
        }
    }
}