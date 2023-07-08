using Unity.Collections;
using Unity.Entities;

namespace RTS.Gameplay.Resources
{
    public struct ResourceDataBufferElement : IBufferElementData
    {
        public FixedString32Bytes Name;
        public int InitialQuantity;
    }
}