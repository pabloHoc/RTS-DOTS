using Unity.Entities;
using UnityEngine;

namespace RTS.Common
{
    public class SyncUpTransformComponentAuthoring : MonoBehaviour
    {
        public GameObject GameObject;

        public class SyncUpTransformComponentBaker : Baker<SyncUpTransformComponentAuthoring>
        {
            public override void Bake(SyncUpTransformComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponentObject(entity, new SyncUpTransformComponent { GameObject = authoring.GameObject });
            }
        }
    }
}