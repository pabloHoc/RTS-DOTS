using RTS.GameState;
using RTS.Input;
using RTS.SystemGroups;
using RTS.UI;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace RTS.Gameplay.Buildings
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct BuildingSystem : ISystem
    {
        private float _currentBuildingRotation;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildingConfigSingleton>();
            state.RequireForUpdate<GameStateSingleton>();
            state.RequireForUpdate<InputSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var gameState = SystemAPI.GetSingletonEntity<GameStateSingleton>();
            var buildingConfig = SystemAPI.GetSingletonEntity<BuildingConfigSingleton>();
            var buildingDataBuffer = SystemAPI.GetBuffer<BuildingDataBufferElement>(buildingConfig);
            
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            
            if (GameUI.Instance.BuildButtonClicked && !SystemAPI.IsComponentEnabled<BuildModeTag>(gameState))
            {
                SystemAPI.SetComponentEnabled<BuildModeTag>(gameState, true);
                var buildingEntity = state.EntityManager.Instantiate(buildingDataBuffer[GameUI.Instance.BuildingIndex].Prefab);
                ecb.AddComponent<BuildingPositioningTag>(buildingEntity);
            }
            
            if (SystemAPI.IsComponentEnabled<BuildModeTag>(gameState))
            {
                var input = SystemAPI.GetSingleton<InputSingleton>();
                
                var building = buildingDataBuffer[GameUI.Instance.BuildingIndex].Prefab;

                _currentBuildingRotation += input.ScrollAmount * 2f;
                
                var job = new BuildBuildingJob
                {
                    Ecb = ecb.AsParallelWriter(),
                    Building = building,
                    BuildingPosition = input.CursorWorldPosition,
                    BuildingRotation = _currentBuildingRotation,
                    BuildBuilding = input.WasPrimaryActionPressedThisFrame,
                    CancelBuilding = input.IsCancelActionPressed
                };

                if (input.IsCancelActionPressed)
                {
                    SystemAPI.SetComponentEnabled<BuildModeTag>(gameState, false);
                }

                state.Dependency = job.ScheduleParallel(state.Dependency);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
        
        [WithAll(typeof(BuildingComponent), typeof(BuildingPositioningTag))]
        [BurstCompile]
        private partial struct BuildBuildingJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter Ecb;
            public Entity Building;
            public float3 BuildingPosition;
            public bool BuildBuilding;
            public bool CancelBuilding;
            public float BuildingRotation;

            [BurstCompile]
            public void Execute(
                [ChunkIndexInQuery] int index,
                Entity entity,
                ref LocalTransform transform,
                in WorldRenderBounds renderBounds)
            {
                transform.Position = BuildingPosition;
                transform.Position.y = renderBounds.Value.Center.y;
                transform.Rotation = quaternion.RotateY(BuildingRotation); 
                
                if (BuildBuilding)
                {
                    var instantiatedBuilding = Ecb.Instantiate(index, Building);
                    Ecb.AddComponent<LocalTransform>(index, instantiatedBuilding);
                    Ecb.SetComponent(index, instantiatedBuilding, transform);
                }

                if (CancelBuilding)
                {
                    Ecb.DestroyEntity(index, entity);
                }
            }
        }
    }
}