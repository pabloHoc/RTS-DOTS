using RTS.Data;
using Unity.Entities;

namespace RTS.Gameplay.Units
{
    public struct UnitDatabaseSingleton : IComponentData
    {
        public BlobAssetReference<UnitsBlobAsset> Data;
    }
}