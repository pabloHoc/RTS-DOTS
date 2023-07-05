using Unity.Entities;

namespace RTS.Gameplay.Building
{
    public struct BuildingReferencesComponent : IComponentData
    {
        public Entity SomeBuilding;
    }
}