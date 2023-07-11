using RTS.Gameplay.UnitMovement;
using RTS.Gameplay.UnitSelection;
using RTS.SystemGroups;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace RTS.Gameplay.UnitSelection
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct UnitSelectionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<SelectionSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var selection = SystemAPI.GetSingleton<SelectionSingleton>();
            var movableComponentLookup = SystemAPI.GetComponentLookup<MovableComponent>(true);

            if (!selection.IsActive) return;

            var selectionBox = (AABB) new MinMaxAABB
            {
                Min = GetBottomLeftPosition(selection.StartPosition, selection.EndPosition),
                Max = GetTopRightPosition(selection.StartPosition, selection.EndPosition) 
            };
            
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var job = new SelectUnitsJob
            {
                Ecb = ecb.AsParallelWriter(),
                SelectionBox = selectionBox,
                KeepCurrentlySelected = selection.KeepCurrentlySelected,
                SelectedEntity = selection.SelectedEntity,
                MovableComponentLookup = movableComponentLookup
            };

            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
        
        private static float3 GetBottomLeftPosition(float3 start, float3 end)
        {
            return new float3(math.min(start.x, end.x), 0, math.min(start.z, end.z));
        }
        
        private static float3 GetTopRightPosition(float3 start, float3 end)
        {
            return new float3(math.max(start.x, end.x), 10, math.max(start.z, end.z));
        }
        
        public void OnDestroy(ref SystemState state)
        {
            
        }
    }

    [WithAll(typeof(SelectableUnitTag), typeof(LocalToWorld))]
    [BurstCompile]
    public partial struct SelectUnitsJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        [ReadOnly] public AABB SelectionBox; 
        [ReadOnly] public bool KeepCurrentlySelected;
        [ReadOnly] public Entity SelectedEntity;
        [ReadOnly] public ComponentLookup<MovableComponent> MovableComponentLookup;

        public void Execute(Entity entity, LocalToWorld transform)
        {
            var isEntityMovable = MovableComponentLookup.HasComponent(SelectedEntity);
            var isSelectedEntityMovable = MovableComponentLookup.HasComponent(SelectedEntity);
            
            if (entity.Equals(SelectedEntity) || (isEntityMovable && SelectionBox.Contains(transform.Position)))
            {
                Ecb.SetComponentEnabled<SelectedUnitTag>(entity.Index, entity, true);
            }
            
            // If we select a building we want to deselect movable units
            else if (!KeepCurrentlySelected || !isSelectedEntityMovable)
            {
                Ecb.SetComponentEnabled<SelectedUnitTag>(entity.Index, entity, false);
            }
        }
    }
}