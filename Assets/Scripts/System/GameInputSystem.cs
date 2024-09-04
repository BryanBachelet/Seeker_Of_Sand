using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GuerhoubaGames.Input
{

    public enum GamepadScheme
    {
        BaseLayout = 0,
        DivideSchemeLayout = 1,
    }

    public class GameInputSystem : MonoBehaviour
    {

        public static GameInputSystem instance;
        [SerializeField] public PlayerInput playerInputComponent;
        [SerializeField] private bool m_isDebugActive;

        [Header("Gamepad Scheme Parameter")]
        public GamepadScheme gamepadScheme;
        public DivideSchemeManager divideSchemeManager;

        private bool m_isKeyboardScheme;

        #region Unity Functions
        public void Awake()
        {
            instance = this;
        }

        public void Start()
        {
            // Have to active the script for divide layout
            if (gamepadScheme == GamepadScheme.DivideSchemeLayout) divideSchemeManager.enabled = true;
            else divideSchemeManager.enabled = false;

            DetectionDevicesUpdate();
        }

        void Update()
        {
            DetectionDevicesUpdate();
        }
        #endregion

        public void DetectionDevicesUpdate()
        {

            if (Keyboard.current.anyKey.isPressed)
            {
                SetKeyboardScheme();
                return;
            }
            for (int i = 0; i < playerInputComponent.devices.Count; i++)
            {


                if (playerInputComponent.devices[i].name == "Mouse" || playerInputComponent.devices[i].name == "Keyboard")
                {
                    if (Mathf.Abs(Mouse.current.delta.magnitude) < 0.5f) continue;

                    if (m_isDebugActive) Debug.Log(playerInputComponent.devices[i].name + " has been update :" + playerInputComponent.devices[i].IsActuated());
                    SetKeyboardScheme();
                    return;
                }
                else
                {



                    if (!playerInputComponent.devices[i].IsActuated() && !Gamepad.current.IsPressed()) continue;

                    if (m_isDebugActive) Debug.Log(playerInputComponent.devices[i].name + " has been update :" + playerInputComponent.devices[i].IsActuated());
                    SetGamepadScheme();
                    return;
                }
            }

        }

        #region Control Scheme Functions

        private void SetKeyboardScheme()
        {
            if (m_isKeyboardScheme) return;
            playerInputComponent.SwitchCurrentActionMap("Player");
            m_isKeyboardScheme = true;
            InputDevice[] devices = new InputDevice[3];
            devices[0] = Keyboard.current;
            devices[1] = Mouse.current;
            devices[2] = Gamepad.current;
            playerInputComponent.SwitchCurrentControlScheme("Keyboard&Mouse", devices);

        }

        private void SetGamepadScheme()
        {

            if (!m_isKeyboardScheme) return;
            SetGamepadActionMap();

            m_isKeyboardScheme = false;
            InputDevice[] devices = new InputDevice[3];
            devices[0] = Keyboard.current;
            devices[1] = Mouse.current;
            devices[2] = Gamepad.current;
            playerInputComponent.SwitchCurrentControlScheme("Gamepad", devices);

        }

        private void SetGamepadActionMap()
        {
            switch (gamepadScheme)
            {
                case GamepadScheme.BaseLayout:
                    playerInputComponent.SwitchCurrentActionMap("Player");
                    break;
                case GamepadScheme.DivideSchemeLayout:
                    divideSchemeManager.ChangeToCombatActionMap();
                    break;
                default:
                    playerInputComponent.SwitchCurrentActionMap("Player");
                    break;
            }
        }


        #endregion




    }
}
