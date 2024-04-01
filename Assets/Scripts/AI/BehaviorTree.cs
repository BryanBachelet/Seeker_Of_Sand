using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GuerhoubaGames.AI
{
    
    public class BehaviorTree : MonoBehaviour
    {

        [Header("BehaviorTree Parameters")]
        public int updateTickFrame = 3;
        public AINode root = null;

        private int m_tickFrameCount = 0;
        private bool m_isBlockState = false;


        public void Update()
        {
            
        }

        public void UpdateBehaviorTree()
        {

        }

        public void ActiveBlockState()
        {
            m_isBlockState = true;
        }

        public void DeactiveBlockState()
        {
            m_isBlockState = false;
        }
    }

}