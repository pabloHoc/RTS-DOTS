using RTS.Gameplay.Units;
using Unity.Entities;
using UnityEngine;

namespace RTS.Authoring.Gameplay.Unit
{
    public class UnitComponentAuthoring : MonoBehaviour
    {
        public int DatabaseIndex;

        public class UnitComponentBaker : Baker<UnitComponentAuthoring>
        {
            public override void Bake(UnitComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new UnitComponent
                {
                    DatabaseIndex = authoring.DatabaseIndex
                });
            }
        }
    }
}