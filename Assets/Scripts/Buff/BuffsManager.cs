using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buff
{
    public class Buff
    {
        public float m_buffDuration;
        protected float m_buffTimer;

        public bool IsBuffFinish()
        {
            return (m_buffTimer >= m_buffDuration);
        }

        public void IncreaseTimer(float deltaTime)
        {
            m_buffTimer += deltaTime;
        }

    }

    public class BuffCharacter : Buff
    {
        CharacterStat buffStat;

        public BuffCharacter(CharacterData data, float duration)
        {
            buffStat = data.stats;
            m_buffDuration = duration;
        }

        public void GetBuff(ref CharacterStat profil)
        {
            profil += buffStat;
        }
    }



    public class BuffsManager : MonoBehaviour
    {
        private List<Buff> m_buffs = new List<Buff>(0);

        public void Update()
        {
            ManageBuffs();
        }

        public void AddBuff(Buff newBuff)
        {
            m_buffs.Add(newBuff);
        }
        public void ManageBuffs()
        {
            for (int i = 0; i < m_buffs.Count; i++)
            {
                if(m_buffs[i].IsBuffFinish())
                {
                    m_buffs.RemoveAt(i);
                    continue;
                }
                m_buffs[i].IncreaseTimer(Time.deltaTime);
            }
        }

        public void ApplyBuffCharacter(ref CharacterStat stats)
        {
            for (int i = 0; i < m_buffs.Count; i++)
            {
                if (m_buffs[i] is BuffCharacter)
                {
                    BuffCharacter buff = (BuffCharacter)m_buffs[i];
                    buff.GetBuff(ref stats);
                }
            }
        }
    }




}