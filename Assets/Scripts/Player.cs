using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Freya;

namespace IntroAssignment {
    public class Player : MonoBehaviour, Controls.IPlayerActions {

        public float movementSpeed;
        public float rotationSensitivity;
    
        public Transform playerTransform;
        private Rigidbody _rigidbody;

        private Controls controls;

        [SerializeField]private Transform _cameraTransform;

        void Awake(){
            playerTransform = GetComponent<Transform>();
            _rigidbody = GetComponent<Rigidbody>();

            _cameraTransform = Camera.main.transform;
            
            controls = new Controls();
            controls.Player.Enable();
            controls.Player.SetCallbacks(this);
            
            // GameManager.players.Add(this);
        }

        private void FixedUpdate() {
            PlayerMove(controls.Player.Move.ReadValue<Vector2>());
        }

        public void OnMove(InputAction.CallbackContext context) {
            PlayerMove(context.ReadValue<Vector2>());
        }
        public void PlayerMove(Vector2 movement){
            // Debug.Log(movement);
            // _rigidbody.AddForce(new Vector3(movement.x,0,movement.y)*movementSpeed,ForceMode.Force);
            Vector3 targetPosition = (_cameraTransform.right.FlattenY() * movement.x * movementSpeed) +
                                     (_cameraTransform.forward.FlattenY() * movement.y * movementSpeed);
            
            playerTransform.LookAt(playerTransform.position + targetPosition);
            playerTransform.Translate(targetPosition.x,0,targetPosition.z);
            _rigidbody.MovePosition(targetPosition.FlattenY());
            // _rigidbody.MovePosition();
        }
        
        // public void PlayerMove(Vector2 movement){
        //     Debug.Log(movement);
        //     // _rigidbody.AddForce(new Vector3(movement.x,0,movement.y)*movementSpeed,ForceMode.Force);
        //     Vector3 targetPosition = playerTransform.position +
        //                              (_cameraTransform.right* movement.x * movementSpeed) +
        //                              (_cameraTransform.forward * movement.y * movementSpeed);
        //     playerTransform.LookAt(targetPosition);
        //     playerTransform.Translate(targetPosition.x - playerTransform.position.x,0,targetPosition.z - playerTransform.position.z);
        //     // _rigidbody.MovePosition();
        // }
        

        public void OnJump(InputAction.CallbackContext context) {
            // Debug.Log("Jump");
        }

        public void OnFire(InputAction.CallbackContext context) {
            // Debug.Log("Fire");
        }
    }
}