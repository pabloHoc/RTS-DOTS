using System;
using Unity.Collections;
using Unity.Entities;

namespace RTS.Gameplay.Resources
{
    public enum ResourceType
    {
        Stored,
        Cost,
        Production,
        Upkeep,
        AdditiveModifier
    }

    public struct ResourceBufferElement : IBufferElementData
    {
        public FixedString32Bytes Name;
        public int Value;
        public ResourceType Type;
    }
}