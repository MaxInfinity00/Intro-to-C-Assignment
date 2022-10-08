using UnityEngine;

namespace IntroAssignment {
    public class RaycastWeapon : Weapon {

        public int damage;
        public float range;

        public override void Fire() {
            currentAmmo--;
            if(Physics.Raycast(_weaponTransform.position,_weaponTransform.forward,out RaycastHit hit,range)) {
                Player player = hit.collider.GetComponent<Player>();
                player?.TakeDamage(damage);
            }
        }
    }
}