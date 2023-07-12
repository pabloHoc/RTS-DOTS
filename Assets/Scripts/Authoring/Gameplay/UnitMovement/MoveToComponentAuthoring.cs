using RTS.Gameplay.UnitMovement;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace RTS.Authoring.Gameplay.UnitMovement
{
    public class MoveToComponentAuthoring : MonoBehaviour
    {
        public float3 TargetPosition;

        public class MoveToComponentBaker : Baker<MoveToComponentAuthoring>
        {
            public override void Bake(MoveToComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MoveToComponent { TargetPosition = authoring.TargetPosition });
            }
        }
    }
}