using UnityEngine;

namespace IntroAssignment {
    [CreateAssetMenu()]
    public class GameSettings : ScriptableObject {
        public int seed;
        public int playerCount;
        public int turnTime;
    }
}