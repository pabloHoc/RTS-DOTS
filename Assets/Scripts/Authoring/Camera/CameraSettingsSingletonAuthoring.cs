using RTS.Camera;
using Unity.Entities;
using UnityEngine;

namespace RTS.Authoring.Camera
{
    public class CameraSettingsSingletonAuthoring : MonoBehaviour
    {
        [Header("Horizontal Movement")]
        public float MaxMovementSpeed;
        public float Acceleration;
        public float Deceleration;

        [Header("Edge Movement")]
        public float EdgeTolerance;  
        
        [Header("Rotation")]
        public float RotationSpeed;     
        
        [Header("Drag")]
        public float DragSpeed;

        [Header("Zoom")]
        public float ZoomStepSize;
        public float ZoomDampening;
        public float MinZoomHeight;
        public float MaxZoomHeight;
        public float ZoomSpeed ;
        
        public class CameraSettingsSingletonBaker : Baker<CameraSettingsSingletonAuthoring>
        {
            public override void Bake(CameraSettingsSingletonAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity,
                    new CameraSettingsSingleton
                    {
                        MaxMovementSpeed = authoring.MaxMovementSpeed,
                        Acceleration = authoring.Acceleration,
                        Deceleration =  authoring.Deceleration,
                        EdgeTolerance = authoring.EdgeTolerance,
                        RotationSpeed = authoring.RotationSpeed,
                        DragSpeed = authoring.DragSpeed,
                        ZoomStepSize = authoring.ZoomStepSize,
                        ZoomDampening = authoring.ZoomDampening,
                        MinZoomHeight = authoring.MinZoomHeight,
                        MaxZoomHeight = authoring.MaxZoomHeight,
                        ZoomSpeed = authoring.ZoomSpeed
                    });
            }
        }
    }
}