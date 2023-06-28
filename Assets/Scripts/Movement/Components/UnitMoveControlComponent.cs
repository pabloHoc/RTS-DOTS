using Unity.Entities;
using Unity.Mathematics;

namespace RTS.Movement
{
    public struct UnitMoveControlComponent : IComponentData
    {
        public bool MoveUnits;
        public float3 TargetPosition;
    }
}