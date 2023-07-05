using Unity.Entities;
using Unity.Mathematics;

namespace RTS.Gameplay.UnitMovement
{
    public struct MoveToComponent : IComponentData
    {
        public float3 TargetPosition;
    }
}