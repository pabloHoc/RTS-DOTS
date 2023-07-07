using Unity.Collections;
using Unity.Entities;

namespace RTS.Gameplay.Buildings
{
    public struct BuildingDataBuffer : IBufferElementData
    {
        public FixedString32Bytes Name;
        public Entity Prefab;
    }
}