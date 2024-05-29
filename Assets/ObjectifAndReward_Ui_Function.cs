using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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


    public void FinishRoomChallenge()
    {
        img_Objectif.sprite = spriteObjectif_tab[2];
        txtPro_Objectif.text = text_Objectif[2];
    }


    //TODO: Need change if statement for switch
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

        if (currentRoomManager.roomType == RoomType.Enemy)
        {
            img_Objectif.sprite = spriteObjectif_tab[0];
            txtPro_Objectif.text = text_Objectif[0];
        }
        else if (currentRoomManager.roomType == RoomType.Event)
        {
            img_Objectif.sprite = spriteObjectif_tab[1];
            txtPro_Objectif.text = text_Objectif[1];
        }
        else if (currentRoomManager.roomType == RoomType.Free)
        {
            img_Objectif.sprite = spriteObjectif_tab[2];
            txtPro_Objectif.text = text_Objectif[2];
        }
        objectifAnimator.SetTrigger("ActiveDisplay");
        rewardAnimator.SetTrigger("ActiveDisplay");
    }
}
