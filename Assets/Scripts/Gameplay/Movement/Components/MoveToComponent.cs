using Unity.Entities;
using Unity.Mathematics;

namespace RTS.Gameplay.Movement
{
    public struct MoveToComponent : IComponentData
    {
        public float3 TargetPosition;
    }
}