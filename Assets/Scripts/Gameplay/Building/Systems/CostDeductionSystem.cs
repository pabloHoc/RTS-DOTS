using RTS.Authoring.Gameplay.Unit;
using RTS.Common;
using RTS.Gameplay.Resources;
using RTS.Gameplay.Units;
using RTS.SystemGroups;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace RTS.Gameplay.Building
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    [UpdateAfter(typeof(BuildingSystem))]
    public partial struct CostDeductionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            var buildingDatabase = SystemAPI.GetSingleton<UnitDatabaseSingleton>().Data;

            var job = new AddBuildingCostJob
            {
                Ecb = ecb.AsParallelWriter(),
                UnitDatabase = buildingDatabase
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

    [WithAll(typeof(UnitComponent), typeof(EntityCreatedTag))]
    public partial struct AddBuildingCostJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        public BlobAssetReference<UnitsBlobAsset> UnitDatabase;

        public void Execute(
            [ChunkIndexInQuery] int index,
            in EntityOwnerComponent owner,
            in UnitComponent unit
        )
        {
            ref var unitData = ref UnitDatabase.Value.Units[unit.DatabaseIndex];
            Debug.Log("HERE");
            for (var i = 0; i < unitData.Cost.Length; i++)
            {
                var resource = unitData.Cost[i];
                Debug.Log($"Cost {resource.Name} {resource.Value}")
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