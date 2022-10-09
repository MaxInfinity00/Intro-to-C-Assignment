using System;
using UnityEngine;

namespace IntroAssignment {
    [RequireComponent(typeof(Collider))]
    public class Pickup : MonoBehaviour {
        [SerializeField]private PickupType _pickupType;
        private Transform _transform;
        [SerializeField] private float _rotationSpeed = 5f;

        private void Awake() {
            _transform = transform;
        }

        private void OnCollisionEnter(Collision collision) {
            Player player = collision.body.GetComponent<Player>();
            if (player != null) {
                switch (_pickupType) {
                    case PickupType.Heal:
                        player.Heal(20);
                        break;
                    
                    case PickupType.Time:
                        GameManager.instance.AddToTimer(10);
                        break;
                    
                    case PickupType.Ammo:
                        player.currentWeapon.currentAmmo += 2;
                        break;
                }
                Destroy(gameObject);
            }
        }

        private void Update() {
            _transform.Rotate(Vector3.up,_rotationSpeed);
        }
    }

    public enum PickupType {
        Heal,
        Time,
        Ammo
    }
}