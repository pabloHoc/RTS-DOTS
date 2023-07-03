using Unity.Entities;
using Unity.Mathematics;

namespace RTS.Input
{
    public struct InputComponent : IComponentData
    {
        public bool PrimaryActionPressed;
        public bool SecondaryActionPressed;
        public bool SelectMultipleUnitsPressed;
        
        public bool OrbitCameraPressed; 
        public bool DragCameraPressed;

        public float2 CameraMovementInput;
        public float CameraRotationInput;
        public float CameraZoomLevelInput;

        public float2 CursorScreenPosition;
        public float2 CursorDelta;
        public float3 CursorWorldPosition;
    }
}