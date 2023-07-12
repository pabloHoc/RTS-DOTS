using RTS.Gameplay.UnitMovement;
using Unity.Entities;
using UnityEngine;

namespace RTS.Authoring.Gameplay.UnitMovement
{
    public class MovableComponentAuthoring : MonoBehaviour
    {
        public float Speed;

        public class MovableComponentBaker : Baker<MovableComponentAuthoring>
        {
            public override void Bake(MovableComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MovableComponent { Speed = authoring.Speed });
            }
        }
    }
}