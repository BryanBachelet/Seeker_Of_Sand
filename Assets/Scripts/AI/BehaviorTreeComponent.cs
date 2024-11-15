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
        public bool isFirstSpawn = true;

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

            if (isFirstSpawn)
            {
                behaviorTree = behaviorTree.CloneTree();
                behaviorTree.BindTree(GetComponent<Enemies.NpcMetaInfos>());
                isFirstSpawn = false;
            }
            isActivate = true;
        }
    }
}
