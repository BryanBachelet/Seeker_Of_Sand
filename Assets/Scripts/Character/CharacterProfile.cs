using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public interface CharacterComponent
{
    public void InitComponentStat(CharacterStat stat);
    public void UpdateComponentStat(CharacterStat stat);
}

public class CharacterProfile : MonoBehaviour
{
    private HealthSystem m_healthSystem;
    public CharacterStatPreset characterStatPreset; 
    [HideInInspector] 
    public CharacterStat stats;
    
    [HideInInspector] public CharacterStat m_baseStat;
    public static CharacterProfile instance;
    private CharacterComponent[] m_characterComponent = new CharacterComponent[0];
    private Buff.BuffsManager m_buffManager;

    [SerializeField] private GameObject m_pauseMenuObject;
    [SerializeField] private GameObject m_CustomPassSeeThrough;


    #region Static Functions

    public static CharacterStat GetCharacterStat()
    {
        return instance.stats;
    }
         
        
    #endregion


    private void Awake()
    {
        instance = this;
        if(characterStatPreset)
        {
            stats = characterStatPreset.preset;
        }
      
    }
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
        // ApplyStat(stats);
    }

    public void ApplyStat(CharacterStat newStat)
    {
        stats = newStat;
        for (int i = 0; i < m_characterComponent.Length; i++)
        {
            m_characterComponent[i].InitComponentStat(stats);
        }
    }
    public void AddStat(CharacterStat newStat)
    {
        stats = stats +newStat;
    }

    public void RemoveStats(CharacterStat newStat)
    {
        stats  -= newStat;
    }

    public void UpdateStats()
    {
        for (int i = 0; i < m_characterComponent.Length; i++)
        {
            m_characterComponent[i].UpdateComponentStat(stats);
        }
    }

    public void OpenPauseMenuInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            m_pauseMenuObject.GetComponent<PauseMenu>().CallPauseMenu();
            m_CustomPassSeeThrough.gameObject.SetActive(!m_CustomPassSeeThrough.gameObject.activeSelf);
        }
    }

}
