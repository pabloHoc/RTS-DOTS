using Unity.Collections;
using Unity.Entities;

namespace RTS.Gameplay.Building
{
    public struct BuildingDataBuffer : IBufferElementData
    {
        public FixedString32Bytes Name;
        public Entity Prefab;
    }
}