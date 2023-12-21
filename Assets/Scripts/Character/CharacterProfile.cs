using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public interface CharacterComponent
{
   public void InitComponentStat(CharacterStat stat);
}

public class CharacterProfile : MonoBehaviour
{
    private HealthSystem m_healthSystem;
    [SerializeField] public CharacterStat stats;
    [HideInInspector] public CharacterStat m_baseStat;
    private CharacterComponent[] m_characterComponent = new CharacterComponent[0];
    private Buff.BuffsManager m_buffManager;

    [SerializeField] private GameObject m_pauseMenuObject;

    private void Start()
    {
        m_characterComponent = GetComponents<CharacterComponent>();
        m_baseStat = stats;
        m_buffManager = GetComponent<Buff.BuffsManager>();
        ApplyStat(stats);
    }

    public void Update()
    {
     
        m_buffManager.ApplyBuffCharacter(ref stats);
        m_buffManager.ManageBuffs(ref stats);
        ApplyStat(stats);
    }

    public void ApplyStat(CharacterStat newStat)
    {
        stats = newStat;
        for (int i = 0; i < m_characterComponent.Length; i++)
        {
            m_characterComponent[i].InitComponentStat(stats);
        }
    }


    public void OpenPauseMenuInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) m_pauseMenuObject.GetComponent<PauseMenu>().CallPauseMenu();
       
    }


}
