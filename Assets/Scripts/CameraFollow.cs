using UnityEngine;
using UnityEngine.InputSystem;
using Freya;

namespace IntroAssignment {
    public class CameraFollow : MonoBehaviour, Controls.ICameraActions {

        [SerializeField] private Transform _target;

        [SerializeField] private bool _clampX = true;
        [SerializeField] private Vector2 _minMaxX = new Vector2(-45,45);

        [Range(0.1f, 1f)] public float mouseSensitivity = 0.5f;
        [SerializeField] private float _maxCameraDist = 5f;
    
        private Transform _selfTransform;
        private Controls _controls;
        private Vector3 _targetRotation = Vector3.zero;

        private bool isAiming;

        public float transitionTime;
        private float _timePassed;
        private Vector3 _sourcePosition;
        private Vector3 _destinationPosition;
        private Quaternion _sourceRotation;
        private Quaternion _destinationRotation;

        private void Awake() {
            _selfTransform = GetComponent<Transform>();
        
            _controls = new Controls();
            _controls.Camera.Enable();
            _controls.Camera.SetCallbacks(this);
            _sourceRotation = Quaternion.identity;
            _destinationRotation = Quaternion.identity;
            // _controls.Camera.Look.performed += OnLook;
        }

        public void SetTarget(Transform newTarget) {
            _timePassed = 0;
            _target = newTarget;
            _sourcePosition = _selfTransform.position;
            _sourceRotation = _selfTransform.rotation;
            _destinationPosition = newTarget.position - newTarget.forward * _maxCameraDist;
            _destinationRotation = Quaternion.Euler(Vector3.up * newTarget.localEulerAngles.y);
            _targetRotation = _destinationRotation.eulerAngles;
            isAiming = false;
        }

        public void OnLook(InputAction.CallbackContext context) {
            if (GameManager.instance.controlState != ControlState.On) return;
            
            Vector2 values = context.ReadValue<Vector2>();
            
                // _targetRotation = _targetRotation.FlattenY() + Vector3.left * values.y * mouseSensitivity;
                //
                // if(_clampX)
                //     _targetRotation.x = Mathf.Clamp(_targetRotation.x,_minMaxX.x,_minMaxX.y);
                //
                // _selfTransform.localEulerAngles = _targetRotation;
            // }
            // else {
            _targetRotation += new Vector3(-values.y,values.x) * mouseSensitivity;
            
            if(_clampX)
                _targetRotation.x = Mathf.Clamp(_targetRotation.x,_minMaxX.x,_minMaxX.y);

            _selfTransform.localEulerAngles = _targetRotation;
            
            if (isAiming) {
                _target.Rotate(Vector3.up, values.x * mouseSensitivity);
            }
                // }
        }

        public void OnAim(InputAction.CallbackContext context) {
            if (GameManager.instance.controlState != ControlState.On) return;
            _selfTransform.rotation = _target.rotation;
            _targetRotation = _target.rotation.eulerAngles;
            if (context.performed) {
                isAiming = true;
            }
            else if (context.canceled) {
                isAiming = false;
            }
        }

        private void LateUpdate() {
            if (GameManager.instance.controlState == ControlState.On ) {
                if (isAiming) {
                    _selfTransform.position = _target.position;
                }
                else{
                    float cameraDist = Physics.Raycast(_target.position, -transform.forward, out RaycastHit hit, _maxCameraDist) ? hit.distance : _maxCameraDist;
                    _selfTransform.position = _target.position - transform.forward * cameraDist;
                }
            }
            else if(GameManager.instance.controlState == ControlState.Transition){
                _timePassed += Time.deltaTime;
                float t = (Mathfs.Cos(((_timePassed / transitionTime) + 1) * Mathfs.PI) + 1) / 2f;
                _selfTransform.position = Vector3.Lerp(_sourcePosition, _destinationPosition, t.Clamp01());
                _selfTransform.rotation = Quaternion.Lerp(_sourceRotation,_destinationRotation,t.Clamp01());
                if (_timePassed >= transitionTime) GameManager.instance.controlState = ControlState.On;
            }
        }
    }
}