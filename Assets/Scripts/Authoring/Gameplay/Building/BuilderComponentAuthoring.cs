using RTS.Gameplay.Building;
using Unity.Entities;
using UnityEngine;

namespace RTS.Authoring.Gameplay.Building
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