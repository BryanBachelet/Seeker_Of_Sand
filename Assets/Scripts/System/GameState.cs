using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.IO;

public class ObjectState
{
    public bool isPlaying = true;
}

public struct EndInfoStats
{
    public float durationGame;
    public int nightValidate;
    public int enemyKill;
    public float bigestCombo;
    public float altarSuccessed;
    public float altarRepeated;


    public byte[] SaveData()
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter binary = new BinaryWriter(stream);
        binary.Write(durationGame);
        binary.Write(nightValidate);
        binary.Write(enemyKill);
        binary.Write(bigestCombo);
        binary.Write(altarSuccessed);
        binary.Write(altarRepeated);

        return stream.ToArray();
    }


    public void ReadData(BinaryReader binaryReader)
    {
        durationGame = binaryReader.ReadSingle();
        nightValidate = binaryReader.ReadInt32();
        enemyKill = binaryReader.ReadInt32();
        bigestCombo = binaryReader.ReadSingle();
        altarSuccessed = binaryReader.ReadSingle();
        altarRepeated = binaryReader.ReadSingle();
    }

    public bool HasSuperiorValue(EndInfoStats statToCompare)
    {
        if (durationGame < statToCompare.durationGame) return true;
        if (nightValidate < statToCompare.nightValidate) return true;
        if (enemyKill < statToCompare.enemyKill) return true;
        if (bigestCombo < statToCompare.bigestCombo) return true;
        if (altarSuccessed < statToCompare.altarSuccessed) return true;
        if (altarRepeated < statToCompare.altarRepeated) return true;

        return false;
    }
}

public class GameState : MonoBehaviour
{
    private static Enemies.EnemyManager m_enemyManager;
    private static List<ObjectState> listObject = new List<ObjectState>(0);

    public static UIEndScreen endMenu;
    public static string profileName = "";

    [SerializeField] private static bool m_isPlaying = true;

    private static bool m_isDeath;
    private static float m_deathRatio;
    [SerializeField] private float m_timeBetweenDeath = 0.8f;
    [SerializeField] private int m_indexScene = 0;
    private bool m_isDeathProcessusActive;
    private float m_timerBetweenDeath = 0.0f;
    private GameObject m_pauseMenuObj;

    private bool m_activeDebug = true;
    private GameManager m_gmComponent;

    private Scene scene;
    private bool m_activeSceneEvent = false;
   
    public void Start()
    {
        scene = (SceneManager.GetSceneByBuildIndex(5));
        m_isDeath = false;
        m_enemyManager = GetComponent<Enemies.EnemyManager>();

        GameObject gm = GameObject.Find("GameManager");
        if (gm != null)
        {
            if (m_activeDebug) Debug.Log("Found Game manager object ");
            m_gmComponent = gm.GetComponent<GameManager>();
            profileName = m_gmComponent.profileName;
            Character.CharacterShoot charaShoot = m_enemyManager.m_playerTranform.GetComponent<Character.CharacterShoot>();
            charaShoot.m_aimModeState = m_gmComponent.m_aimModeChoose;
            charaShoot.UpdateFeedbackAimLayout();
            
        }
        else
        {
           // if (m_activeDebug) Debug.LogError("Couldn't found Game manager object ");
        }
    }

    public static void DeathActivation()
    {
        m_isDeath = true;
    }

    public void ActiveMainScene()
    {
        if (scene.isLoaded && !m_activeSceneEvent)
        {
            SceneManager.SetActiveScene(scene);
            m_activeSceneEvent = true;
        }
    }

    public void Update()
    {
        ActiveMainScene();

        if (m_isDeath)
        {
            if (!m_isDeathProcessusActive)
                DeathEffect();

            if (m_timerBetweenDeath > m_timeBetweenDeath)
            {

                m_isDeath = false;
                m_timerBetweenDeath = 0;
                SceneManager.LoadScene(m_indexScene);
            }
            else
            {
                m_timerBetweenDeath += Time.deltaTime;
            }
        }
    }

    public void DeathEffect()
    {
        ChangeState();
        m_isDeathProcessusActive = true;
    }

    public static void ChangeState()
    {
        m_isPlaying = !m_isPlaying;
        for (int i = 0; i < listObject.Count; i++)
        {
            listObject[i].isPlaying = m_isPlaying;
        }
        m_enemyManager.ChangePauseState(m_isPlaying);
    }

    public static void SetState(bool state)
    {
        m_isPlaying = state;
        for (int i = 0; i < listObject.Count; i++)
        {
            listObject[i].isPlaying = m_isPlaying;
        }
        m_enemyManager.ChangePauseState(m_isPlaying);
    }

    public static void AddObject(ObjectState state)
    {
        listObject.Add(state);
        state.isPlaying = m_isPlaying;
    }

    public static void RemoveObject(ObjectState state)
    {
        listObject.Remove(state);
    }

    public void OpenGameMenu(InputAction.CallbackContext ctx)
    {
        m_pauseMenuObj.SetActive(!m_pauseMenuObj.activeSelf);
    }

    public static bool IsPlaying()
    {
        return m_isPlaying;
    }

    public static void LaunchEndMenu()
    {
        EndInfoStats stats = m_enemyManager.FillEndStat();
        endMenu.ActiveUIEndScreen(stats);
        ChangeState();
    }
}
