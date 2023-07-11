using Unity.Collections;
using Unity.Entities;

namespace RTS.Gameplay.Buildings
{
    public struct BuildingComponent : IComponentData
    {
        // Building index in building database
        public int Index;
        public FixedString32Bytes Name;
        public float BuildingTime;
    }
}