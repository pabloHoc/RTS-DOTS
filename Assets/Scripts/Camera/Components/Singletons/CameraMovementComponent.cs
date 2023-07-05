using Unity.Entities;
using Unity.Mathematics;

namespace RTS.Camera
{
    public struct CameraMovementComponent : IComponentData
    {
       public float2 Movement;
       public float2 Rotation;
       public float Zoom;
    }
}