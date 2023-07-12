using System;
using RTS.Camera;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace RTS.Authoring.Camera
{
    public class CameraMovementSingletonAuthoring : MonoBehaviour
    {
        [NonSerialized] public float2 Movement;
        [NonSerialized] public float2 Rotation;
        [NonSerialized] public float Zoom;

        public class CameraMovementSingletonBaker : Baker<CameraMovementSingletonAuthoring>
        {
            public override void Bake(CameraMovementSingletonAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CameraMovementSingleton
                {
                    Movement = authoring.Movement,
                    Rotation = authoring.Rotation,
                    Zoom = authoring.Zoom
                });
            }
        }
    }
}