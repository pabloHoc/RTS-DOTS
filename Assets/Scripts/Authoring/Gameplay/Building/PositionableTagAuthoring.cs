using RTS.Gameplay.Building;
using Unity.Entities;
using UnityEngine;

namespace RTS.Authoring.Gameplay.Building
{
    public class PositionableTagAuthoring : MonoBehaviour
    {
        public class PositionableTagBaker : Baker<PositionableTagAuthoring>
        {
            public override void Bake(PositionableTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new PositionableTag());
            }
        }
    }
}