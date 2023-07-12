using System.Collections.Generic;
using RTS.Common;
using RTS.Data;
using RTS.Utils;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace RTS.Gameplay.Units
{
    public struct ResourceBlobAsset
    {
        public FixedString32Bytes Name;
        public int Value;
    }
    
    public struct UnitBlobAsset
    {
        public FixedString32Bytes Name;
        public int BuildTime;
        public BlobArray<ResourceBlobAsset> Cost;
    }
    
    public struct UnitsBlobAsset {
        public BlobArray<UnitBlobAsset> Units;
    }

    public class UnitDatabaseAuthoring : MonoBehaviour
    {
        public UnitsDataContainer DataContainer;

        public class UnitDatabaseBaker : Baker<UnitDatabaseAuthoring>
        {
            public override void Bake(UnitDatabaseAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                
                var blobReference = BlobAssetUtils.BuildBlobAsset(authoring.DataContainer.UnitsData, 
                    delegate(ref BlobBuilder blobBuilder, ref UnitsBlobAsset blobAsset, UnitsData data)
                {
                    var entitiesBuffer = AddBuffer<EntityBufferElement>(entity);
                    var mergedUnitsData = new List<UnitData>();
                    mergedUnitsData.AddRange(data.Buildings);
                    mergedUnitsData.AddRange(data.Units);

                    var unitsDataArray = blobBuilder.Allocate(
                        ref blobAsset.Units,
                        mergedUnitsData.Count
                    );
                    
                    PopulateBlobArray(mergedUnitsData, ref blobBuilder, unitsDataArray);
                    PopulateEntitiesBuffer(mergedUnitsData, entitiesBuffer);
                });
               
                AddBlobAsset(ref blobReference, out var hash);
                AddComponent(entity, new UnitDatabaseSingleton
                {
                    Data = blobReference
                });
            }

            private void PopulateBlobArray(
                List<UnitData> unitData, 
                ref BlobBuilder blobBuilder, 
                BlobBuilderArray<UnitBlobAsset> blobBuilderArray)
            {
                for (var i = 0; i < unitData.Count; i++)
                {
                    var dataAuthoring = unitData[i];

                    blobBuilderArray[i] = new UnitBlobAsset
                    {
                        Name = dataAuthoring.Name
                    };
                    
                    var resourcesDataArray = blobBuilder.Allocate(
                        ref blobBuilderArray[i].Cost,
                        dataAuthoring.Cost.Count
                    );

                    for (var j = 0; j < dataAuthoring.Cost.Count; j++)
                    {
                        var authoringResourceData = dataAuthoring.Cost[j];
                        resourcesDataArray[j] = new ResourceBlobAsset
                        {
                            Name = authoringResourceData.Name,
                            Value = authoringResourceData.Value
                        };
                    }
                }
            }

            private void PopulateEntitiesBuffer(List<UnitData> unitData, DynamicBuffer<EntityBufferElement> entitiesBuffer)
            {
                foreach (var dataAuthoring in unitData)
                {
                    entitiesBuffer.Add(new EntityBufferElement
                    {
                        Entity = GetEntity(dataAuthoring.Prefab, TransformUsageFlags.Dynamic)
                    });
                }
            }
        }
    }
}