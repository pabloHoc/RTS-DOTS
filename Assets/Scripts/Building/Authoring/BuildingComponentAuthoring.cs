using Unity.Entities;
using UnityEngine;

namespace RTS.Building
{
    public class BuildingComponentAuthoring : MonoBehaviour
    {
        public float BuildingTime;

        public class BuildingComponentBaker : Baker<BuildingComponentAuthoring>
        {
            public override void Bake(BuildingComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BuildingComponent { BuildingTime = authoring.BuildingTime });
            }
        }
    }
}