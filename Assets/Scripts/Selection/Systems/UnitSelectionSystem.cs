using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace RTS.Selection
{
    public partial struct UnitSelectionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<SelectionComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var selection = SystemAPI.GetSingleton<SelectionComponent>();

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
                SelectionBox = selectionBox,
                Ecb = ecb.AsParallelWriter(),
                KeepCurrentlySelected = selection.KeepCurrentlySelected,
                SelectedEntity = selection.SelectedEntity
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
        [ReadOnly] public AABB SelectionBox; 
        public EntityCommandBuffer.ParallelWriter Ecb;
        [ReadOnly] public bool KeepCurrentlySelected;
        public Entity SelectedEntity;

        public void Execute(Entity entity, LocalToWorld transform)
        {
            if (SelectionBox.Contains(transform.Position) || entity.Equals(SelectedEntity))
            {
                Ecb.SetComponentEnabled<SelectedUnitTag>(entity.Index, entity, true);
            }
            else if (!KeepCurrentlySelected)
            {
                Ecb.SetComponentEnabled<SelectedUnitTag>(entity.Index, entity, false);
            }
        }
    }
}