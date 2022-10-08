using UnityEngine;

namespace IntroAssignment {
    
    public class LayerOfDeath : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision) {
            collision.collider.GetComponent<Player>()?.Die();
        }
    }
}
