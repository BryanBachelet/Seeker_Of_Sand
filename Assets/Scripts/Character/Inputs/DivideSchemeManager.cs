using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GuerhoubaGames.Input
{
    public class DivideSchemeManager : MonoBehaviour
    {

        public bool isCombatActionMap;
        public float timeBeforeChangeMap = 0.5f;
        private float m_timerBeforeChangeMap = 0.0f;
        public bool isAbleToChangeMap = false;
        private PlayerInput m_playerInputComponent;
        
        public void Awake()
        {
            enabled = false;
        }

        public void Start()
        {
            isCombatActionMap = true;
            m_playerInputComponent = GetComponent<PlayerInput>();
        }

        public void Update()
        {
            if (m_timerBeforeChangeMap > timeBeforeChangeMap)
            {
                isAbleToChangeMap = true;

            }
            else
            {
                isAbleToChangeMap = false;
                m_timerBeforeChangeMap += Time.deltaTime;
                        
            }
        }

        public void ChangeToMoveActionMap()
        {
            Debug.Log("Try to change move map ");

            if (!isAbleToChangeMap)return ;
            m_timerBeforeChangeMap = 0.0f;
            isAbleToChangeMap = false;
            isCombatActionMap = false;
            m_playerInputComponent.SwitchCurrentActionMap("Div_MouvementLayout");
            m_timerBeforeChangeMap = 0.0f;
            Debug.Log("Succes to change move map ");
        }

        public void ChangeToCombatActionMap()
        {
            Debug.Log("Try to change combat map ");
            if (!isAbleToChangeMap) return;
            m_timerBeforeChangeMap = 0.0f;
            isAbleToChangeMap = false;
            isCombatActionMap = true;
            m_playerInputComponent.SwitchCurrentActionMap("Div_CombatLayout");
            m_timerBeforeChangeMap = 0.0f;
            Debug.Log("Succes to change combat map ");

        }

    }
}

