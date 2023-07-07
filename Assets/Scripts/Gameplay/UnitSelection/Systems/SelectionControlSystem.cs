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
            state.RequireForUpdate<GameStateSingleton>();
            state.RequireForUpdate<InputSingleton>();
            state.RequireForUpdate<SelectionSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // There's some duplication here
            var gameState = SystemAPI.GetSingletonEntity<GameStateSingleton>();

            if (SystemAPI.IsComponentEnabled<BuildModeTag>(gameState))
            {
                return;
            }
            
            var input = SystemAPI.GetSingleton<InputSingleton>();
            var selection = SystemAPI.GetSingletonRW<SelectionSingleton>();
            
            if (input.IsPrimaryActionPressed && !selection.ValueRO.IsActive)
            {
                SystemAPI.SetSingleton(new SelectionSingleton
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