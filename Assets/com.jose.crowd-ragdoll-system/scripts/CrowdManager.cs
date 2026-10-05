using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace CrowdRagdollSystem
{
    public class CrowdManager : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float moveSpeed = 3.5f;

        private TransformAccessArray transformAccessArray;
        private NativeArray<bool> isRagdollActive;

        public void RegisterEnemies(Transform[] enemyTransforms)
        {
            transformAccessArray = new TransformAccessArray(enemyTransforms);
            isRagdollActive = new NativeArray<bool>(enemyTransforms.Length, Allocator.Persistent);

            for (int i = 0; i < enemyTransforms.Length; i++)
            {
                if (enemyTransforms[i].TryGetComponent<ModularRagdoll>(out var ragdoll))
                {
                    ragdoll.Initialize(this, i);
                }
            }
        }
        
        public void SetEnemyRagdollState(int index, bool isRagdoll)
        {
            if (isRagdollActive.IsCreated && index >= 0 && index < isRagdollActive.Length)
            {
                isRagdollActive[index] = isRagdoll;
            }
        }

        private void Update()
        {
            if (!transformAccessArray.isCreated) return;

            ChaseJob chaseJob = new ChaseJob
            {
                PlayerPosition = playerTransform.position,
                MoveSpeed = moveSpeed,
                DeltaTime = Time.deltaTime,
                IsRagdollActive = isRagdollActive
            };

            JobHandle jobHandle = chaseJob.Schedule(transformAccessArray);
            jobHandle.Complete();
        }

        private void OnDestroy()
        {
            if (transformAccessArray.isCreated) transformAccessArray.Dispose();
            if (isRagdollActive.IsCreated) isRagdollActive.Dispose();
        }

        [BurstCompile]
        private struct ChaseJob : IJobParallelForTransform
        {
            public float3 PlayerPosition;
            public float MoveSpeed;
            public float DeltaTime;
            [ReadOnly] public NativeArray<bool> IsRagdollActive;

            public void Execute(int index, TransformAccess transform)
            {
                if (IsRagdollActive[index]) return;

                float3 currentPos = transform.position;
                float3 dir = math.normalize(PlayerPosition - currentPos);
                dir.y = 0; // Lock to ground plane

                transform.position = currentPos + dir * MoveSpeed * DeltaTime;
                
                if (math.lengthsq(dir) > 0.001f)
                {
                    transform.rotation = quaternion.LookRotation(dir, math.up());
                }
            }
        }
    }
}
