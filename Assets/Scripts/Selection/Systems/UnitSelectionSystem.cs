using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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

            foreach (var (transform, entity) in SystemAPI.Query<LocalToWorld>().WithAll<SelectableUnitTag>().WithEntityAccess())
            {
                ecb.AddComponent<SelectedUnitTag>(entity);
                
                Debug.Log($"Min: {selectionBox.Min} | Max: {selectionBox.Max} | Point: {transform.Position}");

                if (selectionBox.Contains(transform.Position))
                {
                    ecb.AddComponent<SelectedUnitTag>(entity);
                }
                else
                {
                    ecb.RemoveComponent<SelectedUnitTag>(entity);
                }
            }
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
}