using UnityEngine;
using UnityEngine.Serialization;

namespace IntroAssignment {
    public class Weapon : MonoBehaviour {

        public Vector3 weaponPosition;
        [FormerlySerializedAs("currentammo")] public int currentAmmo;
        // public int maxAmmo;

        protected Transform _weaponTransform;

        public void SetWeapon() {
            _weaponTransform = transform;
            _weaponTransform.localPosition = weaponPosition;
        }

        public void Enable() {
            gameObject.SetActive(true);
        }

        public void Disable() {
            gameObject.SetActive(false);
        }

        public void Look(Vector3 target) {
            _weaponTransform.LookAt(target);
        }

        public virtual void Fire() { }
    }
}