using RTS.Gameplay.Selection;
using Unity.Entities;
using UnityEngine;

namespace RTS.Authoring.Gameplay.Selection
{
    public class SelectableTagAuthoring : MonoBehaviour
    {
        public class SelectableTagBaker : Baker<SelectableTagAuthoring>
        {
            public override void Bake(SelectableTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SelectableTag());
            }
        }
    }
}