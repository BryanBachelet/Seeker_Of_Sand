using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ui_DisplayText : MonoBehaviour
{
    public TMP_Text m_messageText;
    [SerializeField] private Animator m_animator;
    [SerializeField] private float timeDisplay1;
    [SerializeField] private Image imageOrnementBorder;
    [HideInInspector] private Material material_Ornement;
    [SerializeField] private Texture defaultTexture;
    private float tempsEcoule = 0;
    private bool active = false;
    private bool oldAnimationTransition = false;


    // Start is called before the first frame update
    void Start()
    {
        m_animator = this.GetComponent<Animator>();
        m_messageText = this.GetComponentInChildren<TMP_Text>();
        material_Ornement = imageOrnementBorder.material;
        SearchDisplayAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        if (!active) return;

        if (tempsEcoule > timeDisplay1)
        {
            m_animator.ResetTrigger("Display");
            active = false;

        }
        else
        {
            tempsEcoule += Time.deltaTime;
            if (tempsEcoule > timeDisplay1 / 2)
            {

            }
        }
        if (oldAnimationTransition) return;
        else
        {
            if (tempsEcoule > timeDisplay1)
            {
                m_animator.ResetTrigger("Display");
                m_animator.SetTrigger("DisplayCancel");
                active = false;

            }
            else
            {
                tempsEcoule += Time.deltaTime;
                if (tempsEcoule > timeDisplay1 / 2)
                {

                }
            }
        }
    }

    public void DisplayMessage(string message, Texture textureOrnement)
    {
        if(textureOrnement == null)
        {
            material_Ornement.SetTexture("_Gradient",defaultTexture);
        }
        else
        {
            material_Ornement.SetTexture("_Gradient", textureOrnement);
        }
        m_animator.ResetTrigger("DisplayCancel");
        m_messageText.text = message;
        m_animator.SetTrigger("Display");
        tempsEcoule = 0;
        active = true;
    }

    public void SearchDisplayAnimation()
    {


        AnimationClip[] clips = m_animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].name == "ElementalBonusAcquire")
            {
                timeDisplay1 = clips[i].length;
            }
        }
    }
}
