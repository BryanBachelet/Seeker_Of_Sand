using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Affliction_UI_Manager : MonoBehaviour
{
    [SerializeField] private Affliction_UI_Display[] m_afflictionDisplayTarget = new Affliction_UI_Display[10];
    [SerializeField] private Affliction_UI_Display[] m_afflictionDisplayObjectif = new Affliction_UI_Display[10];
    [SerializeField] private Sprite[] m_spriteAffliction;



    #region Unity functions

    public void Start()
    {
        for (int i = 0; i < m_afflictionDisplayTarget.Length; i++)
        {
            m_afflictionDisplayTarget[i].DeactivateAfflictionDisplayUI();
            m_afflictionDisplayObjectif[i].DeactivateAfflictionDisplayUI();
        }
    }

    #endregion

    public void CleanTargetAfflictionDisplay()
    {
        for (int i = 0; i < m_afflictionDisplayTarget.Length; i++)
        {
            m_afflictionDisplayTarget[i].DeactivateAfflictionDisplayUI();
        }
    }

    public void DisplayAffliction(Affliction affliction,bool isGoalTarget)
    {
      if(isGoalTarget)  _DisplayAffliction(affliction, m_afflictionDisplayObjectif);
      if(!isGoalTarget)  _DisplayAffliction(affliction, m_afflictionDisplayTarget);
    }

    private void _DisplayAffliction(Affliction affliction, Affliction_UI_Display[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].isAfflictionUIActive == false || (array[i].afflictionType == affliction.type))
            {
                Sprite sprite = m_spriteAffliction[(int)affliction.type - 1];
                array[i].ActivateMalus(affliction.duration, affliction.stackCount, affliction.type, sprite);
                break;
            }
        }
    }
}
