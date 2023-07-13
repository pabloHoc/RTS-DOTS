using Unity.Entities;
using Unity.Mathematics;

namespace RTS.Gameplay.Selection
{
    public struct SelectionSingleton : IComponentData
    {
        public float3 StartPosition;
        public float3 EndPosition;
        public bool IsActive;
        public bool KeepCurrentlySelected;
        public Entity SelectedEntity;
    }
}