using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.AI
{
    struct EventBehaviorTree
    {
        Action events;
    }

    public class BehaviorTreeComponent : MonoBehaviour
    {
        public BehaviorTree behaviorTree;
        public bool isActivate = false;

        #region Mono Functions
        public void Update()
        {
            if (isActivate)
                behaviorTree.Update();

        }
        #endregion

        public void Init()
        {
            if (isActivate) return;

            behaviorTree = behaviorTree.CloneTree();
            behaviorTree.BindTree(GetComponent<Enemies.NpcMetaInfos>());
            isActivate = true;
        }
    }
}
