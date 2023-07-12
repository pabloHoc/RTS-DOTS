using RTS.Gameplay.UnitSelection;
using Unity.Entities;
using UnityEngine;

namespace RTS.Authoring.Gameplay.UnitSelection
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