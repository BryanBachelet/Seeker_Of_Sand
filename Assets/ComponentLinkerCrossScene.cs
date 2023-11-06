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
    [Header("---Player Parameter----------------------------")]
    [SerializeField] private GameObject m_skillBarHolderRef;
    [SerializeField] public List<VisualEffect> m_SpellReady = new List<VisualEffect>();
    [SerializeField] public List<Image> m_icon_Sprite;
    [SerializeField] public List<Image> m_spellGlobalCooldown;
    [SerializeField] public List<TextMeshProUGUI> m_TextSpellGlobalCooldown;
    #endregion
    #region Character Movement
    private Character.CharacterMouvement m_characterMovement;
    [SerializeField] public Animator m_uiStateAnimator;
    [SerializeField] private Image m_spriteState;
    #endregion
    #region eperience System
    private Experience_System m_ExperienceSysteme;
    [SerializeField] public Image m_LevelDisplayFill;
    [SerializeField] public VisualEffect m_levelUpEffectDisplay;
    #endregion
    #region Character Upgrade
    private CharacterUpgrade m_CharacterUpgrade;
    [SerializeField] public GameObject m_upgradeUIGO;
    [SerializeField] public TMP_Text m_upgradePoint_Txt;
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
    #region Character Dash
    private Character.CharacterDash m_CharacterDash;
    [SerializeField] public Image m_dashUI;
    #endregion
    #region TerrainLocation
    private TerrainLocationID m_TerrainLocationID;
    [SerializeField] public TMP_Text m_locationText;
    #endregion
    #region Serie Controller
    private SerieController m_serieController;
    [SerializeField] private TMP_Text m_multiplicatorDisplay;
    [SerializeField] private Image m_serieTimeDisplay;
    [SerializeField] public TMP_Text m_serieKillCount;
   
    #endregion
    #region cristal Inventory
    private CristalInventory m_cristalInventory;
    [SerializeField] private GameObject[] m_cristalDisplay = new GameObject[4];
    #endregion
    #endregion

    #region Camera
    [Header("---Camera Parameter----------------------------")]
    private GameObject m_MainCamera; //Object référence
    private CameraIntroMouvement m_CameraIntroMouvement;
    #endregion

    #region Player Upgrade

    private GameObject m_UpgradeScreenReference; //Object référence
    [Header("---Player Upgrade----------------------------")]
    public UpgradeUI m_UpgradeUI;
    [SerializeField] public UpgradeButton[] m_upgradeButtons = new UpgradeButton[9];
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
    [Header("---Render Book Parameter----------------------------")]
    public UpgradeUI upgradeUIRender;
    #endregion
    #region SpellBook
    [SerializeField] private UiSpellGrimoire m_spellBookObjectReference;
    #endregion

    #region Enemy Manager
    private GameObject m_enemyManagerObjectRef; //Object référence
    private Enemies.EnemyManager m_enemyManager;
    [Header("---Enemy Manager Parameter----------------------------")]
    [SerializeField] public Image[] m_imageLifeEvents = new Image[3];
    [SerializeField] public GameObject[] m_imageLifeEventObj = new GameObject[3];
    [SerializeField] public TMP_Text[] m_textProgressEvent = new TMP_Text[3];
    [SerializeField] public Image[] m_sliderProgressEvent = new Image[3];
    #region HealthManager
    private HealthManager m_healthManager;
    [SerializeField] private DamageHealthFD[] m_damageHealthFDs;
    [SerializeField] private Camera m_cameraReference;
    #endregion
    #endregion

    #region Day Cycle Controller

    private GameObject m_dayCycleControllerObjectReference; //Object référence
    private DayCyclecontroller m_dayCycleController;
    [Header("---Day Cycle Parameter----------------------------")]
    [SerializeField] public TMP_Text m_dayPhases;
    [SerializeField] public TMP_Text m_instruction;
    [SerializeField] public Image m_daySlider;
    [SerializeField] public Animator m_instructionAnimator;
    #endregion

    [SerializeField] private UIEndScreen m_uiEndScreen;

    public void Start()
    {
        StartCoroutine(DelayInitialization());
    }

    public IEnumerator DelayInitialization()
    {
        yield return new WaitForSeconds(2f);
        sceneUIBook = SceneManager.GetSceneByBuildIndex(5);
        ComponentInitialization();
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
        yield return new WaitForSeconds(2f);
        Debug.Log("Scene active = " + SceneManager.GetActiveScene().name);
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
            else if (otherSceneGameObject[i].name == "0-Persistent Element")
            {
                for(int j = 0; j < otherSceneGameObject[i].transform.childCount; j++)
                {
                    if(otherSceneGameObject[i].transform.GetChild(j).name == "Enemy Manager")
                    {
                        m_enemyManagerObjectRef = otherSceneGameObject[i].transform.GetChild(j).gameObject;
                    }
                }
                continue;
            }
            else if(otherSceneGameObject[i].name == "2-Moving Decors Element")
            {
                for(int j = 0; j < otherSceneGameObject[i].transform.childCount; j++)
                {
                    if(otherSceneGameObject[i].transform.GetChild(j).name == "Sun&Moon-Holder")
                    {
                        for(int k = 0; k < otherSceneGameObject[i].transform.GetChild(j).transform.childCount; k++)
                        {
                            if(otherSceneGameObject[i].transform.GetChild(j).transform.GetChild(k).name == "DayController")
                            {
                                m_dayCycleControllerObjectReference = otherSceneGameObject[i].transform.GetChild(j).transform.GetChild(k).gameObject;
                            }
                        }

                    }
                }
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
        #region Character Movement
        m_characterMovement = m_PlayerObjectRef.GetComponent<Character.CharacterMouvement>();
        m_characterMovement.m_uiStateAnimator = m_uiStateAnimator;
        m_characterMovement.m_spriteState = m_spriteState;
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
        m_CharacterUpgrade.m_upgradeUi = m_UpgradeUI;
        m_CharacterUpgrade.InitComponents();
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
        m_CharacterDash.m_dashUI = m_dashUI;
        #endregion
        #region Terrain Location ID
        m_TerrainLocationID = m_PlayerObjectRef.GetComponent<TerrainLocationID>();
        m_TerrainLocationID.locationText = m_locationText;
        #endregion
        #region Serie Controller
        m_serieController = m_PlayerObjectRef.GetComponent<SerieController>();
        m_serieController.m_multiplicatorDisplay = m_multiplicatorDisplay;
        m_serieController.m_serieTimeDisplay = m_serieTimeDisplay;
        m_serieController.m_serieKillCount = m_serieKillCount;
        #endregion
        #region Cristal Inventory
        m_cristalInventory = m_PlayerObjectRef.GetComponent<CristalInventory>();
        m_cristalInventory.cristalDisplay = m_cristalDisplay;
        m_cristalInventory.SetupText();
        #endregion
        #endregion

        #region Camera
        m_CameraIntroMouvement = m_MainCamera.GetComponent<CameraIntroMouvement>();
        #endregion

        #region Upgrade Screen
        //m_UpgradeUI = m_UpgradeScreenReference.GetComponent<UpgradeUI>();
        m_UpgradeUI.m_upgradeCharacter = m_PlayerObjectRef.GetComponent<CharacterUpgrade>();
        m_UpgradeScreenReference.GetComponent<UpgradeUI>().upgradeButtons = m_UpgradeUI.upgradeButtons;

        #endregion

        #region SpellBookUI
        m_spellBookObjectReference.m_characterShoot = m_characterShoot;
        #endregion

        #region EnemyManager
        m_enemyManager = m_enemyManagerObjectRef.GetComponent<Enemies.EnemyManager>();
        m_enemyManager.m_imageLifeEvents = m_imageLifeEvents;
        m_enemyManager.m_imageLifeEventsObj = m_imageLifeEventObj;
        m_enemyManager.m_textProgressEvent = m_textProgressEvent;
        m_enemyManager.m_sliderProgressEvent = m_sliderProgressEvent;
        #region HealthManager
        m_healthManager = m_enemyManagerObjectRef.GetComponent<HealthManager>();
        m_healthManager.m_damageHealthFDs = m_damageHealthFDs;
        m_healthManager.m_cameraReference = m_cameraReference;
        m_healthManager.InitTextFeedback();
        #endregion
        #endregion

        #region Day Cycle Controller
        m_dayCycleController = m_dayCycleControllerObjectReference.GetComponent<DayCyclecontroller>();
        m_dayCycleController.m_instructionAnimator = m_instructionAnimator;
        m_dayCycleController.m_DayPhases = m_dayPhases;
        m_dayCycleController.m_Instruction = m_instruction;
        m_dayCycleController.m_daySlider = m_daySlider;
        #endregion


        #region End Menu
        GameState.endMenu = m_uiEndScreen;
        #endregion
        Debug.Log("Initialization done in : " + Time.timeSinceLevelLoad);
    }

}
