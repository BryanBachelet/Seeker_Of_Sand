using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GuerhoubaGames.Resources
{
    public class PullingMetaData : MonoBehaviour
    {
        [HideInInspector] public bool isActive = false;
        [HideInInspector] public int id = -1;
        [HideInInspector] public bool bIsExtraSpawn =false;

       public Action OnSpawn;

        public void ResetOnSpawn()
        {
            OnSpawn?.Invoke();
        }
    }
}