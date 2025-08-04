
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterPotion : MonoBehaviour
{
    [SerializeField] private int m_potionCharge = 3;
    [SerializeField] private int m_currentCharge = 3;
    [SerializeField] private float m_interactionDuration = 1.5f;
   // [SerializeField] private float m_percentGain = .70f;

    [Header("Debug Parameters")]
    public bool m_activeDebugPotionMessage = false;

    private bool m_IsDrinkIsActive = false;
    private float m_interactionTimer = 0;

    private HealthPlayerComponent m_playerHealth;

    // Int, is the charge number
    public Action<int> OnPotionDrink;
    public Action<int> OnPotionStartDrink;
    public Action<int> OnPotionCancel;
    public Action<int> OnPotionRecharge;

    // Start is called before the first frame update
    void Start()
    {
        m_playerHealth = GetComponent<HealthPlayerComponent>(); 
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePotionDrink();
    }

    #region Inputs Functions
    
    public void InputPotionDrink(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            ActivePotionDrink();
        }
        if (ctx.canceled)
        {
            CancelDrinkPotion(new AttackDamageInfo());
        }
    }

    #endregion

    private void UpdatePotionDrink()
    {
        if (!m_IsDrinkIsActive) return;

        m_interactionTimer += Time.deltaTime;

        if(m_interactionTimer > m_interactionDuration)
        {
            ValidatePotionDrink();
            m_interactionTimer = 0.0f;
            m_IsDrinkIsActive = false;
        }
    }

    private void ValidatePotionDrink()
    {
        if (m_activeDebugPotionMessage)
            Debug.Log("Potion :  Potion has been drink, charge remaining  : "+   m_currentCharge  );
        m_currentCharge--;
        m_playerHealth.RestoreQuarter(true);
        OnPotionDrink?.Invoke(m_currentCharge);
    }

    public void ActivePotionDrink()
    {
        if (m_potionCharge <= 0)
            return;
        if (m_activeDebugPotionMessage)
            Debug.Log("Potion :  Start to drink");

        m_IsDrinkIsActive = true;  
        m_playerHealth.OnDamage += CancelDrinkPotion;
        OnPotionStartDrink?.Invoke(m_currentCharge);
    }

    private void CancelDrinkPotion(AttackDamageInfo info)
    {
        if (!m_IsDrinkIsActive) return;

        m_IsDrinkIsActive= false;

        if (m_activeDebugPotionMessage && info.damage != 0)
            Debug.Log("Potion : Potion drink cancel by hit");

        if (m_activeDebugPotionMessage && info.damage == 0)
            Debug.Log("Potion : Potion drink cancel by stopping the input");

        OnPotionCancel?.Invoke(m_currentCharge);
    }

    public void RechargePotion(int quantity, bool isFullRecharge = false)
    {
        if(isFullRecharge)
        {
            m_currentCharge = m_potionCharge;
        }
        else
        {
            m_currentCharge++;
        }
        
        OnPotionRecharge?.Invoke(m_currentCharge);
    }

}
