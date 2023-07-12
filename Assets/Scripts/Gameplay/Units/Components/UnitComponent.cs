using Unity.Collections;
using Unity.Entities;

namespace RTS.Gameplay.Units
{
    public struct UnitComponent : IComponentData
    {
        // Building index in building database
        public int Index;
        public FixedString32Bytes Name;
        public float BuildingTime;
    }
}