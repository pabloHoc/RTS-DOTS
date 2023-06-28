using Unity.Burst;
using Unity.Entities;

namespace RTS.Selection
{
    public partial struct SelectionControlSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SelectionControlComponent>();
            state.RequireForUpdate<SelectionComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var selectionControl = SystemAPI.GetSingleton<SelectionControlComponent>();
            var selection = SystemAPI.GetSingletonRW<SelectionComponent>();

            if (selectionControl.IsSelecting && !selection.ValueRO.IsActive)
            {
                SystemAPI.SetSingleton(new SelectionComponent
                {
                    StartPosition = selectionControl.CursorPosition,
                    EndPosition = selectionControl.CursorPosition,
                    IsActive = true
                });
            }

            if (selection.ValueRO.IsActive)
            {
                selection.ValueRW.EndPosition = selectionControl.CursorPosition;
            }

            if (!selectionControl.IsSelecting && selection.ValueRO.IsActive)
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