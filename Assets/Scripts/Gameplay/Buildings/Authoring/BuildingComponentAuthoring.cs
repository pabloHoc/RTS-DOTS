using System.Collections.Generic;
using RTS.Gameplay.Resources;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace RTS.Gameplay.Buildings
{
    public class BuildingComponentAuthoring : MonoBehaviour
    {
        public int Index;
        public string Name;
        public float BuildingTime;

        public class BuildingComponentBaker : Baker<BuildingComponentAuthoring>
        {
            public override void Bake(BuildingComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BuildingComponent
                {
                    Index = authoring.Index,
                    Name = authoring.Name,
                    BuildingTime = authoring.BuildingTime
                });
            }
        }
    }
}