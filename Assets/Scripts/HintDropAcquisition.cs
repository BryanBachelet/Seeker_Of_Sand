using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    [SerializeField] private Animator m_animator;
    [SerializeField] public List<DropInfo> m_dropBuffer = new List<DropInfo>();
    [SerializeField] public Image m_BackGroundImageReference;
    [SerializeField] public Image m_dropImageReference;
    [SerializeField] public TMP_Text m_dropDescription;
    [SerializeField] public TMP_Text m_dropName;
    [SerializeField] public TMP_Text m_dropType;
    public Color currentBackGroundColor;

    public bool activeGetColor = false;

    float lastdropLoot;

    public bool stopEditMode = false;

    private bool m_isRemove;

    // Start is called before the first frame update
    void Start()
    {
        lastdropLoot =  Time.time;
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
            if (time > lastdropLoot + 5 && !m_isRemove)
            {
                m_animator.SetBool("Open", false);
                m_dropBuffer.RemoveAt(0);
              
                m_isRemove = true;
            }
            if (time > lastdropLoot + 10)
            {
                
                //m_animator.SetBool("Open", false);
                if (m_dropBuffer.Count >= 1)
                {
                    ActivationDisplayLoot();
                }
            }
        }

    }

    public void AddNewDrop(DropInfo newDropInfo)
    {
        m_dropBuffer.Add(newDropInfo);
    }
    public void ActivationDisplayLoot()
    {
        lastdropLoot = Time.time;
        currentBackGroundColor = GetRandomColorInSprite();
        m_BackGroundImageReference.color = currentBackGroundColor;
        m_dropImageReference.sprite = m_dropBuffer[0].m_dropImage;
        m_dropName.text = m_dropBuffer[0].dropName;
        m_dropDescription.text = m_dropBuffer[0].dropDescription;
        m_dropType.text = m_dropBuffer[0].m_dropType;
        m_animator.SetBool("Open", true);
        m_isRemove = false;
    }

    public void DeactivationDiplayLoot()
    {
        m_dropImageReference.sprite = null;
        m_dropName.text ="";
        m_dropDescription.text ="";
        m_dropType.text = "";
        m_animator.SetBool("Open", true);
        m_isRemove = false;
    }

    public Color GetRandomColorInSprite()
    {
        Sprite currentDropSprite = m_dropBuffer[0].m_dropImage;
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
