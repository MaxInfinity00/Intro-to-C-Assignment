using TMPro;
using UnityEngine;

namespace IntroAssignment {
    public class AmmoIndicator : MonoBehaviour {
        public TextMeshProUGUI ammoText;

        public static AmmoIndicator instance;

        private void Awake() {
            instance = this;
        }

        public void UpdateAmmo(int ammo) {
            ammoText.text = ammo.ToString();
        }

        public void HideAmmo() {
            ammoText.text = "";
        }
    }
}