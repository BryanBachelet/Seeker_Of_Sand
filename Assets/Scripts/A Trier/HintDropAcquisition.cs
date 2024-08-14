using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class HintDropAcquisition : MonoBehaviour
{
    [System.Serializable]
    public struct DropInfo
    {
        public string m_dropType;
        public Sprite m_dropImage;
        public string dropDescription;
        public string dropName;
    }

    #region minorDrop
    [SerializeField] private float m_timeDisplayMinorLoot = 1;
    [SerializeField] private Animator m_animatorMinor;
    [SerializeField] public List<DropInfo> m_minorDropBuffer = new List<DropInfo>();
    [SerializeField] public Image m_minorBackGroundImageReference;
    [SerializeField] public Image m_minorDropImageReference;
    [SerializeField] public TMP_Text m_minorDropDescription;
    [SerializeField] public TMP_Text m_minorDropName;
    [SerializeField] public TMP_Text m_minorDropType;
    public Color minorCurrentBackGroundColor;
    private bool m_isMinorRemove;
    float lastminorDropLoot;
    #endregion
    #region majorDrop
    [SerializeField] private Image m_bandeauObject;
    [SerializeField] private float m_timeDisplayMajorLoot = 1;
    [SerializeField] private Animator m_animatorMajor;
    [SerializeField] public List<DropInfo> m_majorDropBuffer = new List<DropInfo>();
    [SerializeField] public Image m_majorBackGroundImageReference;
    [SerializeField] public Image m_majorDropImageReference;
    [SerializeField] public TMP_Text m_majorDropDescription;
    [SerializeField] public TMP_Text m_majorDropName;
    [SerializeField] public TMP_Text m_majorDropType;
    public Color majorCurrentBackGroundColor;
    private bool m_isMajorRemove;
    float lastmajorDropLoot;
    float lastMajorDropTime = 0;
    
    #endregion

    public bool activeGetColor = false;


    public bool stopEditMode = false;


    // Start is called before the first frame update
    void Start()
    {
        lastminorDropLoot =  Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (activeGetColor)
        {
            activeGetColor = false;
            stopEditMode = false;
        }
        if (!stopEditMode)
        {
            float time = Time.time;
            if (time > lastminorDropLoot + (m_timeDisplayMinorLoot / 2) && !m_isMinorRemove)
            {
                m_animatorMinor.SetBool("Open", false);
                if(m_minorDropBuffer.Count!= 0) m_minorDropBuffer.RemoveAt(0);
              
                m_isMinorRemove = true;
            }
            if (time > lastminorDropLoot + m_timeDisplayMinorLoot)
            {
                
                //m_animator.SetBool("Open", false);
                if (m_minorDropBuffer.Count >= 1)
                {
                    ActivationDisplayMinorLoot();
                }
            }

            if (time > lastmajorDropLoot + (m_timeDisplayMajorLoot / 2) && !m_isMajorRemove)
            {
                m_animatorMajor.SetBool("Open", false);
                if (m_majorDropBuffer.Count != 0) m_majorDropBuffer.RemoveAt(0);
                m_isMajorRemove = true;
                lastMajorDropTime = Time.time;
            }
            else if(!m_isMajorRemove)
            {
                float fadeStep = (Time.time - lastMajorDropTime) / 2 - 0.5f;
                m_bandeauObject.material.SetFloat("_Fade_Step", fadeStep);
                //Debug.Log(fadeStep);

            }
            else if (m_isMajorRemove)
            {
                float fadeStep = (lastMajorDropTime + 1 - Time.time) / 1 - 0.5f;
                m_bandeauObject.material.SetFloat("_Fade_Step", fadeStep);
                //Debug.Log(fadeStep);

            }
            if (time > lastmajorDropLoot + m_timeDisplayMajorLoot)
            {

                //m_animator.SetBool("Open", false);
                if (m_majorDropBuffer.Count >= 1)
                {
                    ActivationDisplayMajorLoot();
                }
            }
        }

    }

    public void AddNewMinorDrop(DropInfo newDropInfo)
    {
        m_minorDropBuffer.Add(newDropInfo);
    }

    public void AddMajorDrop(DropInfo newDropInfo)
    {
        m_majorDropBuffer.Add(newDropInfo);
    }
    public void ActivationDisplayMinorLoot()
    {
        lastminorDropLoot = Time.time;
        majorCurrentBackGroundColor = GetRandomColorInSprite(m_minorDropBuffer[0].m_dropImage);
        m_minorBackGroundImageReference.color = majorCurrentBackGroundColor;
        m_minorDropImageReference.sprite = m_minorDropBuffer[0].m_dropImage;
        m_minorDropName.text = m_minorDropBuffer[0].dropName;
        m_minorDropDescription.text = m_minorDropBuffer[0].dropDescription;
        m_minorDropType.text = m_minorDropBuffer[0].m_dropType;
        m_animatorMinor.SetBool("Open", true);
        m_isMinorRemove = false;
    }
    public void ActivationDisplayMajorLoot()
    {
        lastmajorDropLoot = Time.time;
        majorCurrentBackGroundColor = GetRandomColorInSprite(m_majorDropBuffer[0].m_dropImage);
        m_majorBackGroundImageReference.color = majorCurrentBackGroundColor;
        m_majorDropImageReference.sprite = m_majorDropBuffer[0].m_dropImage;
        m_majorDropName.text = m_majorDropBuffer[0].dropName;
        m_majorDropDescription.text = m_majorDropBuffer[0].dropDescription;
        m_majorDropType.text = m_majorDropBuffer[0].m_dropType;
        m_animatorMajor.SetBool("Open", true);
        m_isMajorRemove = false;
        lastMajorDropTime = Time.time;

    }

    public void DeactivationDiplayLoot()
    {
        m_minorDropImageReference.sprite = null;
        m_minorDropName.text ="";
        m_minorDropDescription.text ="";
        m_minorDropType.text = "";
        m_animatorMinor.SetBool("Open", true);
        m_isMinorRemove = false;
    }

    public Color GetRandomColorInSprite(Sprite sprite)
    {
        Sprite currentDropSprite = sprite;
        float width = currentDropSprite.rect.width / 2;
        float height = currentDropSprite.rect.height / 2;
        int rndW = Random.Range(0, (int)width);
        int rndH = Random.Range(0, (int)height);
        Color newColor = currentDropSprite.texture.GetPixel(rndW, rndH);
        newColor = new Color(Mathf.Clamp(newColor.r, 0.25f, 0.75f), Mathf.Clamp(newColor.g, 0.25f, 0.75f), Mathf.Clamp(newColor.b, 0.25f, 0.75f));
        newColor.a = 1;

        return newColor;
    }
}
