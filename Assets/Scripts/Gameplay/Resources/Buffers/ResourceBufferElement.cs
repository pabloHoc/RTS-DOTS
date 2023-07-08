using Unity.Collections;
using Unity.Entities;

namespace RTS.Gameplay.Resources
{
    public struct ResourceBufferElement : IBufferElementData
    {
        public FixedString32Bytes Name;
        public int Quantity;
    }
}