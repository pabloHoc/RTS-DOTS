using RTS.Gameplay.Selection;
using Unity.Entities;
using UnityEngine;

namespace RTS.Authoring.Gameplay.Selection
{
    public class SelectedTagAuthoring : MonoBehaviour
    {
        public class SelectedUnitTagBaker : Baker<SelectedTagAuthoring>
        {
            public override void Bake(SelectedTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SelectedTag());
                SetComponentEnabled<SelectedTag>(entity, false);
            }
        }
    }
}