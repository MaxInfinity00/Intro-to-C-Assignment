using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Freya;

namespace IntroAssignment {
    public class Player : MonoBehaviour, Controls.IPlayerActions {

        public float movementSpeed;
        public float jumpForce;
        public float groundCheckHeight;
        
        private float _health;
        private float _maxHealth;
        public bool isAlive = true;

        public List<Weapon> weapons;
        private int _currentWeaponIndex;
    
        public Transform playerTransform;
        private Rigidbody _rigidbody;

        private Controls controls;

        [SerializeField]private Transform _cameraTransform;

        void Awake() {
            isAlive = true;
            playerTransform = GetComponent<Transform>();
            _rigidbody = GetComponent<Rigidbody>();

            _cameraTransform = Camera.main.transform;
            
            controls = new Controls();
            controls.Player.SetCallbacks(this);
            
            GameManager.instance.AddPlayer(this);
        }

        private void FixedUpdate() {
            if (GameManager.instance.controlState != ControlState.On) return;
            Vector2 movement = controls.Player.Move.ReadValue<Vector2>();
            if(movement != Vector2.zero)
                PlayerMove(movement);
        }

        public void OnMove(InputAction.CallbackContext _) {
            // PlayerMove(context.ReadValue<Vector2>());
        }

        public void PlayerMove(Vector2 movement) {
            Vector3 targetPosition = (_cameraTransform.right.FlattenY() * movement.x) +
                                     (_cameraTransform.forward.FlattenY() * movement.y);
            // Debug.Log(playerTransform.position + " " + targetPosition);
            playerTransform.LookAt(playerTransform.position + targetPosition);
            playerTransform.Translate(Vector3.forward * movementSpeed);
        }

        public void OnJump(InputAction.CallbackContext _) {
            if(GameManager.instance.controlState == ControlState.On && GroundCheck())
                _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        private bool GroundCheck() {
            return Physics.Raycast(playerTransform.position, Vector3.down, groundCheckHeight);
        }

        public void OnSwitchWeapon(InputAction.CallbackContext context) {
            if (!context.performed) return;
            Debug.Log(context.ReadValue<float>());
        }

        public void OnAim(InputAction.CallbackContext _) {
            
        }

        public void OnFire(InputAction.CallbackContext _) {
            // Debug.Log("Fire");
        }
        
        ///<summary> Heal the Player </summary>
        public void Heal(int healAmount) {
            _health += healAmount;
            _health.Clamp(0,_maxHealth);
        }

        ///<summary> Damage the Player </summary>
        public void TakeDamage(int damage) {
            _health -= damage;
            if (_health <= 0)  Die();
            else _health.Clamp(0,_maxHealth);
        }

        public void Die() {
            isAlive = false;
            
            //other dead shit
        }

        public void SetControls(bool b) {
            if (b) controls.Player.Enable();
            else controls.Player.Disable();
        }
    }
}