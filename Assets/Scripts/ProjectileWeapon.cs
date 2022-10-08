using UnityEngine;
using UnityEngine.Serialization;

namespace IntroAssignment {
    public class ProjectileWeapon : Weapon {

        public GameObject projectile;
        public float shootForce;

        [FormerlySerializedAs("SpawnPoint")] public Transform spawnPoint;

        public override void Fire() {
            currentAmmo--;
            // GameObject firedProjectile = Instantiate(projectile,projectileSpawnPosition,transform.rotation);
            GameObject firedProjectile = Instantiate(projectile, spawnPoint.position, _weaponTransform.rotation);
            firedProjectile.GetComponent<Rigidbody>().AddForce(_weaponTransform.forward*shootForce,ForceMode.VelocityChange);
        }
    }
}