using Unity.Entities;
using UnityEngine;

namespace RTS.Camera
{
    public class MainCameraTagAuthoring : MonoBehaviour
    {
        public class MainCameraTagBaker : Baker<MainCameraTagAuthoring>
        {
            public override void Bake(MainCameraTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MainCameraTag());
            }
        }
    }
}