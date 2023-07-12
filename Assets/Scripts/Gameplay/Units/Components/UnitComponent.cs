using Unity.Collections;
using Unity.Entities;

namespace RTS.Gameplay.Units
{
    public struct UnitComponent : IComponentData
    {
        public int DatabaseIndex;
        public FixedString32Bytes Name;
        public float BuildingTime;
    }
}