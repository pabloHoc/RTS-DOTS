using Unity.Entities;

namespace RTS.Common
{
    public struct EntityBufferElement : IBufferElementData
    {
        public Entity Entity;
    }
}