using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace RTS.Camera
{
    public class CameraMovementComponentAuthoring : MonoBehaviour
    {
        [NonSerialized] public float2 Movement;
        [NonSerialized] public float2 Rotation;
        [NonSerialized] public float Zoom;

        public class CameraMovementComponentBaker : Baker<CameraMovementComponentAuthoring>
        {
            public override void Bake(CameraMovementComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CameraMovementComponent
                {
                    Movement = authoring.Movement,
                    Rotation = authoring.Rotation,
                    Zoom = authoring.Zoom
                });
            }
        }
    }
}