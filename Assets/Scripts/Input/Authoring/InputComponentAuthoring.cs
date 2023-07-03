using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace RTS.Input
{

    public class InputComponentAuthoring : MonoBehaviour
    {
        [NonSerialized] public bool PrimaryActionPressed;
        [NonSerialized] public bool SecondaryActionPressed;
        [NonSerialized] public bool SelectMultipleUnitsPressed;
        [NonSerialized] public bool OrbitCameraPressed;
        [NonSerialized] public bool DragCameraPressed;
        [NonSerialized] public float2 CameraMovementInput;
        [NonSerialized] public float CameraRotationInput;
        [NonSerialized] public float CameraZoomLevelInput;
        [NonSerialized] public float2 CursorScreenPosition;
        [NonSerialized] public float2 CursorDelta;
        [NonSerialized] public float3 CursorWorldPosition;
        [NonSerialized] public Entity EntityHit;

        public class InputComponentBaker : Baker<InputComponentAuthoring>
        {
            public override void Bake(InputComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity,
                    new InputComponent
                    {
                        PrimaryActionPressed = authoring.PrimaryActionPressed,
                        SecondaryActionPressed = authoring.SecondaryActionPressed,
                        SelectMultipleUnitsPressed = authoring.SelectMultipleUnitsPressed,
                        OrbitCameraPressed = !authoring.OrbitCameraPressed,
                        DragCameraPressed = authoring.DragCameraPressed,
                        CameraMovementInput = authoring.CameraMovementInput,
                        CameraRotationInput = authoring.CameraRotationInput,
                        CameraZoomLevelInput = authoring.CameraZoomLevelInput,
                        CursorScreenPosition = authoring.CursorScreenPosition,
                        CursorDelta = authoring.CursorDelta,
                        CursorWorldPosition = authoring.CursorWorldPosition,
                        EntityHit = authoring.EntityHit
                    });
            }
        }
    }
}