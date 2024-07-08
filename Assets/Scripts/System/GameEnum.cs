using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.GameEnum
{
    public enum CharacterObjectType
    {
        SPELL = 0,
        FRAGMENT = 1,
    }

    public enum GameElement
    {
        WATER = 0,
        AIR = 1,
        FIRE = 2,
        EARTH = 3
    }

    public enum RoomType
    {
        Event = 0,
        Enemy = 1,
        Free = 2,
        Merchant = 3,
    }

    public enum RewardType
    {

        UPGRADE = 0,
        SPELL = 1,
        ARTEFACT = 2,
        HEAL = 3,
        MERCHANT = 4,
    }

    public enum AreaType
    {
        CIRCLE = 0,
        RECT = 1,
        TRIANGLE = 2,
        CUSTOM = 3,
    }



    public enum FeedbackType
    {
        VISUAL = 0,
        SOUND = 1,
    }

    public enum AttackType
    {
        COLLIDER_OBJ = 0,
        PROJECTILE_OBJ = 1,
    }

    public enum RangeAttackType
    {
        PROJECTILE = 0,
        AREA = 1,
    }

    public enum AttackLaunchMoment
    {
        START_CONTACT = 0,
        UPDATE_CONTACT = 1,
        AFTER_MVT =2,
    }


    public enum AttackPhase
    {
        PREP = 0,
        CONTACT = 1,
        RECOVERY = 2,
        NONE = 3,
    }




}
