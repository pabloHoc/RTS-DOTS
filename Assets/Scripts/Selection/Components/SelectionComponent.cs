using Unity.Entities;
using Unity.Mathematics;

namespace RTS.Selection
{
    public struct SelectionComponent : IComponentData
    {
        public float3 StartPosition;
        public float3 EndPosition;
        public bool IsActive;
    }
}