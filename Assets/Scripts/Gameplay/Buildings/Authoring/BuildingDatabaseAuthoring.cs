using System;
using System.Collections.Generic;
using RTS.Common;
using RTS.Gameplay.Resources;
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
                var entitiesBuffer = AddBuffer<EntityBufferElement>(entity);

                var builder = new BlobBuilder(Allocator.Temp);

                // Construct the root object for the blob asset. Notice the use of `ref`.
                ref var buildingsData = ref builder.ConstructRoot<BuildingsData>();
                var buildingsDataArray = builder.Allocate(
                    ref buildingsData.Buildings,
                    authoring.BuildingsData.Count
                );
                
                // Now fill the constructed root with the data:
                for (var i = 0; i < authoring.BuildingsData.Count; i++)
                {
                    var authoringBuildingData = authoring.BuildingsData[i];
                    
                    buildingsDataArray[i] = new BuildingData
                    {
                        Name = authoringBuildingData.Name
                    };
                    
                    var resourcesDataArray = builder.Allocate(
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
                        Entity = GetEntity(authoringBuildingData.Prefab, TransformUsageFlags.Dynamic),
                    });
                }

                // Now copy the data from the builder into its final place, which will
                // use the persistent allocator
                var blobReference =
                    builder.CreateBlobAssetReference<BuildingsData>(Allocator.Persistent);

                // Make sure to dispose the builder itself so all internal memory is disposed.
                builder.Dispose();

                // Register the Blob Asset to the Baker for de-duplication and reverting.
                AddBlobAsset(ref blobReference, out var hash);
                AddComponent(entity, new BuildingDatabaseSingleton
                {
                    Data = blobReference
                });
            }
        }
    }
}