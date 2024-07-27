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
        
        ShootNumber = 1004,
        Piercing = 1005,
        Bounce = 1006,
        GainPerStack =1007,
        AngleTrajectory =1008,
        HitNumber =1009,
        DamageAdditionel= 1010,
        SizeExplosion= 1011,
        BounceNumber= 1012,
        MaxSummon =1013,
        SummonSimultanely = 1014,
        SpellCount =1015,
       
        
        
        LifeTime = 2001,
        Range = 2002,
        CompleteShotTime = 2003,
        StackDuration = 2004,
        SpellCanalisation = 2005,
        TrajectoryTimer =2006,
        Size = 2007,
        SizeMuplitiplicator = 2008,
        TravelTime = 2009,
        HitFrequency =2010,
        SpeedReduce = 2011,
        ScaleRate = 2012,
        TimeDelay = 2013,
        DistanceDash =2014,
        Invunerability =2016,
        MouvementTravelTime =2017,
        Proximity =2018,
        SpeedFollow =2019,
        SpeedReturn=2020,
        TimeBetweenShot =2021,
        ShootAngle = 2022,
        OffsetDistance = 2023,
        SpellFrequency = 2024,
    }

    

    public enum ValueType
    {
        BOOL = 0,
        INT = 1000,
        FLOAT = 2000,
        STRING = 3000,
    }


    // Tag Spell
    // For each tag the value None need to be in higher case : NONE
    // and need to be add index zero 

    public enum SpellTagOrder
    {
        GameElement = 0,
        BuffType = 1,
        SpellNature = 2,
        SpellNature1 = 3,
        SpellProjectileTrajectory = 4,
        CanalisationType = 5,
        SpellMovementBehavior = 6,
        DamageTrigger = 7,
        SpellParticualarity = 8,
        SpellParticualarity1 = 9,
        SpellParticualarity2 = 10,
        MouvementBehavior = 11,
        UpgradeSensitivity = 12,
    }

    public enum GameElement
    {
        NONE = 0,
        WATER = 1,
        AIR = 2,
        FIRE = 3,
        EARTH = 4,
    }

    public enum BuffType
    {
        NONE =0,
        DAMAGE_SPELL =1,
        BUFF_SPELL =2,
    }

    public enum SpellNature
    {
        NONE = 0,
        AURA = 1,
        PROJECTILE = 2,
        AREA = 3,
        SUMMON = 4,
        DOT = 5,
       
    }

    public enum SpellProjectileTrajectory
    {
        NONE = 0,
        LINE = 1,
        CURVE = 2,
        SPECIAL =3,
        
    }

    public enum CanalisationType
    {
        NONE =0,
        LIGHT_CANALISATION = 1,
        HEAVY_CANALISATION = 2,
    }


    public enum SpellMovementBehavior
    {
        NONE=0,
        FollowPlayer=1,
        OnSelf=2,
        Fix = 3,
        Return=4,
    }

    public enum DamageTrigger
    {
        NONE = 0,
        OnHit = 1,
    }

    public enum UpgradeSensitivity
    {
        NONE = 0,
        HighScale =1 ,
        LowScale =2,
    }

    public enum SpellParticualarity
    {
        NONE = 0,
        Delayed = 1,
        Explosion = 2,
        Piercing = 3,
        Bouncing = 4,
        Physics = 5,
    }

    public enum MouvementBehavior
    {
        NONE =0,
        Dash =1,
        Slide =2,
        Jump =3,
        InAir =4,
    }


}
