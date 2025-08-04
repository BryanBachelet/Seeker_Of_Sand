using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GuerhoubaGames.VFX
{
    public class VFXTargetFollow : MonoBehaviour
    {
        public bool constantFollow;
        public bool isFollowingParent;
        private bool m_canFollow;

        private Transform m_currentTarget;
        private Vector3 m_targetOffset;

        private VFXAttackMeta vfxMetaComponent;
        
        public void Awake()
        {
            vfxMetaComponent = GetComponent<VFXAttackMeta>();
            vfxMetaComponent.OnStart += StartFollow;
        }

        public void StartFollow ()
        {
            m_canFollow = true;
            if (isFollowingParent) m_currentTarget = vfxMetaComponent.vfxData.parent;
            else m_currentTarget = vfxMetaComponent.vfxData.target;
            m_targetOffset = m_currentTarget.position - transform.position;
        }

        public void Update()
        {
            if (!m_canFollow) return;

            if(constantFollow)
            {
                transform.position = m_currentTarget.position + m_targetOffset;
            }


        }
    }

}