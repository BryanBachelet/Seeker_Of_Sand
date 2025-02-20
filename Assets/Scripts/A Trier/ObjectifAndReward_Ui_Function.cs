using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GuerhoubaGames.GameEnum;

public class ObjectifAndReward_Ui_Function : MonoBehaviour
{
    public Animator objectifAnimator;
    public TMP_Text txtPro_Objectif;
    public Image img_Objectif;

    public Sprite[] spriteObjectif_tab;
    public string[] text_Objectif;

    public Animator rewardAnimator;
    public TMP_Text txtPro_Reward;
    public Image img_Reward;

    public Sprite[] spriteReward_tab;

    public RoomManager currentRoomManager;

    public bool enemyRoom = false;
    public bool eventRoom = false;

    public float currentProgress = 1;
    public float delayProgress = 1;
    static private float progressStatic;

    public Animator objectifAnimatorMajor;
    public Image fill_Progress;
    public Image fill_ProgressDelay;

    public bool stopDisplay = false;

    private static float timeLastUpdate;
    public float timeDelay = 1;

    [SerializeField] private Enemies.EnemyManager enemyManager;
    // Start is called before the first frame update
    void Start()
    {

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
                //objectifAnimator.ResetTrigger("ActiveDisplay");
                //rewardAnimator.ResetTrigger("ActiveDisplay");
                //img_Objectif.sprite = spriteObjectif_tab[2];
                //txtPro_Objectif.text = text_Objectif[2];
                //eventRoom = false;
                //enemyRoom = false;
                //objectifAnimator.SetTrigger("ActiveDisplay");
                //rewardAnimator.SetTrigger("ActiveDisplay");
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
        rewardAnimator.ResetTrigger("ActiveDisplay");
        if (currentRoomManager.rewardType == RewardType.UPGRADE)
        {
            img_Reward.sprite = spriteReward_tab[0];
        }
        else if (currentRoomManager.rewardType == RewardType.HEAL)
        {
            img_Reward.sprite = spriteReward_tab[1];
        }
        else if (currentRoomManager.rewardType == RewardType.SPELL)
        {
            img_Reward.sprite = spriteReward_tab[2];
        }
        else if (currentRoomManager.rewardType == RewardType.ARTEFACT)
        {
            img_Reward.sprite = spriteReward_tab[3];
        }

        if (currentRoomManager.currentRoomType == RoomType.Enemy)
        {
            currentRoomManager.currentRoomType = RoomType.Event;
        }
            if (currentRoomManager.currentRoomType == RoomType.Enemy)
        {
            enemyManager.ActiveSpawnPhase(true, Enemies.EnemySpawnCause.EVENT);
            img_Objectif.sprite = spriteObjectif_tab[0];
            txtPro_Objectif.text = text_Objectif[0];
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
            enemyManager.ActiveSpawnPhase(true, Enemies.EnemySpawnCause.EVENT);
            img_Objectif.sprite = spriteObjectif_tab[1];
            txtPro_Objectif.text = text_Objectif[1];
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
            img_Objectif.sprite = spriteObjectif_tab[2];
            txtPro_Objectif.text = text_Objectif[2];
            eventRoom = false;
            enemyRoom = false;
            DisactiveDisplayProgress();
        }
        objectifAnimator.SetTrigger("ActiveDisplay");
        rewardAnimator.SetTrigger("ActiveDisplay");
    }

    public void ActiveDisplayProgress()
    {
        objectifAnimatorMajor.SetBool("MajorDisplay", true);
    }

    public void DisactiveDisplayProgress()
    {

        objectifAnimatorMajor.SetBool("MajorDisplay", false);
        enemyManager.ActiveSpawnPhase(false, Enemies.EnemySpawnCause.EVENT);
        objectifAnimator.ResetTrigger("ActiveDisplay");
        rewardAnimator.ResetTrigger("ActiveDisplay");
        img_Objectif.sprite = spriteObjectif_tab[2];
        txtPro_Objectif.text = text_Objectif[2];
        objectifAnimator.SetTrigger("ActiveDisplay");
        rewardAnimator.SetTrigger("ActiveDisplay");


    }

    public void StopEventDisplay()
    {
        stopDisplay = true;


    }
}
