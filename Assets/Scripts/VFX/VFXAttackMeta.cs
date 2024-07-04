using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        public void InitVFXObject(VfxAttackData vfxAttackData)
        {
            vfxData = vfxAttackData;
        }
    }


}