using CrowdRagdollSystem;
using UnityEngine;

public class TriggerRiceived : MonoBehaviour
{
    [Header("Force Settings")]
    public float minForce = 10f;
    public float maxForce = 50f;

    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other.gameObject.name} entered the mesh trigger zone!");
        
        Vector3 randomDirection = Random.onUnitSphere;
        float randomMagnitude = Random.Range(minForce, maxForce);
        Vector3 forceVector = randomDirection * randomMagnitude;

        // 2. Generate the Hit Point Vector (A random point on the surface of the entering collider)
        Vector3 hitPoint = other.ClosestPoint(other.transform.position + Random.onUnitSphere * 10f);
        other.GetComponent<ModularRagdoll>().TriggerRagdoll(forceVector,hitPoint);
    }

    // Fires continuously while the other collider stays inside
    private void OnTriggerStay(Collider other)
    {
        // Add logic for continuous presence if needed
    }

    // Fires once when the other collider exits the trigger zone
    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"{other.gameObject.name} left the mesh trigger zone.");
    }
}
