using Unity.Entities;
using UnityEngine;

namespace RTS.Building
{
    public class BuildModeTagAuthoring : MonoBehaviour
    {
        public class BuildModeTagBaker : Baker<BuildModeTagAuthoring>
        {
            public override void Bake(BuildModeTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BuildModeTag());
                SetComponentEnabled<BuildModeTag>(entity, false);
            }
        }
    }
}