using RTS.Gameplay.UnitBuilding;
using Unity.Entities;
using UnityEngine;

namespace RTS.Authoring.Gameplay.UnitBuilding
{
    public class BuilderComponentAuthoring : MonoBehaviour
    {
        public class BuilderComponentBaker : Baker<BuilderComponentAuthoring>
        {
            public override void Bake(BuilderComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BuilderComponent());
            }
        }
    }
}