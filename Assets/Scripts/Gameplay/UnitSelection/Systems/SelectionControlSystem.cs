using RTS.Building;
using RTS.Gameplay.UnitSelection.Singletons;
using RTS.GameState;
using RTS.Input;
using RTS.SystemGroups;
using Unity.Burst;
using Unity.Entities;

namespace RTS.Gameplay.UnitSelection
{
    [UpdateInGroup(typeof(InputSystemGroup))]
    [UpdateAfter(typeof(InputSystem))]
    public partial struct SelectionControlSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameStateComponent>();
            state.RequireForUpdate<InputComponent>();
            state.RequireForUpdate<SelectionComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // There's some duplication here
            var gameState = SystemAPI.GetSingletonEntity<GameStateComponent>();

            if (SystemAPI.IsComponentEnabled<BuildModeTag>(gameState))
            {
                return;
            }
            
            var input = SystemAPI.GetSingleton<InputComponent>();
            var selection = SystemAPI.GetSingletonRW<SelectionComponent>();
            
            if (input.IsPrimaryActionPressed && !selection.ValueRO.IsActive)
            {
                SystemAPI.SetSingleton(new SelectionComponent
                {
                    StartPosition = input.CursorWorldPosition,
                    EndPosition = input.CursorWorldPosition,
                    IsActive = true,
                    KeepCurrentlySelected = input.IsSelectMultipleUnitsPressed,
                    SelectedEntity = input.EntityHit
                });
            }

            if (selection.ValueRO.IsActive)
            {
                selection.ValueRW.EndPosition = input.CursorWorldPosition;
            }

            if (!input.IsPrimaryActionPressed && selection.ValueRO.IsActive)
            {
                selection.ValueRW.IsActive = false;
            }
        }
        
        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            
        }
    }
}