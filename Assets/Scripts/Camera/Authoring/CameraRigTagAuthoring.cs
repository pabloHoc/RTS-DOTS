using Unity.Entities;
using UnityEngine;

namespace RTS.Camera
{
    public class CameraRigTagAuthoring : MonoBehaviour
    {
        public class CameraRigTagBaker : Baker<CameraRigTagAuthoring>
        {
            public override void Bake(CameraRigTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CameraRigTag());
            }
        }
    }
}