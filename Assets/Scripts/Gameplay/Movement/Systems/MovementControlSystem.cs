using RTS.Gameplay.Selection;
using RTS.GameState;
using RTS.Input;
using RTS.SystemGroups;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

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
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            if (SystemAPI.IsComponentEnabled<BuildModeTag>(gameState))
            {
                return;
            }
            
            var input = SystemAPI.GetSingleton<InputSingleton>();
            
            if (input.IsSecondaryActionPressed)
            {
                var job = new SetTargetPositionJob
                {
                    Ecb = ecb.AsParallelWriter(),
                    TargetPosition = input.CursorWorldPosition
                };

                state.Dependency = job.ScheduleParallel(state.Dependency);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }

    [WithAll(typeof(SelectedTag), typeof(MovableComponent))]
    [BurstCompile]
    public partial struct SetTargetPositionJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        [ReadOnly] public float3 TargetPosition;

        public void Execute(
            [ChunkIndexInQuery] int chunkIndex, 
            Entity entity)
        {
            // Remove moveTo component in currently moving entities
            Ecb.RemoveComponent<MoveToComponent>(chunkIndex, entity);
            Ecb.AddComponent(chunkIndex, entity, new MoveToComponent
            {   
                TargetPosition = TargetPosition
            });
        }
    }
}