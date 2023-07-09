using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.Jobs;

namespace RTS.Common
{    
    [UpdateAfter(typeof(TransformSystemGroup))]
    public partial struct TransformSyncingSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }
        
        public void OnUpdate(ref SystemState state)
        {
            var query = SystemAPI.QueryBuilder().WithAll<SyncTransformComponent>().Build();
            // TODO: memory leak here - check (maybe?)
            var entities = query.ToEntityArray(Allocator.TempJob);
            var transformAccessArray = new TransformAccessArray(0);

            foreach (var entity in entities)
            {
                var component = state.EntityManager.GetComponentData<SyncTransformComponent>(entity);
                transformAccessArray.Add(component.GameObject.transform);
            }

            var job = new SyncTransformJob
            {
                LocalToWorld = SystemAPI.GetComponentLookup<LocalToWorld>(),
                Entities = entities
            }.Schedule(transformAccessArray, state.Dependency);
            job.Complete();
        }

        public void OnDestroy(ref SystemState state)
        {

        }
    }
    
    [BurstCompile]
    struct SyncTransformJob : IJobParallelForTransform
    {
        [ReadOnly] public ComponentLookup<LocalToWorld> LocalToWorld;
        [ReadOnly] public NativeArray<Entity> Entities;

        public void Execute(int index, TransformAccess transform)
        {
            var ltw = LocalToWorld[Entities[index]];
            transform.position = ltw.Position;
            transform.rotation = ltw.Rotation;
        }
    }
}