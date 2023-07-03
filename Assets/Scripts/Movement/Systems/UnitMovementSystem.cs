using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace RTS.Movement
{
    [BurstCompile]
    public partial struct UnitMovementSystem : ISystem
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

            var job = new MoveUnitJob
            {
                Ecb = ecb.AsParallelWriter(),
                DeltaTime = SystemAPI.Time.DeltaTime
            };

            state.Dependency = job.Schedule(state.Dependency);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }

    [WithAll(typeof(MoveToComponent), typeof(MovableComponent))]
    [BurstCompile]
    public partial struct MoveUnitJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        [ReadOnly] public float DeltaTime;

        [BurstCompile]
        public void Execute(
            [ChunkIndexInQuery] int chunkIndex, 
            Entity entity, 
            ref LocalTransform transform, 
            in MoveToComponent moveTo, 
            in MovableComponent movable,
            in WorldRenderBounds renderBounds)
        {
            var distance = math.distance(transform.Position, moveTo.TargetPosition);
            var direction = math.normalize(moveTo.TargetPosition - transform.Position);
                
            if (distance > 0.1f)
            {
                transform.Position += direction * movable.Speed * DeltaTime;
                transform.Position.y = renderBounds.Value.Center.y;
            }
            else
            {
                Ecb.RemoveComponent<MoveToComponent>(chunkIndex, entity);    
            }
        }
    }
}