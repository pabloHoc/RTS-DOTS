using Unity.Entities;

namespace RTS.Building
{
    public struct BuildingReferencesComponent : IComponentData
    {
        public Entity SomeBuilding;
    }
}