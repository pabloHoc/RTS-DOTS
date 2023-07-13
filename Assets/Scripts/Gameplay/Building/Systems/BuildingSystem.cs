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
using Unity.Mathematics;

namespace RTS.Gameplay.Building
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct BuildingSystem : ISystem
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

            var unitRepository = SystemAPI.GetSingletonEntity<UnitDatabaseSingleton>();
            var unitEntitiesBuffer = SystemAPI.GetBuffer<EntityBufferElement>(unitRepository);
            
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            if (GameUI.Instance.BuildButtonClicked && !SystemAPI.IsComponentEnabled<BuildModeTag>(gameState))
            {
                SystemAPI.SetComponentEnabled<BuildModeTag>(gameState, true);
                var unitToCreate = state.EntityManager.Instantiate(unitEntitiesBuffer[GameUI.Instance.BuildingIndex].Entity);

                if (SystemAPI.HasComponent<PositionableTag>(unitToCreate))
                {
                    state.EntityManager.AddComponent<PositioningTag>(unitToCreate);
                } else if (SystemAPI.HasComponent<MoveToComponent>(unitToCreate))
                {
                    var player = SystemAPI.GetSingletonEntity<HumanPlayerSingleton>();
                    
                    SystemAPI.SetComponent(unitToCreate, new MoveToComponent
                    {
                        TargetPosition = new float3(50, 0, 50)
                    });
                    
                    state.EntityManager.AddComponentData(unitToCreate, new EntityOwnerComponent
                    {
                        Entity = player
                    });
                    
                    state.EntityManager.AddComponent<EntityCreatedTag>(unitToCreate);
                    state.EntityManager.SetComponentEnabled<BuildModeTag>(gameState, false);
                }
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}