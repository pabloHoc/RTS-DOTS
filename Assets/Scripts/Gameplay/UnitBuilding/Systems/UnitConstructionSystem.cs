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

namespace RTS.Gameplay.UnitBuilding
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct UnitConstructionSystem : ISystem
    {
        private float _currentBuildingRotation;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<UnitDatabaseSingleton>();
            state.RequireForUpdate<GameStateSingleton>();
            state.RequireForUpdate<InputSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var gameState = SystemAPI.GetSingletonEntity<GameStateSingleton>();

            var unitDatabase = SystemAPI.GetSingleton<UnitDatabaseSingleton>().Data;
            var unitRepository = SystemAPI.GetSingletonEntity<UnitDatabaseSingleton>();
            var unitEntitiesBuffer = SystemAPI.GetBuffer<EntityBufferElement>(unitRepository);
            
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            
            var input = SystemAPI.GetSingleton<InputSingleton>();

            var player = SystemAPI.GetSingletonEntity<HumanPlayerSingleton>();

            if (GameUI.Instance.BuildButtonClicked && !SystemAPI.IsComponentEnabled<BuildModeTag>(gameState))
            {
                SystemAPI.SetComponentEnabled<BuildModeTag>(gameState, true);
                var buildingEntity = state.EntityManager.Instantiate(unitEntitiesBuffer[GameUI.Instance.BuildingIndex].Entity);
                ecb.AddComponent<UnitPositioningTag>(buildingEntity);
            }
            
            if (SystemAPI.IsComponentEnabled<BuildModeTag>(gameState))
            {
                var building = unitEntitiesBuffer[GameUI.Instance.BuildingIndex].Entity;
                    
                _currentBuildingRotation += input.ScrollAmount * 2f;
                
                var job = new BuildBuildingJob
                {
                    Ecb = ecb.AsParallelWriter(),
                    Player = player,
                    Unit = building,
                    UnitPosition = input.CursorWorldPosition,
                    UnitRotation = _currentBuildingRotation,
                    BuildUnit = input.WasPrimaryActionPressedThisFrame,
                    CancelBuilding = input.IsCancelActionPressed,
                    UnitDatabase = unitDatabase
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
    
        [WithAll(typeof(UnitPositioningTag))]
        [BurstCompile]
        public partial struct BuildBuildingJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter Ecb;
            public Entity Player;
            public Entity Unit;
            public float3 UnitPosition;
            public bool BuildUnit;
            public bool CancelBuilding;
            public float UnitRotation;
            public BlobAssetReference<UnitsBlobAsset> UnitDatabase;

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