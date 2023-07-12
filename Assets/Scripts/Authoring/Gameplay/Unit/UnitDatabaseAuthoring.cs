using System.Collections.Generic;
using RTS.Common;
using RTS.Data;
using RTS.Gameplay.Units;
using RTS.Utils;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace RTS.Authoring.Gameplay.Unit
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
        public BlobArray<int> BuildableUnitIds;
    }
    
    public struct UnitsBlobAsset {
        public BlobArray<UnitBlobAsset> Units;
    }

    public class UnitDatabaseAuthoring : MonoBehaviour
    {
        public UnitsData UnitsData;

        public class UnitDatabaseBaker : Baker<UnitDatabaseAuthoring>
        {
            public override void Bake(UnitDatabaseAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                
                var blobReference = BlobAssetUtils.BuildBlobAsset(authoring.UnitsData, 
                    delegate(ref BlobBuilder blobBuilder, ref UnitsBlobAsset blobAsset, UnitsData authoringData)
                {
                    var unitsAuthoringData = new List<UnitData>();
                    unitsAuthoringData.AddRange(authoringData.Buildings);
                    unitsAuthoringData.AddRange(authoringData.Units);

                    var unitsDataArray = blobBuilder.Allocate(
                        ref blobAsset.Units,
                        unitsAuthoringData.Count
                    );
                    
                    PopulateBlobArray(unitsAuthoringData, ref blobBuilder, unitsDataArray);
                    AddEntitiesBuffer(entity, unitsAuthoringData);
                    AddBuildableEntityIdsBuffer(entity, unitsAuthoringData);
                });
               
                AddBlobAsset(ref blobReference, out var hash);
                AddComponent(entity, new UnitDatabaseSingleton
                {
                    Data = blobReference
                });
            }

            private void PopulateBlobArray(
                List<UnitData> unitsAuthoringData, 
                ref BlobBuilder blobBuilder, 
                BlobBuilderArray<UnitBlobAsset> blobBuilderArray)
            {
                for (var i = 0; i < unitsAuthoringData.Count; i++)
                {
                    var unitAuthoringData = unitsAuthoringData[i];

                    blobBuilderArray[i] = new UnitBlobAsset
                    {
                        Name = unitAuthoringData.Name
                    };
                    
                    // Add resources
                    
                    var resourcesDataArray = blobBuilder.Allocate(
                        ref blobBuilderArray[i].Cost,
                        unitAuthoringData.Cost.Count
                    );

                    for (var j = 0; j < unitAuthoringData.Cost.Count; j++)
                    {
                        var resourceAuthoringData = unitAuthoringData.Cost[j];
                        resourcesDataArray[j] = new ResourceBlobAsset
                        {
                            Name = resourceAuthoringData.Name,
                            Value = resourceAuthoringData.Value
                        };
                    }
                    
                    // Add buildable unit ids
                    
                    var buildableUnitIdsDataArray = blobBuilder.Allocate(
                        ref blobBuilderArray[i].BuildableUnitIds,
                        unitAuthoringData.BuildableUnitIds.Count
                    );

                    for (var j = 0; j < unitAuthoringData.BuildableUnitIds.Count; j++)
                    {
                        buildableUnitIdsDataArray[j] = unitAuthoringData.BuildableUnitIds[j];
                    }
                }
            }

            private void AddEntitiesBuffer(Entity entity, List<UnitData> data)
            {
                var entitiesBuffer = AddBuffer<EntityBufferElement>(entity);

                foreach (var dataAuthoring in data)
                {
                    entitiesBuffer.Add(new EntityBufferElement
                    {
                        Entity = GetEntity(dataAuthoring.Prefab, TransformUsageFlags.Dynamic)
                    });
                }
            }
            
            private void AddBuildableEntityIdsBuffer(Entity entity, List<UnitData> data)
            {
                var entitiesBuffer = AddBuffer<EntityIdBufferElement>(entity);

                foreach (var dataAuthoring in data)
                {
                    foreach (var buildableUnitId in dataAuthoring.BuildableUnitIds)
                    {
                        entitiesBuffer.Add(new EntityIdBufferElement
                        {
                            EntityId = buildableUnitId
                        });
                    }
                }
            }
        }
    }
}