using RTS.Input;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace RTS.Camera
{
    public partial struct CameraInputControlSystem : ISystem
    {
        private float2 _cameraMovement;
        private float2 _cameraRotation;
        private float2 _cameraEdgeMovement;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CameraSettingsComponent>();
            state.RequireForUpdate<PlayerInputComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var input = SystemAPI.GetSingleton<PlayerInputComponent>();

            if (!IsCursorInsideScreen(input.CursorPosition)) return;

            HandleCameraHorizontalMovement(input);
            HandleCameraRotation(input);
            UpdateCameraMovement(input);
        }

        private static bool IsCursorInsideScreen(float2 cursorPosition) =>
            cursorPosition.x < Screen.width && cursorPosition.x > 0 && cursorPosition.y < Screen.height &&
            cursorPosition.y > 0;

        private void HandleCameraRotation(PlayerInputComponent playerInput) 
        {
            _cameraRotation.x += playerInput.CameraRotationInput;
            
            if (playerInput.OrbitCameraPressed)
            {
                _cameraRotation += playerInput.CursorDelta;
            }
        }
        
        private void HandleCameraHorizontalMovement(PlayerInputComponent playerInput)
        {
            _cameraMovement = playerInput.CameraMovementInput;
            
            // Edge Movement
            
            var cameraEdgeMovement = float2.zero;
            
            var cameraSettings = SystemAPI.GetSingleton<CameraSettingsComponent>();
            var edgeTolerance = cameraSettings.EdgeTolerance;

            if (playerInput.CursorPosition.x < edgeTolerance * Screen.width)
                cameraEdgeMovement.x = -1;
            else if (playerInput.CursorPosition.x > (1f - edgeTolerance) * Screen.width)
                cameraEdgeMovement.x = 1;

            if (playerInput.CursorPosition.y < edgeTolerance * Screen.height)
                cameraEdgeMovement.y = -1;
            else if (playerInput.CursorPosition.y > (1f - edgeTolerance) * Screen.height)
                cameraEdgeMovement.y = 1;

            // Camera edge movement overrides WASD movement
            if (!cameraEdgeMovement.Equals(float2.zero))
            {
                _cameraMovement = cameraEdgeMovement;
            }

            // Drag movement overrides previous movement
            if (playerInput.DragCameraPressed)
            {
                _cameraMovement = -playerInput.CursorDelta * cameraSettings.DragSpeed;
            }
        }
        
        private void UpdateCameraMovement(PlayerInputComponent playerInput)
        {
            SystemAPI.SetSingleton(new CameraMovementComponent
            {
                Movement = _cameraMovement,
                Rotation = _cameraRotation,
                Zoom = playerInput.CameraZoomLevelInput
            });
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}