using Unity.Entities;

namespace RTS.Gameplay.Units
{
    public struct UnitComponent : IComponentData
    {
        public int DatabaseIndex;
    }
}