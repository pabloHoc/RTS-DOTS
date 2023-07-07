using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace RTS.Gameplay.Buildings
{
    [Serializable]
    public struct BuildingDataAuthoring
    {
        public string Name;
        public GameObject Prefab;
    }
    public class BuildingConfigSingletonAuthoring : MonoBehaviour
    {
        public List<BuildingDataAuthoring> BuildingsData;

        public class BuildingConfigSingletonBaker : Baker<BuildingConfigSingletonAuthoring>
        {
            public override void Bake(BuildingConfigSingletonAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                var buffer = AddBuffer<BuildingDataBuffer>(entity);

                foreach (var buildingData in authoring.BuildingsData)
                {
                    buffer.Add(new BuildingDataBuffer
                    {
                        Name = buildingData.Name,
                        Prefab = GetEntity(buildingData.Prefab, TransformUsageFlags.Dynamic)
                    });
                }

                AddComponent(entity, new BuildingConfigSingleton());
            }
        }
    }
}