using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace RTS.Movement
{

    public class UnitMoveControlComponentAuthoring : MonoBehaviour
    {
        [NonSerialized] public bool MoveUnits;
        [NonSerialized] public float3 TargetPosition;

        public class UnitMoveControlComponentBaker : Baker<UnitMoveControlComponentAuthoring>
        {
            public override void Bake(UnitMoveControlComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity,
                    new UnitMoveControlComponent
                    {
                        MoveUnits = authoring.MoveUnits, TargetPosition = authoring.TargetPosition
                    });
            }
        }
    }
}