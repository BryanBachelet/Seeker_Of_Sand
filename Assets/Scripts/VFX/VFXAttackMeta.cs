using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GuerhoubaGames.GameEnum;

namespace GuerhoubaGames.VFX
{

    public struct VfxAttackData
    {
        public float duration;
        public bool isDestroying ;
        public Transform parent;
        public Transform target;
        public AreaType areaType;
        public Vector3 scale;

    }


    public class VFXAttackMeta : MonoBehaviour
    {
        [HideInInspector] public VfxAttackData vfxData;
        [SerializeField] private VFXMovement_Forward moveBehavior;
        public System.Action OnStart;
        public void InitVFXObject(VfxAttackData vfxAttackData)
        {
            vfxData = vfxAttackData;
            if (OnStart != null) OnStart.Invoke();
            if (moveBehavior) moveBehavior.targetPosition = vfxData.target.position;
        }
    }


}