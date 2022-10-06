using UnityEngine;

namespace IntroAssignment {
    [RequireComponent(typeof(Collider))]
    public class Pickup : MonoBehaviour {
        
        private void OnCollisionEnter(Collision collision) {
            if (collision.body.GetComponent<Player>() != null) {
                
            }
        }
    }
}