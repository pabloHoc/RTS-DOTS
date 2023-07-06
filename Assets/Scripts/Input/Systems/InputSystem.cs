using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using RaycastHit = Unity.Physics.RaycastHit;

namespace RTS.Input
{
    // TODO: maybe this should be a singleton and we should have a system that reads from it
    [CreateAfter(typeof(BuildPhysicsWorld))]
    public partial class InputSystem : SystemBase
    {
        private InputActions _inputActions;

        protected override void OnCreate()
        {
            _inputActions = new InputActions();
        }

        protected override void OnStartRunning() {
            _inputActions.Enable();
        }

        protected override void OnStopRunning()
        {
            _inputActions.Disable();
        } 

        protected override void OnUpdate()
        {
            var cursorPosition = _inputActions.Player.CursorPosition.ReadValue<Vector2>();
            var raycastHit = GetRaycastHit(cursorPosition);
            
            SystemAPI.SetSingleton(new InputComponent
            {
                IsPrimaryActionPressed = _inputActions.Player.PrimaryAction.WasPressedThisFrame(),
                IsSecondaryActionPressed = _inputActions.Player.SecondaryAction.WasPressedThisFrame(),
                IsSelectMultipleUnitsPressed = _inputActions.Player.SelectMultipleUnits.WasPressedThisFrame(),
                CursorScreenPosition = cursorPosition,
                CursorDelta = _inputActions.Player.CursorDelta.ReadValue<Vector2>(),
                CursorWorldPosition = raycastHit.Position,
                EntityHit = raycastHit.Entity,
                IsDragCameraPressed = _inputActions.Player.DragCamera.IsPressed(),
                IsOrbitCameraPressed = _inputActions.Player.OrbitCamera.IsPressed(),
                CameraMovementInput = _inputActions.Player.MoveCamera.ReadValue<Vector2>(),
                CameraRotationInput = _inputActions.Player.RotateCamera.ReadValue<float>(),
                ScrollAmount = _inputActions.Player.Zoom.ReadValue<Vector2>().y / 100f,
                IsCancelActionPressed = _inputActions.Player.CancelAction.WasPressedThisFrame()
            });            
        }

        // Helpers

        private RaycastHit GetRaycastHit(Vector2 point)
        {
            if (!UnityEngine.Camera.main)
            {
                Debug.LogError("Main camera missing!");
            }

            var ray = UnityEngine.Camera.main.ScreenPointToRay(point);

            var hit = Raycast(ray.origin, ray.origin + ray.direction * 100);
            
            return hit;
        }

        private RaycastHit Raycast(float3 rayFrom, float3 rayTo)
        {
            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            var input = new RaycastInput
            {
                Start = rayFrom,
                End = rayTo,
                Filter = CollisionFilter.Default
            };

            collisionWorld.CastRay(input, out var hit);

            return hit;
        }
    }
}
