using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MalusDisplay_Manager : MonoBehaviour
{
    [SerializeField] private MalusEffect_Display[] m_MalusDisplay_Script = new MalusEffect_Display[10];
    [SerializeField] private bool[] m_MalusDisplay_State = new bool[10];
    // Start is called before the first frame update

    private void GetActiveDisplayState()
    {
        for (int i = 0; i < m_MalusDisplay_Script.Length; i++)
        {
            m_MalusDisplay_State[i] = m_MalusDisplay_Script[i].m_malusActive;
        }
    }

    public void SendNewMalus(float duration, int stack, Sprite spriteToUse)
    {
        GetActiveDisplayState();
        for(int i = 0; i < m_MalusDisplay_State.Length; i++)
        {
            if (m_MalusDisplay_State[i] == false)
            {
                m_MalusDisplay_Script[i].ActivateMalus(duration, stack, spriteToUse);
            }
        }
    }
}
