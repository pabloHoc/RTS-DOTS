using Unity.Entities;
using UnityEngine;

namespace RTS.Camera
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