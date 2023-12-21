using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LayoutControllerSelection : MonoBehaviour
{
    [SerializeField] private Image m_keyboardControl;
    [SerializeField] private Image m_gamepadControl;
    [SerializeField] private Sprite[] m_layoutControl = new Sprite[6];
    // Start is called before the first frame update
    void Start()
    {
        int layout = GameManager.aimModeNumber;
        m_keyboardControl.sprite = m_layoutControl[layout];
        m_gamepadControl.sprite = m_layoutControl[layout + 3];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
