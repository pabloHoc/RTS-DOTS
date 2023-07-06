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
    public partial class InputSystem : SystemBase, InputActions.IPlayerActions
    {
        private InputActions _inputActions;

        private bool _primaryActionPressed;
        private bool _secondaryActionPressed;

        private bool _selectMultipleUnitsPressed;
        
        private float2 _cursorPosition;
        private float3 _screenToWorldCursorPosition;
        private float2 _cursorDelta;

        private bool _orbitCameraPressed;
        private bool _dragCameraPressed;
        
        private float2 _cameraMovementInput;
        private float2 _cameraEdgeMovementInput;
        private float _cameraRotationInput;
        private float _cameraZoomLevelInput;

        private bool _cancelActionPressed;

        protected override void OnCreate()
        {
            _inputActions = new InputActions();
            _inputActions.Player.SetCallbacks(this);
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
            var raycastHit = GetRaycastHit();
            
            SystemAPI.SetSingleton(new InputComponent
            {
                PrimaryActionPressed = _inputActions.Player.PrimaryAction.WasPressedThisFrame(),
                SecondaryActionPressed = _secondaryActionPressed,
                SelectMultipleUnitsPressed = _selectMultipleUnitsPressed,
                CursorScreenPosition = _cursorPosition,
                CursorDelta = _cursorDelta,
                CursorWorldPosition = raycastHit.Position,
                EntityHit = raycastHit.Entity,
                DragCameraPressed = _dragCameraPressed,
                OrbitCameraPressed = _orbitCameraPressed,
                CameraMovementInput = _cameraMovementInput,
                CameraRotationInput = _cameraRotationInput,
                CameraZoomLevelInput = _cameraZoomLevelInput,
                CancelActionPressed = _cancelActionPressed
            });            
        }

        // Callbacks

        public void OnMoveCamera(InputAction.CallbackContext context)
        {
            _cameraMovementInput = context.ReadValue<Vector2>();
        }

        public void OnRotateCamera(InputAction.CallbackContext context)
        {
            _cameraRotationInput = context.ReadValue<float>();
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            _cameraZoomLevelInput = context.ReadValue<Vector2>().y / 100f;
        }

        public void OnOrbitCamera(InputAction.CallbackContext context)
        {
            _orbitCameraPressed = context.performed;
        }

        public void OnCursorPosition(InputAction.CallbackContext context)
        {
            _cursorPosition = context.ReadValue<Vector2>();
        }

        public void OnCursorDelta(InputAction.CallbackContext context)
        {
            _cursorDelta = context.ReadValue<Vector2>();
        }

        public void OnDragCamera(InputAction.CallbackContext context)
        {
            _dragCameraPressed = context.performed;
        }

        public void OnPrimaryAction(InputAction.CallbackContext context)
        {
            _primaryActionPressed = context.performed;
        }

        public void OnSecondaryAction(InputAction.CallbackContext context)
        {
            _secondaryActionPressed = context.performed;
        }

        public void OnSelectMultipleUnits(InputAction.CallbackContext context)
        {
            _selectMultipleUnitsPressed = context.performed;
        }

        public void OnCancelAction(InputAction.CallbackContext context)
        {
            _cancelActionPressed = context.performed;
        }

        // Helpers

        private RaycastHit GetRaycastHit()
        {
            if (!UnityEngine.Camera.main)
            {
                Debug.LogError("Main camera missing!");
            }

            var ray = UnityEngine.Camera.main.ScreenPointToRay((Vector2)_cursorPosition);

            var hit = Raycast(ray.origin, ray.origin + ray.direction * 100);
            
            // Debug.Log($"Hit Position: {hit.Position} | Hit Entity: {hit.Entity} | Ray Origin: {ray.origin} | Ray Direction: {ray.direction}");

            return hit;
        }

        private Unity.Physics.RaycastHit Raycast(float3 rayFrom, float3 rayTo)
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
