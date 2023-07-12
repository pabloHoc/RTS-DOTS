using RTS.Common;
using RTS.Gameplay.Resources;
using RTS.Gameplay.Units;
using RTS.SystemGroups;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace RTS.Gameplay.UnitBuilding
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    [UpdateAfter(typeof(UnitConstructionSystem))]
    public partial struct UnitCostDeductionSystem : ISystem
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
            ref var buildingData = ref UnitDatabase.Value.Units[unit.UnitId];

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