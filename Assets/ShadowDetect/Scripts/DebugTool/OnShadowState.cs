using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace DebugTool
{
    [RequireComponent(typeof(Image))]
    public class OnShadowState : MonoBehaviour
    {
        public Image _icone;
        public Sprite _spriteOutShadow;
        public Sprite _spriteOnShadow;

        [SerializeField]
        private bool _onShadow = false;

        // Use this for initialization
        void Awake()
        {
            if(_icone == null)
            {
                _icone = GetComponent<Image>();
            }

            Init(_onShadow);
        }

        public void SetOnShadow(bool value)
        {
            if (_spriteOutShadow == null || _spriteOnShadow == null)
                return;

            _icone.sprite = (value) ? _spriteOnShadow : _spriteOutShadow;

            _onShadow = value;
        }

        private void Init(bool value)
        {
            if (_spriteOutShadow == null || _spriteOnShadow == null)
                return;

            _icone.sprite = (value) ? _spriteOnShadow : _spriteOutShadow;
        }
    }
}

