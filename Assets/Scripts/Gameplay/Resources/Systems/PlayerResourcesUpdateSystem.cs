using RTS.Gameplay.Players;
using RTS.SystemGroups;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace RTS.Gameplay.Resources
{
    [UpdateInGroup(typeof(GameplaySystemGroup), OrderLast = true)]
    [BurstCompile]
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

            var job = new UpdatePlayerResourcesJob
            {
                Ecb = ecb.AsParallelWriter(),
                ResourceBufferLookup = _resourceBufferLookup
            };

            
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
        [ReadOnly] public BufferLookup<ResourceBufferElement> ResourceBufferLookup;

        public void Execute(                
            [ChunkIndexInQuery] int index,
            Entity player
            )
        {
            var stored = new UnsafeHashMap<FixedString32Bytes, int>(4, Allocator.TempJob);
            var additiveModifiers = new UnsafeHashMap<FixedString32Bytes, int>(10, Allocator.TempJob);
            var resourceBuffer = ResourceBufferLookup[player];
            
            foreach (var resource in resourceBuffer)
            {
                switch (resource.Type)
                {
                    case ResourceType.Stored:
                        stored.TryAdd(resource.Name, resource.Value);
                        break;
                    // TODO: add a util for this
                    case ResourceType.AdditiveModifier:
                    {
                        if (!additiveModifiers.TryAdd(resource.Name, resource.Value))
                        {
                            var currentValue = additiveModifiers[resource.Name];
                            additiveModifiers.Remove(resource.Name);
                            additiveModifiers.TryAdd(resource.Name, currentValue + resource.Value);
                        }
                        break;
                    }
                }
            }

            foreach (var entry in additiveModifiers)
            {
                var currentValue = stored[entry.Key];
                stored.Remove(entry.Key);
                stored.Add(entry.Key, currentValue + entry.Value);
            }

            Ecb.SetBuffer<ResourceBufferElement>(index, player);
            
            // Reset buffer
            Ecb.SetBuffer<ResourceBufferElement>(index, player);
            
            foreach (var entry in stored)
            {
                Ecb.AppendToBuffer(index, player, new ResourceBufferElement
                {
                    Name = entry.Key,
                    Value = entry.Value,
                    Type = ResourceType.Stored
                });
            }
        }
    }
}