using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ShadowDetect
{
    // Class Event with bool on parameter (and visible on inspector)
    [System.Serializable]
    public class UnityEventBool : UnityEvent<bool> { }

    public enum ShadowState { UNDEFINED, ON, OUT}

    [System.Serializable]
    public enum DETECT_MODE
    {
        ONE = 0,
        ALL = 1
    }

    [System.Serializable]
    public class ShadowTarget
    {
        public ShadowTarget()
        {
            Name = "";
        }
        [SerializeField]
        public string Name;

        [SerializeField]
        public Transform Target;
        [SerializeField]
        private UnityEventBool _onChangeState;
        [SerializeField]
        private UnityEvent _onEnterShadow;
        [SerializeField]
        private UnityEvent _onExitShadow;
        [SerializeField]
        private UnityEvent _onShadow;
        [SerializeField]
        private UnityEvent _outShadow;

        [HideInInspector]
        public ShadowState IsOnShadow_state;
        [HideInInspector]
        public ShadowState Last_IsOnShadow_state;

        public UnityEventBool OnChangeState
        {
            get
            {
                return _onChangeState;
            }

            set
            {
                _onChangeState = value;
            }
        }

        public UnityEvent OnEnterShadow
        {
            get
            {
                return _onEnterShadow;
            }

            set
            {
                _onEnterShadow = value;
            }
        }

        public UnityEvent OnExitShadow
        {
            get
            {
                return _onExitShadow;
            }

            set
            {
                _onExitShadow = value;
            }
        }

        public UnityEvent OnShadow
        {
            get
            {
                return _onShadow;
            }

            set
            {
                _onShadow = value;
            }
        }

        public UnityEvent OutShadow
        {
            get
            {
                return _outShadow;
            }

            set
            {
                _outShadow = value;
            }
        }

        public bool IsOnShadow
        {
            get
            {
                return (IsOnShadow_state == ShadowState.ON);
            }

            set
            {
                IsOnShadow_state = (value)? ShadowState.ON : ShadowState.OUT;
            }
        }

        public bool Last_IsOnShadow
        {
            get
            {
                return (Last_IsOnShadow_state == ShadowState.ON);
            }

            set
            {
                Last_IsOnShadow_state = (value) ? ShadowState.ON : ShadowState.OUT;
            }
        }
    }

    public class ShadowDetect : MonoBehaviour
    {
        [SerializeField]
        private bool _isAuto = true;
        [SerializeField]
        private List<Light> _lights;

        [SerializeField]
        public int _minimumNbTargetOnShadow = 1;
        public DETECT_MODE _detectMode = DETECT_MODE.ALL;

        [SerializeField]
        public float _raycastinRate = 60f;
        private CustomFixedUpdate _CustomFixedUpdate;

        [SerializeField]
        private List<ShadowTarget> _targets;
        [SerializeField]
        private LayerMask _layers;

        [SerializeField]
        private UnityEventBool _onChangeState;
        [SerializeField]
        private UnityEvent _onEnterShadow;
        [SerializeField]
        private UnityEvent _onExitShadow;
        [SerializeField]
        private UnityEvent _onShadow;
        [SerializeField]
        private UnityEvent _outShadow;

        [HideInInspector]
        public ShadowState IsOnShadow_state = ShadowState.UNDEFINED;
        [HideInInspector]
        public ShadowState Last_IsOnShadow_state = ShadowState.UNDEFINED;

        public List<Light> Lights
        {
            get
            {
                return _lights;
            }

            set
            {
                _lights = value;
            }
        }

        public UnityEventBool OnChangeState
        {
            get
            {
                return _onChangeState;
            }

            private set
            {
                _onChangeState = value;
            }
        }

        public UnityEvent OnEnterShadow
        {
            get
            {
                return _onEnterShadow;
            }

            private set
            {
                _onEnterShadow = value;
            }
        }

        public UnityEvent OnExitShadow
        {
            get
            {
                return _onExitShadow;
            }

            private set
            {
                _onExitShadow = value;
            }
        }

        public bool IsAuto
        {
            get
            {
                return _isAuto;
            }

            set
            {
                _isAuto = value;
            }
        }

        public UnityEvent OnShadow
        {
            get
            {
                return _onShadow;
            }

            set
            {
                _onShadow = value;
            }
        }

        public UnityEvent OutShadow
        {
            get
            {
                return _outShadow;
            }

            set
            {
                _outShadow = value;
            }
        }

        public bool IsOnShadow
        {
            get
            {
                return (IsOnShadow_state == ShadowState.ON);
            }

            set
            {
                IsOnShadow_state = (value) ? ShadowState.ON : ShadowState.OUT;
            }
        }

        public bool Last_IsOnShadow
        {
            get
            {
                return (Last_IsOnShadow_state == ShadowState.ON);
            }

            set
            {
                Last_IsOnShadow_state = (value) ? ShadowState.ON : ShadowState.OUT;
            }
        }

        void Awake()
        {
            _CustomFixedUpdate = new CustomFixedUpdate(1.0f/_raycastinRate, _FixedUpdate);
            _minimumNbTargetOnShadow = (_detectMode == DETECT_MODE.ONE) ? 1 : _targets.Count;
            if (_isAuto)
            {
                Lights = new List<Light>();
                Lights = FindObjectsOfType<Light>().ToList();
            }
        }

        // Update is called once per frame
        void Update()
        {
            _CustomFixedUpdate.Update(Time.deltaTime);
        }

        void _FixedUpdate()
        {
            if (Lights == null || _targets == null || _targets.Count == 0)
                return;
            ShadowTarget current_st;
            int nbTargetOnShadow = 0;
            //Browse the list of target
            for (int i = 0; i < _targets.Count; ++i)
            {
                 current_st = _targets[i];
                current_st.IsOnShadow = !IsOnLight(Lights, current_st.Target.position);

                //Call Events
                if (current_st.Last_IsOnShadow_state != current_st.IsOnShadow_state)
                {
                    current_st.OnChangeState.Invoke(current_st.IsOnShadow);
                    if (current_st.IsOnShadow)
                        current_st.OnEnterShadow.Invoke();
                    else
                        current_st.OnExitShadow.Invoke();
                }
                if (current_st.IsOnShadow)
                    current_st.OnShadow.Invoke();
                else
                    current_st.OutShadow.Invoke();

                current_st.Last_IsOnShadow = current_st.IsOnShadow;

                if (current_st.IsOnShadow)
                    nbTargetOnShadow++;
            }

            IsOnShadow = (nbTargetOnShadow >= _minimumNbTargetOnShadow);
            //Call Events
            if (Last_IsOnShadow_state != IsOnShadow_state)
            {
                OnChangeState.Invoke(IsOnShadow);
                if (IsOnShadow)
                    OnEnterShadow.Invoke();
                else
                    OnExitShadow.Invoke();
            }
            if (IsOnShadow)
                OnShadow.Invoke();
            else
                OutShadow.Invoke();

            Last_IsOnShadow = IsOnShadow;
        }

        /// <summary>
        /// Detect if your character is on Light
        /// </summary>
        /// <param name="light"></param>
        /// <returns></returns>
        public bool IsOnLight(List<Light> light, Vector3 target_position)
        {
            //Browse the list of lights
            for (int i = 0; i < light.Count; ++i)
            {
                var onshadow = !IsOnLight(light[i], target_position);
                if (!onshadow)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Detect if your character is on Light
        /// </summary>
        /// <param name="light"></param>
        /// <returns></returns>
        public bool IsOnLight(Light light, Vector3 target_position)
        {
            if (!light.isActiveAndEnabled || light.intensity == 0)
                return false;

            //0: Spot - 1: Directional - 2: Point
            switch ((int)light.type)
            {
                case 0:
                    return IsOnSpotlLight(light, target_position);
                case 1:
                    return IsOnDirectionalLight(light, target_position);
                case 2:
                    return IsOnPointLight(light, target_position);
                default:
                    return false;
            }
        }

        bool IsOnDirectionalLight(Light light, Vector3 target_position)
        {
            RaycastHit hit;
            Ray ray = new Ray(target_position, -light.transform.forward);
#if UNITY_EDITOR
            Debug.DrawRay(ray.origin, ray.direction, Color.red);
#endif
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~_layers))
            {
                // Do Stuff if you want
                return false;
            }

            return true;
        }
        bool IsOnSpotlLight(Light light, Vector3 target_position)
        {
            Vector3 FromLightToCharacter = target_position - light.transform.position;

            if (FromLightToCharacter.magnitude > light.range * (light.intensity / 10.0f) || Vector3.Angle(light.transform.forward, FromLightToCharacter) > light.spotAngle / 2.0f)
                return false;

            RaycastHit hit;
            Ray ray = new Ray(light.transform.position, FromLightToCharacter);

#if UNITY_EDITOR
            Debug.DrawRay(ray.origin, ray.direction, Color.blue);
#endif

            if (Physics.Raycast(ray, out hit, FromLightToCharacter.magnitude, ~_layers))
            {
                // Do Stuff if you want
                return false;
            }

            return true;
        }
        bool IsOnPointLight(Light light, Vector3 target_position)
        {
            Vector3 FromCharacterToLight = light.transform.position - target_position;

            if (FromCharacterToLight.magnitude >= light.range * (light.intensity / 10.0f))
                return false;

            RaycastHit hit;
            Ray ray = new Ray(target_position, FromCharacterToLight);

#if UNITY_EDITOR
            Debug.DrawRay(ray.origin, ray.direction, Color.blue);
#endif

            if (Physics.Raycast(ray, out hit, FromCharacterToLight.magnitude, ~_layers))
            {
                // Do Stuff if you want
                return false;
            }

            return true;
        }

        void OnDrawGizmos()
        {
            if (Lights == null || _targets == null || _targets.Count == 0)
                return;

            ShadowTarget current_st;
            for (int i = 0; i < _targets.Count; ++i)
            {
                current_st = _targets[i];
                if (current_st.IsOnShadow)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(current_st.Target.position, 0.1f);
            }
        }
    }
}
