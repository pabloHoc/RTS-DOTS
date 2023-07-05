using Unity.Entities;
using UnityEngine;

namespace RTS.Building
{
    public class BuildingReferencesComponentAuthoring : MonoBehaviour
    {
        public GameObject SomeBuilding;

        public class BuildingReferencesComponentBaker : Baker<BuildingReferencesComponentAuthoring>
        {
            public override void Bake(BuildingReferencesComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BuildingReferencesComponent
                {
                    SomeBuilding = GetEntity(authoring.SomeBuilding, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}