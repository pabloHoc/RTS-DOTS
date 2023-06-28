using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace RTS.Movement
{
    public partial struct MoveToSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (transform, moveTo, movable, entity) in SystemAPI.Query<RefRW<LocalTransform>, MoveToComponent, MovableComponent>().WithEntityAccess())
            {
                var distance = math.distance(transform.ValueRO.Position, moveTo.TargetPosition);
                var direction = math.normalize(moveTo.TargetPosition - transform.ValueRO.Position);
                
                if (distance > 0.1f)
                {
                    transform.ValueRW.Position += direction * movable.Speed * SystemAPI.Time.DeltaTime;
                }
                else
                {
                    ecb.RemoveComponent<MoveToComponent>(entity);    
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}