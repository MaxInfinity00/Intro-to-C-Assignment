using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Freya;
using UnityEngine.InputSystem.Controls;

namespace IntroAssignment {
    public class Player : MonoBehaviour, Controls.IPlayerActions {

        public float movementSpeed;
        public float jumpForce;
        public float groundCheckHeight;
        
        private float _health;
        private float _maxHealth;
    
        private Transform _playerTransform;
        private Rigidbody _rigidbody;

        private Controls controls;

        [SerializeField]private Transform _cameraTransform;

        void Awake(){
            _playerTransform = GetComponent<Transform>();
            _rigidbody = GetComponent<Rigidbody>();

            _cameraTransform = Camera.main.transform;
            
            controls = new Controls();
            controls.Player.Enable();
            controls.Player.SetCallbacks(this);
            
            // GameManager.players.Add(this);
        }

        private void FixedUpdate() {
            Vector2 movement = controls.Player.Move.ReadValue<Vector2>();
            if(movement != Vector2.zero)
                PlayerMove(controls.Player.Move.ReadValue<Vector2>());
        }

        public void OnMove(InputAction.CallbackContext _) {
            // PlayerMove(context.ReadValue<Vector2>());
        }

        public void PlayerMove(Vector2 movement) {
            Vector3 targetPosition = (_cameraTransform.right.FlattenY() * movement.x) +
                                     (_cameraTransform.forward.FlattenY() * movement.y);
            // Debug.Log(playerTransform.position + " " + targetPosition);
            _playerTransform.LookAt(_playerTransform.position + targetPosition);
            _playerTransform.Translate(Vector3.forward * movementSpeed);
        }

        public void OnJump(InputAction.CallbackContext _) {
            if(GroundCheck())
                _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        private bool GroundCheck() {
            return Physics.Raycast(_playerTransform.position, Vector3.down, groundCheckHeight);
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
            _health.Clamp(0,_maxHealth);
        }
    }
}