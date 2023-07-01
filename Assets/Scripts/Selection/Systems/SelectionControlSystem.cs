using RTS.Input;
using Unity.Burst;
using Unity.Entities;

namespace RTS.Selection
{
    public partial struct SelectionControlSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InputComponent>();
            state.RequireForUpdate<SelectionComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var input = SystemAPI.GetSingleton<InputComponent>();
            var selection = SystemAPI.GetSingletonRW<SelectionComponent>();

            if (input.PrimaryActionPressed && !selection.ValueRO.IsActive)
            {
                SystemAPI.SetSingleton(new SelectionComponent
                {
                    StartPosition = input.CursorWorldPosition,
                    EndPosition = input.CursorWorldPosition,
                    IsActive = true
                });
            }

            if (selection.ValueRO.IsActive)
            {
                selection.ValueRW.EndPosition = input.CursorWorldPosition;
            }

            if (!input.PrimaryActionPressed && selection.ValueRO.IsActive)
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