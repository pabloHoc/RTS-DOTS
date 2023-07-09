using System.Collections.Generic;
using RTS.Gameplay.Resources;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace RTS.Gameplay.Buildings
{
    public class BuildingComponentAuthoring : MonoBehaviour
    {
        public string Name;
        public float BuildingTime;
        public List<ResourceDataAuthoring> Cost;

        public class BuildingComponentBaker : Baker<BuildingComponentAuthoring>
        {
            public override void Bake(BuildingComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var buffer = AddBuffer<ResourceBufferElement>(entity);
                
                foreach (var resource in authoring.Cost)
                {
                    buffer.Add(new ResourceBufferElement
                    {
                        Name = resource.Name,
                        Value = resource.Value,
                        Type = ResourceType.Cost
                    });
                }
                
                AddComponent(entity, new BuildingComponent
                {
                    Name = authoring.Name,
                    BuildingTime = authoring.BuildingTime
                });
            }
        }
    }
}