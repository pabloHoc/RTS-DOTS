using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTS.Input
{
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

        protected override void OnCreate()
        {
            _inputActions = new InputActions();
            _inputActions.Player.SetCallbacks(this);
        }

        protected override void OnStartRunning() => _inputActions.Enable();

        protected override void OnStopRunning() => _inputActions.Disable();

        protected override void OnUpdate()
        {
            SystemAPI.SetSingleton(new InputComponent
            {
                PrimaryActionPressed = _primaryActionPressed,
                SecondaryActionPressed = _secondaryActionPressed,
                SelectMultipleUnitsPressed = _selectMultipleUnitsPressed,
                CursorScreenPosition = _cursorPosition,
                CursorDelta = _cursorDelta,
                CursorWorldPosition = ScreenToWorldPoint(),
                DragCameraPressed = _dragCameraPressed,
                OrbitCameraPressed = _orbitCameraPressed,
                CameraMovementInput = _cameraMovementInput,
                CameraRotationInput = _cameraRotationInput,
                CameraZoomLevelInput = _cameraZoomLevelInput
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

        // Helpers

        private float3 ScreenToWorldPoint()
        {
            if (!UnityEngine.Camera.main)
            {
                Debug.LogError("Main camera missing!");
            }

            var mousePos = Mouse.current.position.ReadValue();
            var ray = UnityEngine.Camera.main.ScreenPointToRay(mousePos);

            var hit = Raycast(ray.origin, ray.origin + ray.direction * 100);

            // Debug.Log($"Hit Position: {hit.Position} | Hit Entity: {hit.Entity} | Ray Origin: {ray.origin} | Ray Direction: {ray.direction}");

            return hit.Position;
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
