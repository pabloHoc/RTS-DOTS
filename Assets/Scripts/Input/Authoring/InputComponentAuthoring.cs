using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace RTS.Input
{

    public class InputComponentAuthoring : MonoBehaviour
    {
        [NonSerialized] public bool IsPrimaryActionPressed;
        [NonSerialized] public bool IsSecondaryActionPressed;
        [NonSerialized] public bool IsSelectMultipleUnitsPressed;
        [NonSerialized] public bool WasPrimaryActionPressedThisFrame;
        [NonSerialized] public bool IsOrbitCameraPressed;
        [NonSerialized] public bool IsDragCameraPressed;
        [NonSerialized] public float2 CameraMovementInput;
        [NonSerialized] public float CameraRotationInput;
        [NonSerialized] public float2 CursorScreenPosition;
        [NonSerialized] public float2 CursorDelta;
        [NonSerialized] public float3 CursorWorldPosition;
        [NonSerialized] public float ScrollAmount;
        [NonSerialized] public Entity EntityHit;
        [NonSerialized] public bool IsCancelActionPressed;

        public class InputComponentBaker : Baker<InputComponentAuthoring>
        {
            public override void Bake(InputComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity,
                    new InputComponent
                    {
                        IsPrimaryActionPressed = authoring.IsPrimaryActionPressed,
                        IsSecondaryActionPressed = authoring.IsSecondaryActionPressed,
                        IsSelectMultipleUnitsPressed = authoring.IsSelectMultipleUnitsPressed,
                        WasPrimaryActionPressedThisFrame = authoring.WasPrimaryActionPressedThisFrame,
                        IsOrbitCameraPressed = !authoring.IsOrbitCameraPressed,
                        IsDragCameraPressed = authoring.IsDragCameraPressed,
                        CameraMovementInput = authoring.CameraMovementInput,
                        CameraRotationInput = authoring.CameraRotationInput,
                        ScrollAmount = authoring.ScrollAmount,
                        CursorScreenPosition = authoring.CursorScreenPosition,
                        CursorDelta = authoring.CursorDelta,
                        CursorWorldPosition = authoring.CursorWorldPosition,
                        EntityHit = authoring.EntityHit,
                        IsCancelActionPressed = authoring.IsCancelActionPressed
                    });
            }
        }
    }
}