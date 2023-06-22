using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChooseSkillManualy : MonoBehaviour
{
    [SerializeField] private int m_skillNumber;
    [SerializeField] private Slider m_slider;
    [SerializeField] private Text m_txtNumberSkill;
    public int currentSkill;
    [SerializeField] private Character.CharacterShoot characterShootScript;
    // Start is called before the first frame update
    void Start()
    {
        m_slider = this.GetComponent<Slider>();
        if(characterShootScript == null)
        {
            characterShootScript = GameObject.Find("Player").GetComponent<Character.CharacterShoot>();
            InitialyseSkill();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSkill()
    {
        currentSkill = (int)m_slider.value;
        m_txtNumberSkill.text = "" + currentSkill;
        characterShootScript.capsuleIndex[m_skillNumber] = currentSkill;
        characterShootScript.InitCapsule();
    }

    public void InitialyseSkill()
    {
        currentSkill = characterShootScript.capsuleIndex[m_skillNumber];
        m_slider.value = currentSkill;
        m_txtNumberSkill.text = "" + currentSkill;
    }
}
