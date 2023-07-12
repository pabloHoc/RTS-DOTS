using RTS.Camera;
using Unity.Entities;
using UnityEngine;

namespace RTS.Authoring.Camera
{
    public class CameraRigSingletonAuthoring : MonoBehaviour
    {
        public class CameraRigSingletonBaker : Baker<CameraRigSingletonAuthoring>
        {
            public override void Bake(CameraRigSingletonAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CameraRigSingleton());
            }
        }
    }
}