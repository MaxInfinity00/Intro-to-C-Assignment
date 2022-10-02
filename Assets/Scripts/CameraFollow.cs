using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Freya;

namespace IntroAssignment {
    public class CameraFollow : MonoBehaviour, Controls.ICameraActions {

        [SerializeField] private Transform _target;

        [SerializeField] private bool _clampX = true;
        [SerializeField] private Vector2 _minMaxX = new Vector2(-45,45);

        // [SerializeField] private bool _smooth = true;
        // [SerializeField] private float _smoothTime = 0.2f;
        // [SerializeField] private Vector3 _cameraVelocity = Vector3.zero;

        [Range(0.1f, 1f)] public float mouseSensitivity = 0.5f;
        [SerializeField] private float _maxCameraDist = 5f;
    
        private Transform _selfTransform;
        private Controls _controls;
        private Vector3 _targetRotation = Vector3.zero;

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
            // _controls.Camera.Look.performed += OnLook;
        }

        public void SetTarget(Transform newTarget) {
            _target = newTarget;
        }

        public void OnLook(InputAction.CallbackContext context) {
            if (!GameManager.canControl) return;
            Vector2 values = context.ReadValue<Vector2>();

            
            _targetRotation += new Vector3(-values.y,values.x) * mouseSensitivity;
            
            if(_clampX)
                _targetRotation.x = Mathf.Clamp(_targetRotation.x,_minMaxX.x,_minMaxX.y);

            // if (!_smooth)
            _selfTransform.localEulerAngles = _targetRotation;
                

        }

        public void StartTransition() {
            _sourcePosition = _selfTransform.position;
            _sourceRotation = _selfTransform.rotation;
        }
        private void LateUpdate() {
            if (GameManager.canControl) {
                float cameraDist;
                if (Physics.Raycast(_target.position, -transform.forward, out RaycastHit hit, _maxCameraDist)) {
                    cameraDist = hit.distance;
                }
                else cameraDist = _maxCameraDist;
                _selfTransform.position = _target.position - transform.forward * cameraDist;
            }
            else {
                _timePassed += Time.deltaTime;
                float t = Mathfs.Sin(_timePassed / transitionTime * Mathfs.TAU);
                _selfTransform.position = Vector3.Lerp(_sourcePosition, _destinationPosition, t);
            }
        }
    }
}