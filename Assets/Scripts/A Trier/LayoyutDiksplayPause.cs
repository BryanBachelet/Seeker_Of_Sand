using GuerhoubaGames.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LayoyutDiksplayPause : MonoBehaviour
{
    public Sprite[] m_gamepadLayout = new Sprite[3];
    public Sprite[] m_keyboardLayout = new Sprite[3];
    [SerializeField] private Image m_gamepadLayoutDisplay;
    [SerializeField] private Image m_keyboardLayoutDisplay;
    [SerializeField] private TMPro.TMP_Text m_gamepadText;
    [SerializeField] private TMPro.TMP_Text m_keyboardText;
    private CharacterShoot m_characterShoot;
    // Start is called before the first frame update
    void Start()
    {
        m_characterShoot = GameObject.Find("Player").GetComponent<CharacterShoot>();
    }

    // Update is called once per frame
    void Update()
    {
        m_gamepadLayoutDisplay.sprite = m_gamepadLayout[(int)m_characterShoot.m_aimModeState];
        m_gamepadText.text = "(Gamepad) " + m_characterShoot.m_aimModeState.ToString();
        m_keyboardLayoutDisplay.sprite = m_keyboardLayout[(int)m_characterShoot.m_aimModeState];
        m_keyboardText.text = "(keyboard) " + m_characterShoot.m_aimModeState.ToString();
    }
}
