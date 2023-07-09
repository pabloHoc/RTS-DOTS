using Unity.Entities;

namespace RTS.Common
{
    public struct EntityOwnerComponent : IComponentData
    {
        public Entity Entity;
    }
}