using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;
public class ComponentLinkerCrossScene : MonoBehaviour
{
    public Scene sceneUIBook;
    #region Player

    private GameObject m_PlayerObjectRef; //Object référence
    #region Character Shoot
    private Character.CharacterShoot m_characterShoot;
    [SerializeField] private GameObject m_skillBarHolderRef;
    [SerializeField] public List<VisualEffect> m_SpellReady = new List<VisualEffect>();
    [SerializeField] public List<Image> m_icon_Sprite;
    [SerializeField] public List<Image> m_spellGlobalCooldown;
    [SerializeField] public List<TextMeshProUGUI> m_TextSpellGlobalCooldown;
    #endregion
    #region eperience System
    private Experience_System m_ExperienceSysteme;
    [SerializeField] public Image m_LevelDisplayFill;
    [SerializeField] public VisualEffect m_levelUpEffectDisplay;
    #endregion
    #region Character Upgrade
    private CharacterUpgrade m_CharacterUpgrade;
    [SerializeField] public GameObject m_upgradeUIGO;
    [SerializeField] public Text m_upgradePoint_Txt;
    [SerializeField] public Text m_levelDisplay;
    #endregion
    #region Intercation Event
    private InteractionEvent m_InteractionEvent;
    [SerializeField] public GameObject m_ui_HintInteraction;
    #endregion
    #region health Player
    private health_Player m_HealthPlayer;
    [SerializeField] public Image m_healthBar_Slider_CurrentHealth;
    [SerializeField] public Image m_healthBar_Slider_Quarter;
    #endregion
    private Character.CharacterDash m_CharacterDash;
    private TerrainLocationID m_TerrainLocationID;
    #endregion
    #region Camera
    private GameObject m_MainCamera; //Object référence
    private CameraIntroMouvement m_CameraIntroMouvement;
    #endregion
    #region Player Upgrade
    private GameObject m_UpgradeScreenReference; //Object référence
    private UpgradeUI m_UpgradeUI;

    #endregion
    #region Pillar Health
    private GameObject m_PillarObject; //Object référence
    private ObjectHealthSystem m_ObjectHealthSystemePillar;
    #endregion
    #region Altar Health
    private GameObject m_AltarObject;
    private ObjectHealthSystem m_ObjectHealthSystemeEvent;
    #endregion

    #region render book object
    public UpgradeUI upgradeUIRender;
    #endregion

    #region SpellBook
    [SerializeField] private UiSpellGrimoire m_spellBookObjectReference;
    #endregion
    public void Start()
    {
        StartCoroutine(DelayInitialization());
    }

    public IEnumerator DelayInitialization()
    {
        yield return new WaitForSeconds(2f);
        sceneUIBook = SceneManager.GetSceneByBuildIndex(5);
        ComponentInitialization();
        SceneManager.SetActiveScene(sceneUIBook);
    }
    public void ComponentInitialization()
    {

        GameObject[] otherSceneGameObject = sceneUIBook.GetRootGameObjects();
        for (int i = 0; i < otherSceneGameObject.Length; i++)
        {
            if (otherSceneGameObject[i].name == "Player")
            {
                m_PlayerObjectRef = otherSceneGameObject[i];
                continue;
            }
            else if (otherSceneGameObject[i].name == "Main Camera")
            {
                m_MainCamera = otherSceneGameObject[i];
                continue;
            }
            else if (otherSceneGameObject[i].name == "CanvasPrincipal")
            {
                for (int j = 0; j < otherSceneGameObject[i].transform.childCount; j++)
                {
                    if (otherSceneGameObject[i].transform.GetChild(j).name == "Upgrade_Screen")
                    {
                        m_UpgradeScreenReference = otherSceneGameObject[i].transform.GetChild(j).gameObject;
                    }
                }
                //m_UpgradeScreenReference = otherSceneGameObject[i];
                Debug.Log("Upgrade Screen acquired");
                continue;
            }

        }

        #region Player
        #region Character Shoot
        m_characterShoot = m_PlayerObjectRef.GetComponent<Character.CharacterShoot>();
        m_characterShoot.m_SkillBarHolder = m_skillBarHolderRef;
        m_characterShoot.m_SpellReady = m_SpellReady;
        m_characterShoot.icon_Sprite = m_icon_Sprite;
        m_characterShoot.m_spellGlobalCooldown = m_spellGlobalCooldown;
        m_characterShoot.m_TextSpellGlobalCooldown = m_TextSpellGlobalCooldown;
        m_characterShoot.InitCapsule();
        m_characterShoot.InitComponent();

        #endregion
        #region Experience systeme
        m_ExperienceSysteme = m_PlayerObjectRef.GetComponent<Experience_System>();
        m_ExperienceSysteme.m_LevelDisplayFill = m_LevelDisplayFill;
        m_ExperienceSysteme.levelUpEffectUi = m_levelUpEffectDisplay;
        #endregion
        #region Character Upgrade
        m_CharacterUpgrade = m_PlayerObjectRef.GetComponent<CharacterUpgrade>();
        m_CharacterUpgrade.upgradeUiGO = m_upgradeUIGO;
        m_CharacterUpgrade.m_upgradePoint = m_upgradePoint_Txt;
        m_CharacterUpgrade.m_LevelDisplay = m_levelDisplay;
        #endregion
        #region InteractionEvent
        m_InteractionEvent = m_PlayerObjectRef.GetComponent<InteractionEvent>();
        m_InteractionEvent.ui_HintInteractionObject = m_ui_HintInteraction;
        #endregion
        #region Health Player
        m_HealthPlayer = m_PlayerObjectRef.GetComponent<health_Player>();
        m_HealthPlayer.m_SliderCurrentHealthHigh = m_healthBar_Slider_CurrentHealth;
        m_HealthPlayer.m_SliderCurrentQuarterHigh = m_healthBar_Slider_Quarter;
        #endregion
        #region Character Dash
        m_CharacterDash = m_PlayerObjectRef.GetComponent<Character.CharacterDash>();
        #endregion
        #region Terrain Location ID
        m_TerrainLocationID = m_PlayerObjectRef.GetComponent<TerrainLocationID>();
        #endregion
        #endregion

        #region Camera
        m_CameraIntroMouvement = m_MainCamera.GetComponent<CameraIntroMouvement>();
        #endregion

        #region Upgrade Screen
        m_UpgradeUI = m_UpgradeScreenReference.GetComponent<UpgradeUI>();
        #endregion

        #region SpellBookUI
        m_spellBookObjectReference.m_characterShoot = m_characterShoot;
        #endregion

        Debug.Log("Initialization done in : " + Time.timeSinceLevelLoad);
    }

}
