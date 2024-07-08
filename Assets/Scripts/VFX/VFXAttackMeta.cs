using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GuerhoubaGames.VFX
{

    public struct VfxAttackData
    {
        public float attackRange;
        public float duration;
        public bool isDestroying ;
    }


    public class VFXAttackMeta : MonoBehaviour
    {
        [HideInInspector] public VfxAttackData vfxData;
        public System.Action OnStart;
        public void InitVFXObject(VfxAttackData vfxAttackData)
        {
            vfxData = vfxAttackData;
            if (OnStart != null) OnStart.Invoke();
        }
    }


}