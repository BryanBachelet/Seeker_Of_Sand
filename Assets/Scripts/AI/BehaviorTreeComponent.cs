using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.AI
{
    public class BehaviorTreeComponent : MonoBehaviour
    {
        public BehaviorTree behaviorTree;
        public bool isActivate = false;
        #region Mono Functions
        public void Update()
        {
            if(isActivate) 
                behaviorTree.Update();
        }
        #endregion

        public void Init()
        {
            isActivate = true; 
            behaviorTree = behaviorTree.CloneTree();
            behaviorTree.BindTree(GetComponent<Enemies.NpcMetaInfos>());
        }
    }
}
