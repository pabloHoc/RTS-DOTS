using Unity.Entities;
using Unity.Mathematics;

namespace RTS.Input
{
    public struct InputComponent : IComponentData
    {
        public bool IsPrimaryActionPressed;
        public bool IsSecondaryActionPressed;
        public bool IsSelectMultipleUnitsPressed;
        
        public bool WasPrimaryActionPressedThisFrame;
        
        public bool IsOrbitCameraPressed; 
        public bool IsDragCameraPressed;

        public float2 CameraMovementInput;
        public float CameraRotationInput;

        public float2 CursorScreenPosition;
        public float2 CursorDelta;
        public float3 CursorWorldPosition;
        public float ScrollAmount;
        
        public Entity EntityHit;

        public bool IsCancelActionPressed;
    }
}