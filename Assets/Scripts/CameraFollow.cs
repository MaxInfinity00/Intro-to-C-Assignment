using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace IntroAssignment {
    public class CameraFollow : MonoBehaviour {

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
        private Vector3 targetRotation = Vector3.zero;

        private void Awake() {
            _selfTransform = GetComponent<Transform>();
        
            _controls = new Controls();
            _controls.Camera.Enable();
            _controls.Camera.Look.performed += OrbitCamera;
        }

        public void SetTarget(Transform newTarget) {
            _target = newTarget;
        }

        private void OrbitCamera(InputAction.CallbackContext context) {
            Vector2 values = context.ReadValue<Vector2>();

            
            targetRotation += new Vector3(-values.y,values.x) * mouseSensitivity;
            
            if(_clampX)
                targetRotation.x = Mathf.Clamp(targetRotation.x,_minMaxX.x,_minMaxX.y);

            // if (!_smooth)
            _selfTransform.localEulerAngles = targetRotation;
                

        }


        private void LateUpdate() {
            // if(_smooth)
            // {
            //     _selfTransform.localEulerAngles = Vector3.SmoothDamp(_selfTransform.localEulerAngles, targetRotation,
            //         ref _cameraVelocity, _smoothTime);
            // }
            
            float cameraDist;
            if (Physics.Raycast(_target.position, -transform.forward, out RaycastHit hit, _maxCameraDist)) {
                cameraDist = hit.distance;
            }
            else cameraDist = _maxCameraDist;
            _selfTransform.position = _target.position - transform.forward * cameraDist;
        }
    }
}