using Unity.Entities;

namespace RTS.Camera
{
    public struct CameraSettingsSingleton : IComponentData
    {
        public float MaxMovementSpeed;
        public float Acceleration;
        public float Deceleration;

        public float EdgeTolerance;
        
        public float RotationSpeed;

        public float DragSpeed;
        
        public float ZoomStepSize;
        public float ZoomDampening;
        public float MinZoomHeight;
        public float MaxZoomHeight;
        public float ZoomSpeed ;
    }
}