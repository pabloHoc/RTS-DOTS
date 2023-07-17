using RTS.Common;
using RTS.Gameplay.Movement;
using RTS.Gameplay.Players.Singletons;
using RTS.Gameplay.Units;
using RTS.GameState;
using RTS.Input;
using RTS.SystemGroups;
using RTS.UI;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace RTS.Gameplay.Building
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    [BurstCompile]
    public partial struct InstantiationSystem : ISystem
    {
        private float _currentBuildingRotation;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<HumanPlayerSingleton>();
            state.RequireForUpdate<UnitDatabaseSingleton>();
            state.RequireForUpdate<GameStateSingleton>();
            state.RequireForUpdate<InputSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var gameState = SystemAPI.GetSingletonEntity<GameStateSingleton>();

            if (GameUI.Instance.BuildButtonClicked && !SystemAPI.IsComponentEnabled<BuildModeTag>(gameState))
            {
                var unitRepository = SystemAPI.GetSingletonEntity<UnitDatabaseSingleton>();
                var unitEntitiesBuffer = SystemAPI.GetBuffer<EntityBufferElement>(unitRepository);
            
                var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
                var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

                var unitToCreate = state.EntityManager.Instantiate(unitEntitiesBuffer[GameUI.Instance.BuildingIndex].Entity);
                var player = SystemAPI.GetSingletonEntity<HumanPlayerSingleton>();
                var isPositionableEntity = SystemAPI.HasComponent<PositionableTag>(unitToCreate);

                var job = new InstantiateUnitJob
                {
                    Ecb = ecb,
                    Player = player,
                    UnitToCreate = unitToCreate,
                    GameState = gameState,
                    IsPositionableEntity = isPositionableEntity
                };

                state.Dependency = job.Schedule(state.Dependency);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }

    public struct InstantiateUnitJob : IJob
    {
        public EntityCommandBuffer Ecb;
        [ReadOnly] public Entity Player;
        [ReadOnly] public Entity UnitToCreate;
        [ReadOnly] public Entity GameState;
        [ReadOnly] public bool IsPositionableEntity;

        public void Execute()
        { 
            Ecb.SetComponentEnabled<BuildModeTag>(GameState, true);

            if (IsPositionableEntity)
            {
                Ecb.AddComponent<PositioningTag>(UnitToCreate);
            } else
            {
                Ecb.SetComponent(UnitToCreate, new MoveToComponent
                {
                    TargetPosition = new float3(50, 0, 50)
                });

                Ecb.AddComponent(UnitToCreate, new EntityOwnerComponent
                {
                    Entity = Player
                });

                Ecb.AddComponent<EntityCreatedTag>(UnitToCreate);
                Ecb.SetComponentEnabled<BuildModeTag>(GameState, false);
            }
        }
    }
}