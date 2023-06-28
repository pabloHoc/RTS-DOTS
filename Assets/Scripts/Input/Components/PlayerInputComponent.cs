using Unity.Entities;
using Unity.Mathematics;

namespace RTS.Input
{
    public struct PlayerInputComponent : IComponentData
    {
        public bool PrimaryActionPressed;
        public bool SecondaryActionPressed;
        
        public bool OrbitCameraPressed; 
        public bool DragCameraPressed;

        public float2 CameraMovementInput;
        public float CameraRotationInput;
        public float CameraZoomLevelInput;

        public float2 CursorPosition;
        public float2 CursorDelta;
    }
}