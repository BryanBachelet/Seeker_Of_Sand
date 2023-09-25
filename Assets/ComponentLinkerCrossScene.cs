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
    private CameraIntroMouvement m_CameraIntroMouvement;

    private UpgradeUI m_UpgradeUI;

    private ObjectHealthSystem m_ObjectHealthSystemePillar;

    private ObjectHealthSystem m_ObjectHealthSystemeEvent;

    public void ComponentInitialization()
    {
        GameObject[] otherSceneGameObject = sceneUIBook.GetRootGameObjects();
        for (int i = 0; i < otherSceneGameObject.Length; i++)
        {
            if (otherSceneGameObject[i].name == "Player")
            {
                m_PlayerObjectRef = otherSceneGameObject[i];
                break;
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
        #region Experience Systeme

        #endregion
        #endregion
    }

    public void Start()
    {
        sceneUIBook = SceneManager.GetSceneByBuildIndex(1);
        ComponentInitialization();

    }
}
