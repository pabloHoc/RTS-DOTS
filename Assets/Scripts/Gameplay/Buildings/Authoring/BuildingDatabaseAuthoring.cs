using System;
using System.Collections.Generic;
using RTS.Common;
using RTS.Gameplay.Resources;
using RTS.Utils;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace RTS.Gameplay.Buildings
{
    [Serializable]
    public struct ResourceDataAuthoring
    {
        public string Name;
        public int Value;
    }

    [Serializable]
    public struct BuildingDataAuthoring
    {
        public string Name;
        public List<ResourceDataAuthoring> Cost;
        public GameObject Prefab;
    }

    public class BuildingDatabaseAuthoring : MonoBehaviour
    {
        public List<BuildingDataAuthoring> BuildingsData;

        public class BuildingDatabaseBaker : Baker<BuildingDatabaseAuthoring>
        {
            public override void Bake(BuildingDatabaseAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                
                var blobReference = BlobAssetUtils.BuildBlobAsset(authoring.BuildingsData, 
                    delegate(ref BlobBuilder blobBuilder, ref BuildingsData blobAsset, List<BuildingDataAuthoring> data)
                {
                    var buildingsDataArray = blobBuilder.Allocate(
                        ref blobAsset.Buildings,
                        data.Count
                    );
                    
                    var entitiesBuffer = AddBuffer<EntityBufferElement>(entity);
                    
                    for (var i = 0; i < data.Count; i++)
                    {
                        var authoringBuildingData = data[i];
                    
                        buildingsDataArray[i] = new BuildingData
                        {
                            Name = authoringBuildingData.Name
                        };
                    
                        var resourcesDataArray = blobBuilder.Allocate(
                            ref buildingsDataArray[i].Cost,
                            authoringBuildingData.Cost.Count
                        );
                    
                        for (var j = 0; j < authoringBuildingData.Cost.Count; j++)
                        {
                            var authoringResourceData = authoringBuildingData.Cost[j];
                            resourcesDataArray[j] = new ResourceData
                            {
                                Name = authoringResourceData.Name,
                                Value = authoringResourceData.Value,
                            };
                        }
                    
                        entitiesBuffer.Add(new EntityBufferElement
                        {
                            Entity = GetEntity(authoringBuildingData.Prefab, TransformUsageFlags.Dynamic)
                        });
                    }
                });
               
                AddBlobAsset(ref blobReference, out var hash);
                AddComponent(entity, new BuildingDatabaseSingleton
                {
                    Data = blobReference
                });
            }
        }
    }
}