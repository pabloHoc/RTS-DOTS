using RTS.Building;
using RTS.GameState;
using RTS.Input;
using RTS.SystemGroups;
using RTS.UI;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace RTS.Gameplay.Building
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct BuildingSystem : ISystem
    {
        private float _currentBuildingRotation;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildingConfigComponent>();
            state.RequireForUpdate<GameStateComponent>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<InputComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var gameState = SystemAPI.GetSingletonEntity<GameStateComponent>();
            var buildingConfig = SystemAPI.GetSingletonEntity<BuildingConfigComponent>();
            var buildingDataBuffer = SystemAPI.GetBuffer<BuildingDataBuffer>(buildingConfig);
            
            if (GameUI.Instance.BuildButtonClicked && !SystemAPI.IsComponentEnabled<BuildModeTag>(gameState))
            {
                Debug.Log("Build mode entered");
                SystemAPI.SetComponentEnabled<BuildModeTag>(gameState, true);
                state.EntityManager.Instantiate(buildingDataBuffer[0].Prefab);
            }

            if (SystemAPI.IsComponentEnabled<BuildModeTag>(gameState))
            {
                var input = SystemAPI.GetSingleton<InputComponent>();
                
                var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
                var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

                _currentBuildingRotation += input.ScrollAmount * 2f;
                
                var job = new BuildBuildingJob
                {
                    Ecb = ecb.AsParallelWriter(),
                    Building = buildingDataBuffer[0].Prefab,
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
        
        [WithAll(typeof(BuildingComponent), typeof(BuildingPositioning))]
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
                    
                    Ecb.RemoveComponent<BuildingPositioning>(index, instantiatedBuilding);
                }

                if (CancelBuilding)
                {
                    Ecb.DestroyEntity(index, entity);
                }
            }
        }
    }
}