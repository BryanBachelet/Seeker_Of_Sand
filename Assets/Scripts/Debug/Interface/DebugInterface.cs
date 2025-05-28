using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugInterface : MonoBehaviour
{
    [Header("Player Interface")]
    [SerializeField] private GameObject m_player;
    [SerializeField] private Button m_playerButton;
    [SerializeField] private GameObject m_playerInterfaceCanvas;
    [SerializeField] private InputField m_moveInputField;
    [SerializeField] private InputField m_bulletNumberInputField;
    private Character.CharacterMouvement m_characterMouvement;
    private Character.CharacterShoot m_characterShoot;

    private void Awake()
    {
        m_characterMouvement = m_player.GetComponent<Character.CharacterMouvement>();
        m_characterShoot = m_player.GetComponent<Character.CharacterShoot>();

        m_playerButton.onClick.AddListener(ActivePlayerPanel);
        m_moveInputField.onValueChanged.AddListener(ModifyPlayerSpeed);
        m_bulletNumberInputField.onValueChanged.AddListener(ModifyProjectileNumber);
    }

    #region Player Function

    private void ActivePlayerPanel()
    {
        m_playerInterfaceCanvas.SetActive(!m_playerInterfaceCanvas.activeSelf);
    }

    private void ModifyPlayerSpeed(string text)
    {
        float value = float.Parse(text);
        if (value < 0)
        {
            Debug.LogError("Can't have negative value for move");
            return;
        }

        m_characterMouvement.runningSpeed = value;

    }

    private void ModifyProjectileNumber(string text)
    {
        int value = int.Parse(text);
        if (value < 0)
        {
            Debug.LogError("Can't have negative value for move");
            return;
        }

        m_characterShoot.projectileNumber = value;
    }

        #endregion

}

