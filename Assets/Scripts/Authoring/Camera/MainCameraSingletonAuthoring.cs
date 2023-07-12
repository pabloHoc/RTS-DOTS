using RTS.Camera;
using Unity.Entities;
using UnityEngine;

namespace RTS.Authoring.Camera
{
    public class MainCameraSingletonAuthoring : MonoBehaviour
    {
        public class MainCameraSingletonBaker : Baker<MainCameraSingletonAuthoring>
        {
            public override void Bake(MainCameraSingletonAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MainCameraSingleton());
            }
        }
    }
}