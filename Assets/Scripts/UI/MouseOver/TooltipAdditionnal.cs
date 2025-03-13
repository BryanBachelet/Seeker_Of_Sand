using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using GuerhoubaTools.Gameplay;
using JetBrains.Annotations;

namespace GuerhoubaGames.UI
{


    public class TooltipAdditionnal : MonoBehaviour
    {
        [Flags]
        public enum PositionTooltipOffset
        {
            None = 0,
            Horizontal = 1,
            Vertical = 2,
            Both = Horizontal | Vertical,
        }

        [HideInInspector] public PlayerInput instance;
        private InputSystemUIInputModule m_inputModule;

        [SerializeField] private LayoutElement m_layoutElement;
        [SerializeField] private TMP_Text m_header;
        [SerializeField] private TMP_Text m_content;
        [SerializeField] public RectTransform rectTransform;
        [SerializeField] private int m_characterWrapLimit = 500;
        [SerializeField] private ContentSizeFitter m_contentSizeFitter;
        [SerializeField] private GameObject gameObject_ContentImage;
        [SerializeField] private RectTransform holder_contentImage;
        [SerializeField] private RectTransform holder_contentImage_Background;
        private Vector2 imageOffSet = Vector2.zero;


        public bool isDebugActive;
        private Vector2 startPosition;
        private float finalHeight;
        private float finalWidth;

        private float signHeight;
        private float signWidth;

        public Vector2 pivotVirtual;
        void Start()
        {
            //  m_inputModule = instance.uiInputModule;
            gameObject.SetActive(false);
            rectTransform = GetComponent<RectTransform>();
            m_contentSizeFitter = GetComponent<ContentSizeFitter>();
            startPosition = rectTransform.position;
            ResetLayout();
        }

        private void OnDisable()
        {
            rectTransform.anchoredPosition = startPosition;
        }

        public void SetPosition(TooltipPositionData positionData, PositionTooltipOffset positionTooltipOffset, Vector2 otherOffset = new Vector2(), bool isTest = false)
        {
            Vector2 position = positionData.basePosition;

            pivotVirtual.x = (position.x + positionData.offset.x) / Screen.width;
            pivotVirtual.y = (position.y + positionData.offset.y) / Screen.height;

            // Remap of the pivot value to get sign
            signWidth = Mathf.Sign(((1.0f - pivotVirtual.x) - .5f) / 2.0f);
            signHeight = Mathf.Sign(((1.0f - pivotVirtual.y) - .5f) / 2.0f);

            float pivotX = (position.x ) / Screen.width;
            float pivotY = (position.y ) / Screen.height;

         


            rectTransform.pivot = new Vector2(pivotX, pivotY);

            rectTransform.anchoredPosition = position + positionData.offset + otherOffset;

            imageOffSet = positionData.additionnalOffset;
            Vector2 sizeContentImage = holder_contentImage.GetComponent<Image>().sprite.textureRect.size;
            float halfWidth_ContentImage = sizeContentImage.x;
            float halfHeight_ContentImage = 0;
            Vector2 offSetPosition = new Vector2(halfWidth_ContentImage, halfHeight_ContentImage);
            holder_contentImage.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + offSetPosition.x, 0) + imageOffSet;
            holder_contentImage.sizeDelta = sizeContentImage;
            holder_contentImage_Background.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + offSetPosition.x, 0) + imageOffSet;
            holder_contentImage_Background.sizeDelta = sizeContentImage + new Vector2(40, 40);

            m_contentSizeFitter.SetLayoutHorizontal();
            m_contentSizeFitter.SetLayoutVertical();


            if (positionTooltipOffset == PositionTooltipOffset.Horizontal || positionTooltipOffset == PositionTooltipOffset.Both)
            {
                otherOffset.x += GetInternalWidth();
            }
            if (positionTooltipOffset == PositionTooltipOffset.Vertical || positionTooltipOffset == PositionTooltipOffset.Both)
            {
                otherOffset.y += GetInternalHeight();
            }

            rectTransform.anchoredPosition = position + positionData.offset + otherOffset;
            if (isDebugActive) m_layoutElement.enabled |= rectTransform.rect.width > m_layoutElement.preferredWidth;
        }


        public float GetExtenalWidth()
        {
            float sign = signWidth;
            if (sign == 0)
                sign = 1.0f;

            if (sign >= 0)
            {
                return sign * ((1.0f - rectTransform.pivot.x) * rectTransform.rect.width);
            }
            else
            {
                return sign * (rectTransform.pivot.x * rectTransform.rect.width);
            }

        }


        /// <summary>
        /// Get height from pivot point to the border 
        /// </summary>
        /// <returns></returns>
        public float GetExternalHeight()
        {
            float sign = signHeight;
            if (sign == 0) 
                sign = 1.0f;

            if (sign >= 0)
            {
                return sign * ((1.0f - rectTransform.pivot.y) * rectTransform.rect.height);
            }
            else
            {
                return sign *(rectTransform.pivot.y * rectTransform.rect.height);
            }

        }

        /// <summary>
        /// Get height from border to the pivot point
        /// </summary>
        /// <returns></returns>
        public float GetInternalHeight()
        {
            float sign = signHeight;
            if (sign == 0)
                sign = 1.0f;
            if (sign < 0)
            {
                return sign * ((1.0f - rectTransform.pivot.y) * rectTransform.rect.height);
            }
            else
            {
                return sign *(rectTransform.pivot.y * rectTransform.rect.height);
            }

        }

        public float GetInternalWidth()
        {
            float sign = signWidth;
            if (sign == 0)
                sign = 1.0f;
            if (signWidth < 0)
            {
                return sign * ((1.0f - rectTransform.pivot.x) * rectTransform.rect.width);
            }
            else
            {
                return sign * (rectTransform.pivot.x * rectTransform.rect.width);
            }

        }


        public void SetPreferredWidth(float prefferedWidth)
        {
            m_layoutElement.preferredWidth = prefferedWidth;

        }

        public void ResetLayout()
        {
            m_contentSizeFitter.SetLayoutHorizontal();
            m_contentSizeFitter.SetLayoutVertical();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
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

            m_content.SetText( displayData.content);
            m_content.ForceMeshUpdate(true,true);


            if (isDebugActive)
            {
                Debug.Log("This is debug");
            }

            for (int i = 0; i < m_content.textInfo.linkInfo.Length; i++)
            {
                TooltipManager.AddSystemTooltip(m_content.textInfo.linkInfo[i].GetLinkID());
            }

            // Check size of tooltip text
            int headerLength = m_header.text.Length;
            int contentLength = m_content.text.Length;


            m_layoutElement.enabled = (headerLength > m_characterWrapLimit || contentLength > m_characterWrapLimit) ? true : false;
            if (displayData.asImageInData)
            {

                gameObject_ContentImage.SetActive(true);
                holder_contentImage.GetComponent<Image>().sprite = displayData.contentImage;
              
            }
            else
            {
                gameObject_ContentImage.SetActive(false);
            }




        }
    }
}