using Unity.Collections;
using Unity.Entities;

namespace RTS.Gameplay.Buildings
{
    public struct ResourceData
    {
        public FixedString32Bytes Name;
        public int Value;
    }
    
    public struct BuildingData
    {
        public FixedString32Bytes Name;
        public BlobArray<ResourceData> Cost;
        
    }

    public struct BuildingsData
    {
        public BlobArray<BuildingData> Buildings;
    }
    
    public struct BuildingDatabaseSingleton : IComponentData
    {
        public BlobAssetReference<BuildingsData> Data;
    }
}