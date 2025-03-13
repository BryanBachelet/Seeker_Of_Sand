using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ui_DisplayText : MonoBehaviour
{
    public TMP_Text m_messageText;
    [SerializeField] private Animator m_animator;
    [SerializeField] private float timeDisplay1;
    private float tempsEcoule = 0;
    private bool active = false;
    // Start is called before the first frame update
    void Start()
    {
        m_animator = this.GetComponent<Animator>();
        m_messageText = this.GetComponentInChildren<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!active) return;
        else
        {
            if(tempsEcoule > timeDisplay1)
            {
                m_animator.ResetTrigger("Display");
                m_animator.SetTrigger("DisplayCancel");
                active = false;

            }
            else 
            {
                tempsEcoule += Time.deltaTime;
                if(tempsEcoule > timeDisplay1 /2)
                {

                }
            }
        }
    }

    public void DisplayMessage(string message)
    {
        m_animator.ResetTrigger("DisplayCancel");
        m_messageText.text = message;
        m_animator.SetTrigger("Display");
        tempsEcoule = 0;
        active = true;
    }
}
