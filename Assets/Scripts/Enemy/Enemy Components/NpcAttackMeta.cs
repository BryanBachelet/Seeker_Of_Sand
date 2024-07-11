using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;

namespace Enemies
{
    public struct AttackObjMetaData
    {
        public int damage;
        public float size;
        public AreaType typeArea;
        public Transform target;
        public bool isOneShoot;
    }


    public class NpcAttackMeta : MonoBehaviour
    {

        [HideInInspector] public AttackObjMetaData attackObjMetaData;
        public System.Action OnStart;

        public void InitAttackObject(AttackObjMetaData attackData)
        {
            attackObjMetaData = attackData;
            if (OnStart != null) OnStart.Invoke();
        }

    }
}