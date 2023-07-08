using System.ComponentModel;
using RTS.Gameplay.Player;
using RTS.SystemGroups;
using Unity.Burst;
using Unity.Entities;

namespace RTS.Gameplay.Resources
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct PlayerResourcesUpdateSystem : ISystem
    {
        private BufferLookup<ResourceBufferElement> _resourceBufferLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _resourceBufferLookup = state.GetBufferLookup<ResourceBufferElement>(true);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _resourceBufferLookup.Update(ref state);

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var job = new UpdatePlayerResourcesJob();

            state.Dependency = job.ScheduleParallel(state.Dependency);

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
    
    [WithAll(typeof(PlayerComponent))]
    [BurstCompile]
    public partial struct UpdatePlayerResourcesJob : IJobEntity {

        public EntityCommandBuffer.ParallelWriter Ecb;

        public void Execute(                
            [ChunkIndexInQuery] int index,
            Entity entity,
            DynamicBuffer<ResourceBufferElement> resourceBuffer
            )
        {
            for (int i = 0; i < resourceBuffer.Length; i++)
            {
                var resource = resourceBuffer[i];
                resource.Quantity++;
                resourceBuffer[i] = resource;
            }
        }
    }
}