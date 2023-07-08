using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace RTS.Gameplay.Resources
{   
    [Serializable]
    public struct ResourceDataAuthoring
    {
        public string Name;
        public int InitialQuantity;
    }
    public class ResourceConfigSingletonAuthoring : MonoBehaviour
    {
        public List<ResourceDataAuthoring> ResourcesData;
        
        public class ResourceConfigSingletonBaker : Baker<ResourceConfigSingletonAuthoring>
        {
            public override void Bake(ResourceConfigSingletonAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                var buffer = AddBuffer<ResourceDataBufferElement>(entity);

                foreach (var resourceData in authoring.ResourcesData)
                {
                    buffer.Add(new ResourceDataBufferElement
                    {
                        Name = resourceData.Name,
                        InitialQuantity = resourceData.InitialQuantity
                    });
                }

                AddComponent(entity, new ResourceConfigSingleton());
            }
        }
    }
}