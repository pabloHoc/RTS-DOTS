using Unity.Entities;
using Unity.Mathematics;

namespace RTS.Selection
{
    public struct SelectionControlComponent : IComponentData
    {
        public bool IsSelecting;
        public float3 CursorPosition;
    }
}