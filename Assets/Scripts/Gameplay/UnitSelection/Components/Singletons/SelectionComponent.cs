using Unity.Entities;
using Unity.Mathematics;

namespace RTS.Gameplay.UnitSelection.Singletons
{
    public struct SelectionComponent : IComponentData
    {
        public float3 StartPosition;
        public float3 EndPosition;
        public bool IsActive;
        public bool KeepCurrentlySelected;
        public Entity SelectedEntity;
    }
}