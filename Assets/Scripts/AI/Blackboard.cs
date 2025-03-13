using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GuerhoubaGames.AI
{
    [System.Serializable]
    public class Blackboard 
    {
        public Vector3 moveToPosition;
        public GameObject moveToObject;
        public bool IsSpecialCapacityCall;
        public int indexSpecialCapacityCall;
        public Action event1 ;
    }
}
