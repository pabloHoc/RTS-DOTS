using RTS.Input;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace RTS.Building
{
    public partial struct BuildingPositioningSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<InputComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var input = SystemAPI.GetSingleton<InputComponent>();

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var job = new PositionBuildingJob
            {
                Ecb = ecb.AsParallelWriter(),
                BuildingPosition = input.CursorWorldPosition,
                BuildBuilding = input.PrimaryActionPressed,
                CancelBuilding = input.CancelActionPressed
            };

            state.Dependency = job.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
        
        [WithAll(typeof(BuildingComponent), typeof(BuildingPositioning))]
        [BurstCompile]
        private partial struct PositionBuildingJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter Ecb;
            public float3 BuildingPosition;
            public bool BuildBuilding;
            public bool CancelBuilding;

            [BurstCompile]
            public void Execute(
                [ChunkIndexInQuery] int chunkIndex,
                Entity entity,
                ref LocalTransform transform,
                in WorldRenderBounds renderBounds)
            {
                transform.Position = BuildingPosition;
                transform.Position.y = renderBounds.Value.Center.y;

                if (BuildBuilding)
                {
                    Ecb.RemoveComponent<BuildingPositioning>(chunkIndex, entity);
                }

                if (CancelBuilding)
                {
                    Ecb.DestroyEntity(chunkIndex, entity);
                }
            }
        }
    }
}