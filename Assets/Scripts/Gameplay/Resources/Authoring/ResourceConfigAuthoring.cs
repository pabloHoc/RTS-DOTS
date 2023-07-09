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
        public int Value;
    }
    
    public class ResourceConfigAuthoring : MonoBehaviour
    {
        public List<ResourceDataAuthoring> ResourcesData;
        public class ResourceConfigBaker : Baker<ResourceConfigAuthoring>
        {
            public override void Bake(ResourceConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ResourceConfigSingleton());
            }
        }
    }
}