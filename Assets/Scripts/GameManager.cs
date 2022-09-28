using System;
using UnityEngine;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine.InputSystem;

namespace IntroAssignment {
    public class GameManager : MonoBehaviour {
        private int _currentPlayer = 0;
        public static List<Player> players;
        
        
        private Controls _controls;

        private void Awake() {
            players = new List<Player>();
            _controls = new Controls();
            _controls.Player.Enable();

            _controls.Manager.NextTurn.performed += NextTurn;
            // _controls.Player.Jump.performed += PlayerJump;
        }

        private void Start() {
            // NextTurn(new InputAction.CallbackContext());
        }

        // private void FixedUpdate() {
        //     if (players.Count <= 0) return;
        //     Vector2 movement = _controls.Player.Move.ReadValue<Vector2>();
        //     players[_currentPlayer].OnMove(movement);
        //     // CinemachineFreeLook
        // }
        //
        // private void PlayerJump(InputAction.CallbackContext context) {
        //     players[_currentPlayer].OnJump();
        // }

        void NextTurn(InputAction.CallbackContext context) {
            // _currentPlayer = (_currentPlayer + 1) % players.Count;
            // CameraFollow.SetTarget(players[_currentPlayer].playerTransform);
        }

    }
}