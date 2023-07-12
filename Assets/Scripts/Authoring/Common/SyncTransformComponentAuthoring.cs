using Unity.Entities;
using UnityEngine;

namespace RTS.Common
{
    public class SyncTransformComponentAuthoring : MonoBehaviour
    {
        public GameObject GameObject;

        public class SyncTransformComponentBaker : Baker<SyncTransformComponentAuthoring>
        {
            public override void Bake(SyncTransformComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponentObject(entity, new SyncTransformComponent { GameObject = authoring.GameObject });
            }
        }
    }
}