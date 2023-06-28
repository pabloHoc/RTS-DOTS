using Unity.Entities;
using Unity.Mathematics;

namespace RTS.Movement
{
    public struct MoveToComponent : IComponentData
    {
        public float3 TargetPosition;
    }
}