using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GuerhoubaGames.GameEnum;
using GuerhoubaGames.Resources;
using GuerhoubaGames.Enemies;

public class ObjectifAndReward_Ui_Function : MonoBehaviour
{
    [SerializeField] private Animator objectifAnimator;
    [SerializeField] private TMP_Text txtPro_Objectif;
    [SerializeField] private TMP_Text txtPro_Objectif_Count;
    [SerializeField] private Image img_Objectif;

    [SerializeField] private Animator optional_objectifAnimator;
    [SerializeField] private TMP_Text optional_txtPro_Objectif;
    [SerializeField] private TMP_Text optional_txtPro_Objectif_Count;
    [SerializeField] private Image optional_img_Objectif;


    [SerializeField] private Animator rewardAnimator;
    [SerializeField] private Image img_Reward;

    [HideInInspector] private GameResources m_gameRessources;

    public RoomManager currentRoomManager;

    [HideInInspector] private bool enemyRoom = false;
    [HideInInspector] private bool eventRoom = false;

    [HideInInspector] private float currentProgress = 1;
    [HideInInspector] private float delayProgress = 1;
    static private float progressStatic;

    public Animator objectifAnimatorMajor;
    public Image fill_Progress;
    public Image fill_ProgressDelay;

    [HideInInspector] private bool stopDisplay = false;

    private static float timeLastUpdate;
    [HideInInspector] private float timeDelay = 500;

    [SerializeField] private EnemyManager enemyManager;
    // Start is called before the first frame update
    void Start()
    {
        enemyManager = GameObject.Find("General_Manager").GetComponent<EnemyManager>();
        m_gameRessources = GameObject.Find("General_Manager").GetComponent<GameResources>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyRoom || eventRoom)
        {
            currentProgress = progressStatic;

            delayProgress = Mathf.Lerp(delayProgress, currentProgress, Time.time / (timeLastUpdate + timeDelay));
            //Debug.Log("current Progress : " + currentProgress + " [Delay progress : " + delayProgress + "|||| Fill : " + (1 - currentProgress));
            fill_Progress.fillAmount = 1 - currentProgress;
            fill_ProgressDelay.fillAmount = 1 - delayProgress;
            if (stopDisplay)
            {
                DisactiveDisplayProgress();
                Debug.Log("Stop display");
                stopDisplay = false;
                eventRoom = false;
                enemyRoom = false;
            }

        }
    }

    public static void UpdateProgress(float progress)
    {
        progressStatic = progress;
        timeLastUpdate = Time.time;
    }
    public void UpdateObjectifAndReward()
    {
        objectifAnimator.ResetTrigger("ActiveDisplay");
        optional_objectifAnimator.ResetTrigger("ActiveDisplay");
        rewardAnimator.ResetTrigger("ActiveDisplay");
        if (currentRoomManager.rewardType == RewardType.UPGRADE)
        {
            img_Reward.sprite = m_gameRessources.spriteReward_tab[0];
        }
        else if (currentRoomManager.rewardType == RewardType.HEAL)
        {
            img_Reward.sprite = m_gameRessources.spriteReward_tab[1];
        }
        else if (currentRoomManager.rewardType == RewardType.SPELL)
        {
            img_Reward.sprite = m_gameRessources.spriteReward_tab[2];
        }
        else if (currentRoomManager.rewardType == RewardType.ARTEFACT)
        {
            img_Reward.sprite = m_gameRessources.spriteReward_tab[3];
        }

        if (currentRoomManager.currentRoomType == RoomType.Enemy)
        {
            currentRoomManager.currentRoomType = RoomType.Event;
        }
            if (currentRoomManager.currentRoomType == RoomType.Enemy)
        {
            enemyManager.ActiveSpawnPhase(true, EnemySpawnCause.EVENT);
            img_Objectif.sprite = m_gameRessources.spriteObjectif_tab[0];
            optional_img_Objectif.sprite = m_gameRessources.optional_spriteObjectif_tab[0];
            txtPro_Objectif.text = m_gameRessources.text_Objectif[0];
            optional_txtPro_Objectif.text = m_gameRessources.optional_text_Objectif[0];
            enemyRoom = true;
            eventRoom = false;
            currentProgress = 0;
            delayProgress = 0;
            fill_Progress.fillAmount = 1;
            fill_ProgressDelay.fillAmount = 1;
            ActiveDisplayProgress();
        }
        else if (currentRoomManager.currentRoomType == RoomType.Event)
        {
            enemyManager.ActiveSpawnPhase(true, EnemySpawnCause.EVENT);
            img_Objectif.sprite = m_gameRessources.spriteObjectif_tab[1];
            optional_img_Objectif.sprite = m_gameRessources.optional_spriteObjectif_tab[1];
            txtPro_Objectif.text = m_gameRessources.text_Objectif[1];
            optional_txtPro_Objectif.text = m_gameRessources.optional_text_Objectif[1];
            eventRoom = true;
            enemyRoom = false;
            currentProgress = 0;
            delayProgress = 0;
            fill_Progress.fillAmount = 1;
            fill_ProgressDelay.fillAmount = 1;
            ActiveDisplayProgress();
        }
        else if (currentRoomManager.currentRoomType == RoomType.Free)
        {
            img_Objectif.sprite = m_gameRessources.spriteObjectif_tab[2];
            optional_img_Objectif.sprite = m_gameRessources.optional_spriteObjectif_tab[2];
            txtPro_Objectif.text = m_gameRessources.text_Objectif[2];
            optional_txtPro_Objectif.text = m_gameRessources.optional_text_Objectif[2];
            eventRoom = false;
            enemyRoom = false;
            DisactiveDisplayProgress();
        }
        objectifAnimator.SetTrigger("ActiveDisplay");
        optional_objectifAnimator.SetTrigger("ActiveDisplay");
        rewardAnimator.SetTrigger("ActiveDisplay");
    }

    public void ActiveDisplayProgress()
    {
        objectifAnimatorMajor.SetBool("MajorDisplay", true);
    }

    public void DisactiveDisplayProgress()
    {

        objectifAnimatorMajor.SetBool("MajorDisplay", false);
        enemyManager.ActiveSpawnPhase(false, EnemySpawnCause.EVENT);
        objectifAnimator.ResetTrigger("ActiveDisplay");
        optional_objectifAnimator.ResetTrigger("ActiveDisplay");
        rewardAnimator.ResetTrigger("ActiveDisplay");
        img_Objectif.sprite = m_gameRessources.spriteObjectif_tab[2];
        optional_img_Objectif.sprite = m_gameRessources.optional_spriteObjectif_tab[2];
        txtPro_Objectif.text = m_gameRessources.text_Objectif[2];
        optional_txtPro_Objectif.text = m_gameRessources.optional_text_Objectif[2];
        objectifAnimator.SetTrigger("ActiveDisplay");
        optional_objectifAnimator.SetTrigger("ActiveDisplay");
        rewardAnimator.SetTrigger("ActiveDisplay");


    }

    public void StopEventDisplay()
    {
        stopDisplay = true;


    }
}
