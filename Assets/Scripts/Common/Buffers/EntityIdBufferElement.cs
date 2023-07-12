using Unity.Entities;

namespace RTS.Common
{
    public struct EntityIdBufferElement : IBufferElementData
    {
        public int EntityId;
    }
}