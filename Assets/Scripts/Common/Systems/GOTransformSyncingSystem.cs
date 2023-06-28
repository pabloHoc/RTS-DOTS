using Unity.Entities;
using Unity.Transforms;

namespace RTS.Common
{
    public partial struct GOTransformSyncingSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (transform, component) in SystemAPI.Query<LocalToWorld, SyncUpTransformComponent>())
            {
                component.GameObject.transform.position = transform.Position;
                component.GameObject.transform.rotation = transform.Rotation;
            }
        }

        public void OnDestroy(ref SystemState state)
        {

        }
    }
}