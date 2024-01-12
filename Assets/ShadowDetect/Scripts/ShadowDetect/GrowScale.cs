using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowScale : MonoBehaviour {
    public Transform _transform;
    public bool _isAnimated = true;
    public float _speedAnimation = 1.0f;
    [System.Serializable]
    public struct MinMax
    {
        public float Min;
        public float Max;
    };
    public MinMax _minMaxScale;

    private Vector3 _targetScale = Vector3.one;

	// Use this for initialization
	void Awake () {
        if (_transform == null)
            _transform = gameObject.transform;
    }
	
    void Update()
    {
        _transform.localScale = Vector3.Lerp(_transform.localScale, _targetScale, _speedAnimation);
    }

	public void Grow(float value)
    {
        if (_isAnimated)
            GrowAnimation(value);
        else
            _transform.localScale = new Vector3(value, value, value);
    }
    public void Grow(bool isBig)
    {
        float value = (isBig) ? _minMaxScale.Max : _minMaxScale.Min;
        if (_isAnimated)
            GrowAnimation(value);
        else
            _transform.localScale = new Vector3(value, value, value);
    }
    private void GrowAnimation(float value)
    {
        _targetScale = new Vector3(value, value, value);
    }
}
