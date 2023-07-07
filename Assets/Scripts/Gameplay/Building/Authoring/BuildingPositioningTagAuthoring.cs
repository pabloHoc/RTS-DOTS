using RTS.Building;
using Unity.Entities;
using UnityEngine;

namespace RTS.Gameplay.Building
{
    public class BuildingPositioningTagAuthoring : MonoBehaviour
    {
        public class BuildingPositioningTagBaker : Baker<BuildingPositioningTagAuthoring>
        {
            public override void Bake(BuildingPositioningTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BuildingPositioningTag());
            }
        }
    }
}