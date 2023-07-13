using RTS.Gameplay.Selection;
using RTS.GameState;
using RTS.Input;
using RTS.SystemGroups;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace RTS.Gameplay.Movement
{
    [UpdateInGroup(typeof(InputSystemGroup))]
    [UpdateAfter(typeof(InputSystem))]
    [BurstCompile]
    public partial struct MovementControlSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameStateSingleton>();
            state.RequireForUpdate<InputSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var gameState = SystemAPI.GetSingletonEntity<GameStateSingleton>();

            if (SystemAPI.IsComponentEnabled<BuildModeTag>(gameState))
            {
                return;
            }
            
            var input = SystemAPI.GetSingleton<InputSingleton>();
            
            if (input.IsSecondaryActionPressed)
            {
                var query = SystemAPI.QueryBuilder().WithAll<SelectedTag, MovableComponent>().Build();
                var entities = query.ToEntityArray(Allocator.Temp);
                
                // Remove moveTo component in currently moving entities
                state.EntityManager.RemoveComponent<MoveToComponent>(entities);
                state.EntityManager.AddComponent<MoveToComponent>(entities);
                
                foreach (var entity in entities)
                {
                    SystemAPI.SetComponent(entity, new MoveToComponent
                    {   
                       TargetPosition = input.CursorWorldPosition
                    });
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}