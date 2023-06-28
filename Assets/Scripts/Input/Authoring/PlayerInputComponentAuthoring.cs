using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace RTS.Input
{

    public class PlayerInputComponentAuthoring : MonoBehaviour
    {
        [NonSerialized] public bool PrimaryActionPressed;
        [NonSerialized] public bool SecondaryActionPressed;
        [NonSerialized] public bool OrbitCameraPressed;
        [NonSerialized] public bool DragCameraPressed;
        [NonSerialized] public float2 CameraMovementInput;
        [NonSerialized] public float CameraRotationInput;
        [NonSerialized] public float CameraZoomLevelInput;
        [NonSerialized] public float2 CursorPosition;
        [NonSerialized] public float2 CursorDelta;

        public class PlayerInputComponentBaker : Baker<PlayerInputComponentAuthoring>
        {
            public override void Bake(PlayerInputComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity,
                    new PlayerInputComponent
                    {
                        PrimaryActionPressed = authoring.PrimaryActionPressed,
                        SecondaryActionPressed = authoring.SecondaryActionPressed,
                        OrbitCameraPressed = !authoring.OrbitCameraPressed,
                        DragCameraPressed = authoring.DragCameraPressed,
                        CameraMovementInput = authoring.CameraMovementInput,
                        CameraRotationInput = authoring.CameraRotationInput,
                        CameraZoomLevelInput = authoring.CameraZoomLevelInput,
                        CursorPosition = authoring.CursorPosition,
                        CursorDelta = authoring.CursorDelta
                    });
            }
        }
    }
}