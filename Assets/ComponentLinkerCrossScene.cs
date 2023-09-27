using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ComponentLinkerCrossScene : MonoBehaviour
{
    public Scene sceneUIBook;
    #region Player
    private GameObject m_PlayerObjectRef; //Object référence
    private Character.CharacterShoot m_characterShoot;
    private Experience_System m_ExperienceSysteme;
    private CharacterUpgrade m_CharacterUpgrade;
    private InteractionEvent m_InteractionEvent;
    private health_Player m_HealthPlayer;
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
                for(int j = 0; j < otherSceneGameObject[i].transform.childCount; j++)
                {
                    if(otherSceneGameObject[i].transform.GetChild(j).name == "Upgrade_Screen")
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
        #endregion
        #region Experience systeme
        m_ExperienceSysteme = m_PlayerObjectRef.GetComponent<Experience_System>();
        #endregion
        #region Character Upgrade
        m_CharacterUpgrade = m_PlayerObjectRef.GetComponent<CharacterUpgrade>();
        #endregion
        #region InteractionEvent
        m_InteractionEvent = m_PlayerObjectRef.GetComponent<InteractionEvent>();
        #endregion
        #region Health Player
        m_HealthPlayer = m_PlayerObjectRef.GetComponent<health_Player>();
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

        Debug.Log("Initialization done in : " + Time.timeSinceLevelLoad);
    }

}
