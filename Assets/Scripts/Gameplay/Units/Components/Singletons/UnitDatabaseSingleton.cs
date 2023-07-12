using Unity.Collections;
using Unity.Entities;

namespace RTS.Gameplay.Units
{
    public struct ResourceData
    {
        public FixedString32Bytes Name;
        public int Value;
    }
    
    public struct UnitData
    {
        public FixedString32Bytes Name;
        public BlobArray<ResourceData> Cost;
        
    }

    public struct UnitsData
    {
        public BlobArray<UnitData> Units;
    }
    
    public struct UnitDatabaseSingleton : IComponentData
    {
        public BlobAssetReference<UnitsData> Data;
    }
}