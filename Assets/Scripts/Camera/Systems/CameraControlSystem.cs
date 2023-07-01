using RTS.Input;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace RTS.Camera
{
    [BurstCompile]
    public partial struct CameraControlSystem : ISystem
    {
        private float2 _cameraMovement;
        private float2 _cameraRotation;
        private float2 _cameraEdgeMovement;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CameraSettingsComponent>();
            state.RequireForUpdate<InputComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var input = SystemAPI.GetSingleton<InputComponent>();

            if (!IsCursorInsideScreen(input.CursorScreenPosition)) return;

            HandleCameraHorizontalMovement(input);
            HandleCameraRotation(input);
            UpdateCameraMovement(input);
        }

        private static bool IsCursorInsideScreen(float2 cursorPosition) =>
            cursorPosition.x < Screen.width && cursorPosition.x > 0 && cursorPosition.y < Screen.height &&
            cursorPosition.y > 0;

        private void HandleCameraRotation(InputComponent input) 
        {
            _cameraRotation.x += input.CameraRotationInput;
            
            if (input.OrbitCameraPressed)
            {
                _cameraRotation += input.CursorDelta;
            }
        }
        
        private void HandleCameraHorizontalMovement(InputComponent input)
        {
            _cameraMovement = input.CameraMovementInput;
            
            // Edge Movement
            
            var cameraEdgeMovement = float2.zero;
            
            var cameraSettings = SystemAPI.GetSingleton<CameraSettingsComponent>();
            var edgeTolerance = cameraSettings.EdgeTolerance;

            if (input.CursorScreenPosition.x < edgeTolerance * Screen.width)
                cameraEdgeMovement.x = -1;
            else if (input.CursorScreenPosition.x > (1f - edgeTolerance) * Screen.width)
                cameraEdgeMovement.x = 1;

            if (input.CursorScreenPosition.y < edgeTolerance * Screen.height)
                cameraEdgeMovement.y = -1;
            else if (input.CursorScreenPosition.y > (1f - edgeTolerance) * Screen.height)
                cameraEdgeMovement.y = 1;

            // Camera edge movement overrides WASD movement
            if (!cameraEdgeMovement.Equals(float2.zero))
            {
                _cameraMovement = cameraEdgeMovement;
            }

            // Drag movement overrides previous movement
            if (input.DragCameraPressed)
            {
                _cameraMovement = -input.CursorDelta * cameraSettings.DragSpeed;
            }
        }
        
        private void UpdateCameraMovement(InputComponent input)
        {
            SystemAPI.SetSingleton(new CameraMovementComponent
            {
                Movement = _cameraMovement,
                Rotation = _cameraRotation,
                Zoom = input.CameraZoomLevelInput
            });
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}