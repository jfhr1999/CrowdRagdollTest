using System.Collections.Generic;
using UnityEngine;

namespace CrowdRagdollSystem
{
    public class BenchmarkSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private CrowdManager crowdManager;
        [SerializeField] private int initialSpawnCount = 150;
        [SerializeField] private float spawnRadius = 30f;

        private List<ModularRagdoll> spawnedRagdolls = new List<ModularRagdoll>();

        private void Start()
        {
            SpawnCrowd(initialSpawnCount);
        }

        public void SpawnCrowd(int count)
        {
            Transform[] transforms = new Transform[count];

            for (int i = 0; i < count; i++)
            {
                Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
                Vector3 spawnPos = new Vector3(randomCircle.x, 0f, randomCircle.y);

                GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                transforms[i] = enemyObj.transform;
                spawnedRagdolls.Add(enemyObj.GetComponent<ModularRagdoll>());
            }

            crowdManager.RegisterEnemies(transforms);
        }

        // Test helper: Call this via OnGUI or Key Press to trigger stress testing
        public void TriggerMassRagdoll(int countToKill, Vector3 hitForce)
        {
            int killLimit = Mathf.Min(countToKill, spawnedRagdolls.Count);
            for (int i = 0; i < killLimit; i++)
            {
                if (spawnedRagdolls[i] != null)
                {
                    spawnedRagdolls[i].TriggerRagdoll(hitForce, spawnedRagdolls[i].transform.position + Vector3.up);
                }
            }
        }
    }
}
