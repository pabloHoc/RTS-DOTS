using RTS.Input;
using RTS.Selection;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace RTS.Movement
{
    [BurstCompile]
    public partial struct UnitControlSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InputComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var input = SystemAPI.GetSingleton<InputComponent>();
            
            if (input.SecondaryActionPressed)
            {
                var query = SystemAPI.QueryBuilder().WithAll<SelectedUnitTag, MovableComponent>().Build();
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