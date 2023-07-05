using RTS.Gameplay.UnitSelection.Tags;
using Unity.Entities;
using UnityEngine;

namespace RTS.Gameplay.UnitSelection
{
    public class SelectableUnitTagAuthoring : MonoBehaviour
    {
        public class SelectableUnitTagBaker : Baker<SelectableUnitTagAuthoring>
        {
            public override void Bake(SelectableUnitTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SelectableUnitTag());
            }
        }
    }
}