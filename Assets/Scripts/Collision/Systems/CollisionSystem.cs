using RTS.Gameplay.Selection;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace RTS.Collision
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    [BurstCompile]
    public partial struct CollisionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        [BurstCompile]
public void OnUpdate(ref SystemState state)
{
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var collisionWorld = physicsWorld.CollisionWorld;

            foreach (var (_, transform, entity) in SystemAPI.Query<SelectableTag, LocalTransform>().WithEntityAccess())
            {
                // Approach 1
                
                var collisionFilter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = ~0u,
                    GroupIndex = 0
                };
                var closestHitCollector = new ClosestHitCollector<DistanceHit>(1f);
                if (physicsWorld.OverlapSphereCustom(transform.Position, 1f, ref closestHitCollector, collisionFilter) && !closestHitCollector.ClosestHit.Entity.Equals(entity))
                {
                    // Debug.Log($"Entity: {entity.Index} | Closest hit: {closestHitCollector.ClosestHit.Entity.Index}");
                }
                
                // Approach 2
                unsafe
                {
                    var collider = SystemAPI.GetComponent<PhysicsCollider>(entity);
                
                    ColliderCastInput input = new ColliderCastInput
                    {
                        Collider = collider.ColliderPtr,
                        Orientation = quaternion.identity,
                        Start = transform.Position,
                        End = (transform.Position + math.forward()) * 3
                        
                    };

                    var hit = new ColliderCastHit();
                    var haveHit = collisionWorld.CastCollider(input, out hit);

                    if (haveHit && !hit.Entity.Equals(entity) && SystemAPI.HasComponent<SelectableTag>(hit.Entity))
                    {
                        // Debug.Log($"Entity: {entity.Index} | Entity Hit: {hit.Entity.Index}");
                    }
                }
            }
            
            // Approach 3
            state.Dependency = new CollisionEventJob().Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        }
        
        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            
        }

        [BurstCompile]
        private struct CollisionEventJob : ICollisionEventsJob
        {
            public void Execute(CollisionEvent collisionEvent)
            {
                // Debug.Log($"Entity A: {collisionEvent.EntityA.Index} | Entity B: {collisionEvent.EntityB.Index}");
            }
        }
    }
}