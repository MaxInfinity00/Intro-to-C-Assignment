using UnityEngine;

namespace IntroAssignment {
    public class Projectile : MonoBehaviour {
        [SerializeField] private int _damage;
        [SerializeField] private float lifeTime;
        [Tooltip("If set to false object will damage in lifeTime - 1")][SerializeField] private bool onContact;
        
        [Header("Exposive")]
        [SerializeField] private bool _explosive;
        [SerializeField] private float _explosiveForce;
        [SerializeField] private float _explosionRange;
        [SerializeField] private GameObject _explosionFX;
        

        [Header("Constant Force")]
        [SerializeField] private bool _constantForce;
        [SerializeField] private float _forceDuration;
        [SerializeField] private float _force;

        private float timeSpent;
        private Transform _projectileTransform;
        private Rigidbody _projectileRigidbody;

        private string test;
        private bool collided;
        private bool exploded;

        private void Awake() {
            Spawn();
        }

        private void Spawn() {
            collided = false;
            exploded = false;
            if (_constantForce)
                _projectileRigidbody = GetComponent<Rigidbody>();
            
            if (_constantForce || !onContact)
                timeSpent = 0;

            if (_constantForce || _explosive)
                _projectileTransform = transform;
            
            Destroy(gameObject,lifeTime);
        }

        private void OnCollisionEnter(Collision collision) {
            if (!onContact || collided) return;
            collided = true;

            if (_explosive) {
                Explode();
                exploded = true;
            }
            else {
                GetComponent<Renderer>().enabled = false;
                collision.collider.GetComponent<Player>()?.TakeDamage(_damage);
            }
        }
        
        public void Explode() {
            exploded = true;

            GetComponent<Renderer>().enabled = false;
            
            if (_explosionFX != null) {
                GameObject fx = Instantiate(_explosionFX, _projectileTransform.position, Quaternion.identity);
                Destroy(fx,4);
            }
            
            Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRange);
            foreach (Collider collider in colliders) {
                Player player = collider.GetComponent<Player>();
                player?.TakeDamage(_damage);
                collider.attachedRigidbody?.AddExplosionForce(_explosiveForce,_projectileTransform.position,_explosionRange);
            }
            
            Destroy(gameObject,2);
        }
        
        private void Update() {
            timeSpent += Time.deltaTime;
            if (_constantForce) {
                if (timeSpent <= _forceDuration) {
                    _projectileRigidbody.AddForce(_projectileTransform.forward * _force,ForceMode.VelocityChange);
                }
            }

            if (!onContact && !exploded) {
                if (timeSpent >= lifeTime - 1) {
                    Explode();
                }
            }
        }
    }
}