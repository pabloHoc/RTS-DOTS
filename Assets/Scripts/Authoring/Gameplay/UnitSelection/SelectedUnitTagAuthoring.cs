using RTS.Gameplay.UnitSelection;
using Unity.Entities;
using UnityEngine;

namespace RTS.Authoring.Gameplay.UnitSelection
{
    public class SelectedUnitTagAuthoring : MonoBehaviour
    {
        public class SelectedUnitTagBaker : Baker<SelectedUnitTagAuthoring>
        {
            public override void Bake(SelectedUnitTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SelectedUnitTag());
                SetComponentEnabled<SelectedUnitTag>(entity, false);
            }
        }
    }
}