using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Freya;

namespace IntroAssignment {
    public class Player : MonoBehaviour, Controls.IPlayerActions {

        public float movementSpeed;
        public float jumpForce;
        public float groundCheckHeight;
        
        private int _health;
        public static int _maxHealth;
        public bool isAlive = true;
        private HealthBar _healthBar;

        public List<Weapon> weapons;
        private int _currentWeaponIndex;
    
        [HideInInspector]public Transform playerTransform;
        private Rigidbody _rigidbody;

        private Controls controls;

        private Transform _cameraTransform;

        private bool isAiming;
        [SerializeField] private MeshRenderer Glasses;

        public Weapon currentWeapon {
            get => weapons[_currentWeaponIndex];
        }

        void Awake() {
            isAlive = true;
            playerTransform = GetComponent<Transform>();
            _rigidbody = GetComponent<Rigidbody>();
            
            _cameraTransform = Camera.main.transform;
            
            controls = new Controls();
            controls.Player.SetCallbacks(this);

            foreach (Weapon weapon in weapons) {
                weapon.SetWeapon();
            }
        }

        public void SetHealthBar(HealthBar healthBar) {
            _health = _maxHealth;
            this._healthBar = healthBar;
            healthBar.UpdateUI(_health,_maxHealth);
        }

        private void Start() {
            GameManager.instance?.AddPlayer(this);
            if (_healthBar != null) {
                _health = _maxHealth;
                _healthBar.UpdateUI(_health,_maxHealth);
            }
            // throw new NotImplementedException();
        }

        private void FixedUpdate() {
            if (GameManager.instance.controlState != ControlState.On) return;
            if (isAiming && weapons.Count > 0) {
                bool x = Physics.Raycast(playerTransform.position,_cameraTransform.forward,out RaycastHit hit,10);
                weapons[_currentWeaponIndex].Look(x ? hit.point : playerTransform.position + _cameraTransform.forward * 10);
            }
            Vector2 movement = controls.Player.Move.ReadValue<Vector2>();
            if(movement != Vector2.zero)
                PlayerMove(movement);
        }

        public void OnMove(InputAction.CallbackContext _){}

        public void PlayerMove(Vector2 movement) {
            // Debug.Log(playerTransform.position + " " + targetPosition);
            if (!isAiming) {
                Vector3 targetPosition = (_cameraTransform.right.FlattenY() * movement.x) +
                                         (_cameraTransform.forward.FlattenY() * movement.y);
                playerTransform.LookAt(playerTransform.position + targetPosition);
                playerTransform.Translate(Vector3.forward * movementSpeed);
            }
            else {
                playerTransform.Translate(new Vector3(movement.x, 0, movement.y) * movementSpeed);
            }
        }

        public void OnJump(InputAction.CallbackContext _) {
            if(GameManager.instance.controlState == ControlState.On && GroundCheck())
                _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        private bool GroundCheck() {
            return Physics.Raycast(playerTransform.position, Vector3.down, groundCheckHeight);
        }

        public void OnSwitchWeapon(InputAction.CallbackContext context) {
            if (!context.performed || weapons.Count <= 0) return;
            weapons[_currentWeaponIndex].Disable();
            
            while (true) {
                _currentWeaponIndex = (_currentWeaponIndex + (int)context.ReadValue<float>()) % weapons.Count;
                if (_currentWeaponIndex == -1) _currentWeaponIndex = weapons.Count-1;
                if (weapons[_currentWeaponIndex].currentAmmo > 0) break;
            }
            
            weapons[_currentWeaponIndex].Enable();
            
            if (weapons.Count > 0) {
                AmmoIndicator.instance.UpdateAmmo(weapons[_currentWeaponIndex].currentAmmo);
            }
            else {
                AmmoIndicator.instance.HideAmmo();
            }
        }

        public void OnAim(InputAction.CallbackContext context) {
            if (context.performed) {
                isAiming = true;
                Glasses.enabled = false;
            }
            else if (context.canceled) {
                isAiming = false;
                Glasses.enabled = true;
            }
        }

        public void OnFire(InputAction.CallbackContext context) {
            if (!context.performed) return;
            if (weapons.Count > 0 && weapons[_currentWeaponIndex].currentAmmo > 0) {
                weapons[_currentWeaponIndex].Fire();
                GameManager.instance.NextTurn();
                AmmoIndicator.instance.UpdateAmmo(weapons[_currentWeaponIndex].currentAmmo);
            }
        }

        public void OnStartTurn() {
            SetControls(true);
            _healthBar.StartTurn();
            if (weapons.Count > 0) {
                AmmoIndicator.instance.UpdateAmmo(weapons[_currentWeaponIndex].currentAmmo);
            }
            else {
                AmmoIndicator.instance.HideAmmo();
            }
        }

        public void OnEndTurn() {
            Glasses.enabled = true;
            SetControls(false);
            _healthBar.EndTurn();
            
        }
        
        ///<summary> Heal the Player </summary>
        public void Heal(int healAmount) {
            _health += healAmount;
            _health.Clamp(0,_maxHealth);
            _healthBar.UpdateUI(_health,_maxHealth);
        }

        ///<summary> Damage the Player </summary>
        public void TakeDamage(int damage) {
            _health -= damage;
            if (_health <= 0)  Die();
            else _health.Clamp(0,_maxHealth);
            _healthBar.UpdateUI(_health,_maxHealth);
        }

        public void Die() {
            isAlive = false;
            gameObject.SetActive(false);
            _healthBar.Die();
            GameManager.instance.GameOver();
        }

        public void SetControls(bool b) {
            if (b) controls.Player.Enable();
            else controls.Player.Disable();
        }
    }
}