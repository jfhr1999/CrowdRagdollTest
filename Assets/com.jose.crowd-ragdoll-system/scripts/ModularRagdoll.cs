using UnityEngine;

namespace CrowdRagdollSystem
{
    public class ModularRagdoll : MonoBehaviour
    {
        [Header("Ragdoll References")]
        [SerializeField] private Animator animator;
        [SerializeField] private Collider mainCollider;
        [SerializeField] private Rigidbody mainRigidbody;
        
        private Rigidbody[] ragdollRigidbodies;
        private Collider[] ragdollColliders;
        
        private CrowdManager crowdManager;
        private int enemyIndex = -1;
        
        [Header("Force Settings")]
        public float minForce = 20f;
        public float maxForce = 100f;
        private Vector3 forceVector;
        private Vector3 hitPoint;
        
        public void Initialize(CrowdManager manager, int index)
        {
            crowdManager = manager;
            enemyIndex = index;
        }

        private void Awake()
        {
            ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            ragdollColliders = GetComponentsInChildren<Collider>();

            // Ensure ragdoll parts start disabled
            SetRagdollState(false);
            
            Vector3 randomDirection = Vector3.zero;
            randomDirection.x = Random.Range(minForce, maxForce);
            randomDirection.y = maxForce;
            randomDirection.z = Random.Range(minForce, maxForce);
            
            float randomMagnitude = Random.Range(minForce, maxForce);
            forceVector = randomDirection * randomMagnitude;

            // 2. Generate the Hit Point Vector (A random point on the surface of the entering collider)
            hitPoint = transform.position + Random.onUnitSphere * 10f;

            float randomTime = Random.Range(1, 4);
            Invoke(nameof(FireTest),randomTime);
        }

        public void FireTest()
        {
            TriggerRagdoll(forceVector,hitPoint);
        }

        public void SetRagdollState(bool isRagdoll)
        {
            // Toggle animator and main capsule
            if (animator) animator.enabled = !isRagdoll;
            if (mainCollider) mainCollider.enabled = !isRagdoll;
            
            if (mainRigidbody)
            {
                mainRigidbody.isKinematic = true;
                mainRigidbody.detectCollisions = false;
            }
            
            if (isRagdoll && ragdollRigidbodies.Length > 0)
            {
                // Unparent the Hips (first bone in hierarchy) to the scene root
                ragdollRigidbodies[0].transform.SetParent(null);
            }

            // Toggle individual bone physics
            for (int i = 0; i < ragdollRigidbodies.Length; i++)
            {
                if (ragdollRigidbodies[i] == mainRigidbody) continue;

                ragdollRigidbodies[i].isKinematic = !isRagdoll;
                ragdollRigidbodies[i].detectCollisions = isRagdoll;
            }

            for (int i = 0; i < ragdollColliders.Length; i++)
            {
                if (ragdollColliders[i] == mainCollider) continue;

                ragdollColliders[i].enabled = isRagdoll;
            }
            
            if (isRagdoll && crowdManager != null && enemyIndex >= 0)
            {
                crowdManager.SetEnemyRagdollState(enemyIndex, true);
            }
        }

        public void TriggerRagdoll(Vector3 force, Vector3 hitPoint)
        {
            SetRagdollState(true);

            // Apply directional force to closest body part
            Rigidbody closestBone = GetClosestBone(hitPoint);
            if (closestBone != null)
            {
                closestBone.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
            }
        }

        private Rigidbody GetClosestBone(Vector3 point)
        {
            Rigidbody closest = null;
            float minSqDistance = float.MaxValue;

            for (int i = 0; i < ragdollRigidbodies.Length; i++)
            {
                if (ragdollRigidbodies[i] == mainRigidbody) continue;

                float sqDist = (ragdollRigidbodies[i].position - point).sqrMagnitude;
                if (sqDist < minSqDistance)
                {
                    minSqDistance = sqDist;
                    closest = ragdollRigidbodies[i];
                }
            }
            return closest;
        }
    }
}
