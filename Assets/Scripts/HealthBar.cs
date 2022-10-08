using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IntroAssignment {
    public class HealthBar : MonoBehaviour {
        public Image healthBarImage;
        public TextMeshProUGUI healthText;

        public void UpdateUI(int health, int maxHealth) {
            healthBarImage.fillAmount = (float)health / maxHealth;
            healthText.text = health.ToString();
        }

        public void Die() {
            gameObject.SetActive(false);
        }

        public void StartTurn() {
            healthBarImage.color = Color.green;
        }

        public void EndTurn() {
            healthBarImage.color = Color.red;
        }
    }
}