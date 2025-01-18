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
        [SerializeField] private ContentSizeFitter m_contentSizeFitter;
        [SerializeField] private GameObject gameObject_ContentImage;
        [SerializeField] private RectTransform holder_contentImage;
        [SerializeField] private RectTransform holder_contentImage_Background;
        private Vector2 imageOffSet = Vector2.zero;

        public void Start()
        {
            m_inputModule = instance.uiInputModule;
            gameObject.SetActive(false);
        }

        public void SetPosition(TooltipPositionData positionData)
        {
            Vector2 position = positionData.basePosition;
             //position = m_inputModule.point.action.ReadValue<Vector2>();

            float pivotX = position.x / Screen.width;
            float pivotY = position.y / Screen.height;

            m_rectTransform.pivot = new Vector2(pivotX, pivotY);
            m_rectTransform.anchoredPosition = position + positionData.offset;
            imageOffSet = positionData.additionnalOffset;
            m_contentSizeFitter.SetLayoutHorizontal();
            m_contentSizeFitter.SetLayoutVertical();
        }
        
   
        public void SetText(TooltipDisplayData displayData)
        {

            if (string.IsNullOrEmpty(displayData.header))
            {
                m_header.gameObject.SetActive(false);
            }
            else
            {
                m_header.gameObject.SetActive(true);
                m_header.text = displayData.header;
            }

            m_content.text = displayData.content;


            // Check size of tooltip text
            int headerLength = m_header.text.Length;
            int contentLength = m_content.text.Length;

            m_layoutElement.enabled = (headerLength > m_characterWrapLimit || contentLength > m_characterWrapLimit) ? true : false;
            if(displayData.asImageInData)
            {

                gameObject_ContentImage.SetActive(true);
                holder_contentImage.GetComponent<Image>().sprite = displayData.contentImage;
                Vector2 sizeContentImage = displayData.contentImage.textureRect.size;
                float halfWidth_ContentImage = sizeContentImage.x;
                float halfHeight_ContentImage = 0;
                Vector2 offSetPosition = new Vector2(halfWidth_ContentImage, halfHeight_ContentImage);
                holder_contentImage.anchoredPosition = new Vector2(m_rectTransform.anchoredPosition.x + offSetPosition.x, 0) + imageOffSet;
                holder_contentImage.sizeDelta = sizeContentImage;
                holder_contentImage_Background.anchoredPosition = new Vector2(m_rectTransform.anchoredPosition.x + offSetPosition.x, 0) + imageOffSet;
                holder_contentImage_Background.sizeDelta = sizeContentImage + new Vector2(40,40);
            }
            else
            {
                gameObject_ContentImage.SetActive(false);
            }
           
        }

        public void SetImage(TooltipDisplayData displayData)
        {

        }






    }
}