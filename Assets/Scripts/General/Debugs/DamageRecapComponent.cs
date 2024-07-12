using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.UI;
using UnityEngine.InputSystem;

namespace GuerhoubaGames.DebugTool
{
    public class DamageRecapComponent : MonoBehaviour
    {
        private List<string> m_damageString = new List<string>();
        public DamageRecapUI damageRecapUI;
        private HealthPlayerComponent healthPlayerComponent;

        void Start()
        {
            damageRecapUI = GameState.m_uiManager.GetComponent<UIDispatcher>().damageRecap;
            healthPlayerComponent = GetComponent<HealthPlayerComponent>();
            healthPlayerComponent.OnDamage += AddDamageToText;
            m_damageString.Clear();
        }
        
        public void ShowUi(InputAction.CallbackContext ctx)
        {
            if(ctx.started)
            {
                damageRecapUI.ShowUI();
            }
        }
    
        public void AddDamageToText(AttackDamageInfo attackDamageInfo)
        {
            m_damageString.Add("- " + attackDamageInfo.attackName + ":" + attackDamageInfo.damage.ToString());
            UpdateUI();
        }

        public void UpdateUI()
        {
            string finalText = "";
            for (int i = 0; i < m_damageString.Count; i++)
            {
                string line = m_damageString[i] + "\n";
                finalText += line;
            }
            damageRecapUI.UpdateText(finalText);
        }
    }
}