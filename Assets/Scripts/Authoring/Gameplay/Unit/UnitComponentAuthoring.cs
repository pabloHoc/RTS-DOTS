using System.Collections.Generic;
using RTS.Common;
using RTS.Gameplay.Resources;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace RTS.Gameplay.Units
{
    public class UnitComponentAuthoring : MonoBehaviour
    {
        public int UnitId;

        public class UnitComponentBaker : Baker<UnitComponentAuthoring>
        {
            public override void Bake(UnitComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new UnitComponent
                {
                    UnitId = authoring.UnitId
                });
            }
        }
    }
}