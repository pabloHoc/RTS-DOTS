using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace RTS.Common
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct CleanUpSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = SystemAPI.QueryBuilder().WithAll<EntityCreatedTag>().Build().ToEntityArray(Allocator.Temp);
            state.EntityManager.RemoveComponent<EntityCreatedTag>(entities);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}