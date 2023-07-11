using RTS.Common;
using RTS.Gameplay.Resources;
using RTS.SystemGroups;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace RTS.Gameplay.Buildings
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    [UpdateAfter(typeof(BuildingConstructionSystem))]
    public partial struct BuildingCostSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            var buildingDatabase = SystemAPI.GetSingleton<BuildingDatabaseSingleton>().Data;

            var job = new AddBuildingCostJob
            {
                Ecb = ecb.AsParallelWriter(),
                BuildingDatabase = buildingDatabase
            };
            
            state.Dependency = job.ScheduleParallel(state.Dependency);
            state.Dependency.Complete();
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }

    [WithAll(typeof(BuildingComponent), typeof(EntityCreatedTag))]
    public partial struct AddBuildingCostJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        public BlobAssetReference<BuildingsData> BuildingDatabase;

        public void Execute(
            [ChunkIndexInQuery] int index,
            in EntityOwnerComponent owner,
            in BuildingComponent building
        )
        {
            ref var buildingData = ref BuildingDatabase.Value.Buildings[building.Index];

            for (var i = 0; i < buildingData.Cost.Length; i++)
            {
                var resource = buildingData.Cost[i];
;               Ecb.AppendToBuffer(index, owner.Entity, new ResourceBufferElement
                {
                    Name = resource.Name,
                    Value = -resource.Value,
                    Type = ResourceType.AdditiveModifier
                });
            }
        }
    }
}