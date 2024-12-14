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

[System.Serializable]
public struct TestStruct
{
    public int intTest;
    public float floatTest;
    public string stringTest;
}


public struct EndInfoStats
{
    public float durationGame;
    public int nightValidate;
    public int enemyKill;
    public int roomCount;
    public int altarSuccessed;
    public int altarRepeated;
    public float maxCombo;

    public string[] stringInfoStats;

    public byte[] SaveData()
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter binary = new BinaryWriter(stream);
        binary.Write(durationGame);
        binary.Write(nightValidate);
        binary.Write(enemyKill);
        binary.Write(roomCount);
        binary.Write(altarSuccessed);
        binary.Write(altarRepeated);
        binary.Write(maxCombo);

        return stream.ToArray();
    }

    public string[] SaveDataString()
    {
        stringInfoStats = new string[6];   
        stringInfoStats[0] = "Duration = " + durationGame.ToString();
        stringInfoStats[1] = "Night Validate = " + nightValidate.ToString();
        stringInfoStats[2] = "Enemy Kill = " + enemyKill.ToString();
        stringInfoStats[3] = "Room Count = " + roomCount.ToString();
        stringInfoStats[4] = "Altar Succes = " + altarSuccessed.ToString();
        stringInfoStats[5] = "Max Combo = " + maxCombo.ToString();

        return stringInfoStats;
    }
    public void ReadData(BinaryReader binaryReader)
    {

        durationGame = binaryReader.ReadSingle();
        nightValidate = binaryReader.ReadInt32();
        enemyKill = binaryReader.ReadInt32();
        roomCount = binaryReader.ReadInt32();
        altarSuccessed = binaryReader.ReadInt32();
        altarRepeated = binaryReader.ReadInt32();
        if (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
            maxCombo = binaryReader.ReadSingle();


    }

    public void ReadData(StreamReader streamReader)
    {
        string[] lines = new string[5];
        int lineFillCount = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            if (streamReader.EndOfStream)
            {
                break;
            }
            lines[i] = streamReader.ReadLine();
            lineFillCount++;
        }
        int currentLine = 0;
        if (currentLine < lineFillCount)
        {
            durationGame = ReadFloat(lines[currentLine]);
            currentLine++;
        }
        if (currentLine < lineFillCount)
        {
            nightValidate = ReadInt(lines[currentLine]);
            currentLine++;
        }
        if (currentLine < lineFillCount)
        {
            enemyKill = ReadInt(lines[currentLine]);
            currentLine++;

        }
        if (currentLine < lineFillCount)
        {
            roomCount = ReadInt(lines[currentLine]);
            currentLine++;
        }
        if (currentLine < lineFillCount)
        {
            altarSuccessed = ReadInt(lines[currentLine]);
            currentLine++;
        }

        if (currentLine < lineFillCount)
        {
            maxCombo = ReadFloat(lines[currentLine]);
            currentLine++;
        }
    }

    private float ReadFloat(string line)
    {
        string[] splitArray = line.Split("=");
        if(splitArray.Length <2)
        {
            return 0;
        }
        string lineData = splitArray[1];
        return float.Parse(lineData);
    }
    private int ReadInt(string line)
    {
        string[] splitArray = line.Split("=");
        if (splitArray.Length < 2)
        {
            return 0;
        }
        string lineData = splitArray[1];
        return int.Parse(lineData);
    }


    public bool HasSuperiorValue(EndInfoStats statToCompare)
    {
        if (durationGame < statToCompare.durationGame) return true;
        if (nightValidate < statToCompare.nightValidate) return true;
        if (enemyKill < statToCompare.enemyKill) return true;
        if (roomCount < statToCompare.roomCount) return true;
        if (altarSuccessed < statToCompare.altarSuccessed) return true;
        if (altarRepeated < statToCompare.altarRepeated) return true;
        if (maxCombo < statToCompare.maxCombo) return true;

        return false;
    }
}

public class GameState : MonoBehaviour
{
    // Static Element
    public static Enemies.EnemyManager m_enemyManager;
    public static GameObject m_uiManager;
    public static GameObject s_playerGo;


    public static GameState instance;

    public TerrainGenerator terrainGenerator;
    public GameObject uiManagerGO;
    public GameObject playerGo;
    public GameObject fixeElementHolder;
    private static List<ObjectState> listObject = new List<ObjectState>(0);

    public static UIEndScreen endMenu;
    public UIEndScreen endmenu_Attribution;
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

    private PlayerInput m_playerInput;

    private Scene scene;
    private bool m_activeSceneEvent = false;

    public void Awake()
    {
        m_enemyManager = GetComponent<Enemies.EnemyManager>();
        m_uiManager = uiManagerGO;
        s_playerGo = playerGo;
        m_playerInput = playerGo.GetComponent<PlayerInput>();
        listObject.Clear();
        m_isPlaying = true;
        instance = this;
        GetGameManager();
        m_uiManager.GetComponent<UIDispatcher>().pauseMenu.m_textName.text = profileName;

    }


    public void GetGameManager()
    {
        if (m_gmComponent) return;
        GameObject gm = GameObject.Find("GameManager");
        if (gm != null)
        {
            if (m_activeDebug) Debug.Log("Found Game manager object ");
            m_gmComponent = gm.GetComponent<GameManager>();
            profileName = m_gmComponent.profileName;
            if (m_enemyManager)
            {
                Character.CharacterShoot charaShoot = playerGo.GetComponent<Character.CharacterShoot>();
                charaShoot.m_aimModeState = m_gmComponent.m_aimModeChoose;
                charaShoot.UpdateFeedbackAimLayout();
            }


        }
    }

    public void Start()
    {
        scene = (SceneManager.GetSceneByBuildIndex(5));
        m_isDeath = false;
        if (terrainGenerator) terrainGenerator.LaunchRoomGenerator();
        m_uiManager.GetComponent<UIDispatcher>().ActiveUIElement();
        endMenu = endmenu_Attribution;

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
        if (m_enemyManager) m_enemyManager.ChangePauseState(m_isPlaying);
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
        m_enemyManager.DestroyAllEnemy();
        EndInfoStats stats = m_enemyManager.FillEndStat();
        endMenu.ActiveUIEndScreen(stats);

        ChangeState();
    }

    public void HideGlobalUI()
    {
        fixeElementHolder.SetActive(false);
    }

    public bool IsGamepad()
    {
        return m_playerInput.currentControlScheme == "Gamepad";
    }
}
