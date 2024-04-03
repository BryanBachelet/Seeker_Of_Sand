using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GuerhoubaGames.AI
{
    
    public class BehaviorTreeComponent : MonoBehaviour
    {
        public BehaviorTree tree;

        public void Start()
        {
            tree = tree.Clone();
            tree.Bind(GetComponent<NPCAgent>());
        }

        public void Update()
        {
            tree.Update();
        }

    
    }

}