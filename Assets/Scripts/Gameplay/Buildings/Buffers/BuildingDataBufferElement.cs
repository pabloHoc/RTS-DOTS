using Unity.Collections;
using Unity.Entities;

namespace RTS.Gameplay.Buildings
{
    public struct BuildingDataBufferElement : IBufferElementData
    {
        public FixedString32Bytes Name;
        public Entity Prefab;
    }
}