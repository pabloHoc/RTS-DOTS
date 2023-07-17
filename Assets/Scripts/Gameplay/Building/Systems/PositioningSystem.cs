using System.ComponentModel;
using RTS.Authoring.Gameplay.Unit;
using RTS.Common;
using RTS.Gameplay.Players.Singletons;
using RTS.Gameplay.Units;
using RTS.GameState;
using RTS.Input;
using RTS.SystemGroups;
using RTS.UI;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace RTS.Gameplay.Building
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct PositioningSystem : ISystem
    {
        private float _currentBuildingRotation;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var gameState = SystemAPI.GetSingletonEntity<GameStateSingleton>();
            var input = SystemAPI.GetSingleton<InputSingleton>();
            var player = SystemAPI.GetSingletonEntity<HumanPlayerSingleton>();
            
            var unitRepository = SystemAPI.GetSingletonEntity<UnitDatabaseSingleton>();
            var unitEntitiesBuffer = SystemAPI.GetBuffer<EntityBufferElement>(unitRepository);
            var unitToBuild = unitEntitiesBuffer[GameUI.Instance.BuildingIndex].Entity;

            if (SystemAPI.IsComponentEnabled<BuildModeTag>(gameState))
            {
                _currentBuildingRotation += input.ScrollAmount * 2f;
     
                var job = new BuildBuildingJob
                {
                    Ecb = ecb.AsParallelWriter(),
                    Player = player,
                    Unit = unitToBuild,
                    UnitPosition = input.CursorWorldPosition,
                    UnitRotation = _currentBuildingRotation,
                    BuildUnit = input.WasPrimaryActionPressedThisFrame,
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
    }
    
    [WithAll(typeof(PositioningTag))]
    [BurstCompile]
    public partial struct BuildBuildingJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        [ReadOnly(true)] public Entity Player;
        [ReadOnly(true)] public Entity Unit;
        [ReadOnly(true)] public float3 UnitPosition;
        [ReadOnly(true)] public bool BuildUnit;
        [ReadOnly(true)] public bool CancelBuilding;
        [ReadOnly(true)] public float UnitRotation;

        [BurstCompile]
        public void Execute(
            [ChunkIndexInQuery] int index,
            Entity entity,
            ref LocalTransform transform,
            in WorldRenderBounds renderBounds)
        {
            transform.Position = UnitPosition;
            transform.Position.y = renderBounds.Value.Center.y;
            transform.Rotation = quaternion.RotateY(UnitRotation); 
            
            if (BuildUnit)
            {
                var instantiatedUnit = Ecb.Instantiate(index, Unit);
                // Add transform
                Ecb.AddComponent<LocalTransform>(index, instantiatedUnit);
                Ecb.SetComponent(index, instantiatedUnit, transform);
                // Add ownership
                Ecb.AddComponent<EntityOwnerComponent>(index, instantiatedUnit);
                Ecb.SetComponent(index, instantiatedUnit, new EntityOwnerComponent
                {
                    Entity = Player
                });
                // Add created tag
                Ecb.AddComponent<EntityCreatedTag>(index, instantiatedUnit);
            }

            if (CancelBuilding)
            {
                Ecb.DestroyEntity(index, entity);
            }
        }
    }
}