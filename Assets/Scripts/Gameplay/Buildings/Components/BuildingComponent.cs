using Unity.Collections;
using Unity.Entities;

namespace RTS.Gameplay.Buildings
{
    public struct BuildingComponent : IComponentData
    {
        public FixedString32Bytes Name;
        public float BuildingTime;
    }
}