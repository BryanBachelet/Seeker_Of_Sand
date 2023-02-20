using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buff
{

    public class BuffsManager
    {
        private List<Buff> m_buffs;
    }
    
    public class Buff
    {
        public float m_buffTime;
        private float m_buffTimer;
    }


}