//
// Klak - Utilities for creative coding with Unity
//
// Copyright (C) 2016 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using UnityEngine;
using Klak.Math;
using Klak.VectorMathExtension;
using UnityEngine.UIElements;

namespace Klak.Motion
{
    /// Follows a given transform smoothly
    [AddComponentMenu("Klak/Motion/Smooth Follow")]
    public class SmoothFollow : MonoBehaviour
    {
        public float distanceMinimumFromTarget = 5;
        public float rangeRandom = 15;
        private Vector3 positionRandom;
        private float rangePositionRnd;
        private float rangeRotationRnd;
        private bool activeFollow = true;

        #region Nested Classes

        public enum Interpolator
        {
            Exponential, Spring, DampedSpring
        }

        #endregion

        #region Editable Properties

        [SerializeField]
        Interpolator _interpolator = Interpolator.DampedSpring;

        [SerializeField]
        Transform _target;

        [SerializeField, Range(0, 20)]
        float _positionSpeed = 2;

        [SerializeField, Range(0, 20)]
        float _rotationSpeed = 2;

        [SerializeField]
        float _jumpDistance = 1;

        [SerializeField, Range(0, 360)]
        float _jumpAngle = 60;

        [SerializeField]
        bool _activeRandom = false;
        #endregion

        #region Public Properties And Methods

        public Interpolator interpolationType
        {
            get { return _interpolator; }
            set { _interpolator = value; }
        }

        public Transform target
        {
            get { return _target; }
            set { _target = value; }
        }

        public float positionSpeed
        {
            get { return _positionSpeed; }
            set { _positionSpeed = value; }
        }

        public float rotationSpeed
        {
            get { return _rotationSpeed; }
            set { _rotationSpeed = value; }
        }

        public float jumpDistance
        {
            get { return _jumpDistance; }
            set { _jumpDistance = value; }
        }

        public float jumpAngle
        {
            get { return _jumpAngle; }
            set { _jumpAngle = value; }
        }

        public bool bookState
        {
            get { return _activeRandom; }
            set { _activeRandom = value; }
        }

        public void Snap()
        {
            if (_positionSpeed > 0) transform.localPosition = target.localPosition;
            if (_rotationSpeed > 0) transform.localRotation = target.localRotation;
        }

        public void JumpRandomly()
        {
            var r1 = Random.Range(0.5f, 1.0f);
            var r2 = Random.Range(0.5f, 1.0f);

            var dp = Random.onUnitSphere * _jumpDistance * r1;
            var dr = Quaternion.AngleAxis(_jumpAngle * r2, Random.onUnitSphere);

            transform.position = dp + target.position;
            transform.position = new Vector3(transform.position.x, target.position.y, transform.position.z);
            transform.rotation = dr * target.rotation;
        }

        #endregion

        #region Private Properties And Functions

        Vector3 _vposition;
        Vector4 _vrotation;

        Vector3 SpringPosition(Vector3 current, Vector3 target)
        {
            _vposition = ETween.Step(_vposition, Vector3.zero, 1 + _positionSpeed * 0.5f);
            _vposition += (target - current) * (_positionSpeed * 0.1f);
            return current + _vposition * Time.deltaTime;
        }

        Quaternion SpringRotation(Quaternion current, Quaternion target)
        {
            var v_current = current.ToVector4();
            var v_target = target.ToVector4();
            _vrotation = ETween.Step(_vrotation, Vector4.zero, 1 + _rotationSpeed * 0.5f);
            _vrotation += (v_target - v_current) * (_rotationSpeed * 0.1f);
            return (v_current + _vrotation * Time.deltaTime).ToNormalizedQuaternion();
        }

        #endregion

        #region MonoBehaviour Functions



        private void Start()
        {
            if (_activeRandom)
            {
                positionRandom = Random.insideUnitSphere * rangeRandom;
                rangePositionRnd = Random.Range(2.5f, 12f);
                rangeRotationRnd = Random.Range(2.5f, 12f);

                _positionSpeed = rangePositionRnd;
                _rotationSpeed = rangeRotationRnd;
            }
            else
            {
                positionRandom = Vector3.zero;
            }

        }
        void Update()
        {
            if (target == null) return;
            if(!activeFollow)
            {
                transform.localPosition = new Vector3(0, 0, 0);
                transform.rotation = new Quaternion(0, 0, 0, 0);
                return;
            }
            Vector3 newPositionRange = Vector3.zero;
            Vector3 positionRange = transform.position - target.position;
            newPositionRange = positionRandom + target.position + positionRange.normalized * 1;

            //Debug.Log(this.name + " goes to [" + newPositionRange);

            if (_interpolator == Interpolator.Exponential)
            {
                if (_positionSpeed > 0)
                    transform.position = ETween.Step(transform.position, newPositionRange, _positionSpeed);
                if (_rotationSpeed > 0)
                    transform.rotation = ETween.Step(transform.rotation, target.rotation, _rotationSpeed);
            }
            else if (_interpolator == Interpolator.DampedSpring)
            {
                if (_positionSpeed > 0)
                    transform.position = DTween.Step(transform.position, newPositionRange, ref _vposition, _positionSpeed);
                if (_rotationSpeed > 0)
                    transform.rotation = DTween.Step(transform.rotation, target.rotation, ref _vrotation, _rotationSpeed);
            }
            else
            {
                if (_positionSpeed > 0)
                    transform.position = SpringPosition(transform.position, newPositionRange);
                if (_rotationSpeed > 0)
                    transform.rotation = SpringRotation(transform.rotation, target.rotation);
            }
        }

        public bool activeGizmo = false;
       
        #endregion

        public void ChangeForBook(bool activeOffSet)
        {
            if (activeOffSet)
            {
                _positionSpeed = 10;
                _jumpDistance = 50;
                _jumpAngle = 45;

                activeFollow = true;
                JumpRandomly();
            }
            else
            {
                _positionSpeed = 0.1f;
                _jumpDistance = 0.1f;
                _jumpAngle = 0.1f;
                
                activeFollow = false;


            }
        }
    }
}
