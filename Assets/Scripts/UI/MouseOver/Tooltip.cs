using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;

namespace GuerhoubaGames.UI
{
    public class Tooltip : MonoBehaviour
    {

        public PlayerInput instance;
        
        private InputSystemUIInputModule m_inputModule;
        [SerializeField] private LayoutElement m_layoutElement;
        [SerializeField] private TMP_Text m_header;
        [SerializeField] private TMP_Text m_content;
        [SerializeField] private RectTransform m_rectTransform;
        [SerializeField] private int m_characterWrapLimit = 500;


        public void Start()
        {
            m_inputModule = instance.uiInputModule;
            gameObject.SetActive(false);
        }

        public void SetText(string content, string header = "")
        {
            if (string.IsNullOrEmpty(header))
            {
                m_header.gameObject.SetActive(false);
            }
            else
            {
                m_header.gameObject.SetActive(true);
                m_header.text = header;
            }

            m_content.text = content;


            // Check size of tooltip text
            int headerLength = m_header.text.Length;
            int contentLength = m_content.text.Length;

            m_layoutElement.enabled = (headerLength > m_characterWrapLimit || contentLength > m_characterWrapLimit) ? true : false;

            Vector2 position = m_inputModule.point.action.ReadValue<Vector2>();

            float pivotX = position.x / Screen.width;
            float pivotY = position.y / Screen.height;
            m_rectTransform.pivot = new Vector2(pivotX, pivotY);
            transform.position = position;
        }

        public void Update()
        {
           Vector2 position = m_inputModule.point.action.ReadValue<Vector2>();

            float pivotX = position.x / Screen.width;
            float pivotY = position.y / Screen.height;
            m_rectTransform.pivot = new Vector2(pivotX, pivotY);
            transform.position = position;
        }


    }
}