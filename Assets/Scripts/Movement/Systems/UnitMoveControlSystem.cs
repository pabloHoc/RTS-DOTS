using RTS.Selection;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace RTS.Movement
{
    public partial struct UnitMoveControlSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<UnitMoveControlComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var moveControl = SystemAPI.GetSingleton<UnitMoveControlComponent>();
            
            if (moveControl.MoveUnits)
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
                       TargetPosition = moveControl.TargetPosition
                    });
                }
                
                SystemAPI.SetSingleton(new UnitMoveControlComponent
                {
                    MoveUnits = false
                });
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}