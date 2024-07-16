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

    public enum StatType
    {
        Damage = 1001,
        Projectile = 1002,
        ShootAngle = 1003,
        ShootNumber = 1004,
        Piercing = 1005,
        Bounce = 1006,
        GainPerStack =1007,
        AngleTrajectory =1008,


        Speed = 2001,
        Range = 2002,
        CompleteShotTime = 2003,
        StackDuration = 2004,
        SpellCanalisation = 2005,
        TrajectoryTimer =2006,
        Size = 2007,
        SizeMuplitiplicator = 2008,
        TravelTime = 2009,
        LifeTime = 2010,
        HitTick = 2011,

    }

    public enum ValueType
    {
        BOOL = 0,
        INT = 1000,
        FLOAT = 2000,
        STRING = 3000,
    }


    // Tag Spell
    public enum BuffType
    {
        NONE,
        DAMAGE_SPELL,
        BUFF_SPELL,
    }

    public enum SpellObjectType
    {
        NONE,
        AURA = 0,
        PROJECTILE = 1,
        AREA = 2,
        INVOCATION = 3,
    }

    public enum SpellProjectileTrajectory
    {
        NONE,
        LINE = 0,
        CURVE = 1,
    }

}
