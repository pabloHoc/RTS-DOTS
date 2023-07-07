using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace RTS.Gameplay.Building
{
    [Serializable]
    public struct BuildingDataAuthoring
    {
        public string Name;
        public GameObject Prefab;
    }
    public class BuildingConfigComponentAuthoring : MonoBehaviour
    {
        public List<BuildingDataAuthoring> BuildingsData;

        public class BuildingConfigComponentBaker : Baker<BuildingConfigComponentAuthoring>
        {
            public override void Bake(BuildingConfigComponentAuthoring authoring)
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

                AddComponent(entity, new BuildingConfigComponent());
            }
        }
    }
}