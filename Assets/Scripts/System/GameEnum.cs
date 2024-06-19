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
        EARTH =3
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
        NOTHING = 4,
    }
}
