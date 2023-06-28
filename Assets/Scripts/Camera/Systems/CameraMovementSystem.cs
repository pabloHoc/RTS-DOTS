using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/*
 * TODO: To improve camera
 * - Drag speed according to floor proximity
 * - Prevent actions when performing others
 * - Prevent orbital panning to go under floor
 * - Stop zoom when close to objects
 * - Zoom to mouse point
 * - Adjust zoom height to terrain height
 * - Jobs?
 * - Wait time for edge movement
 * - Check use of DeltaTime
 * - Smooth rotation
 * - Reset speed when changing direction
 */
namespace RTS.Camera
{
    [BurstCompile]
    public partial struct CameraMovementSystem : ISystem
    {
        private float _horizontalSpeed;
        private float3 _horizontalDirection;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CameraSettingsComponent>();
            state.RequireForUpdate<CameraMovementComponent>();
        }
    
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var camera = SystemAPI.GetSingleton<CameraMovementComponent>();
            var cameraSettings = SystemAPI.GetSingleton<CameraSettingsComponent>();
            
            foreach (var (rigTransform, rigLocalToWorld) in SystemAPI.Query<RefRW<LocalTransform>, LocalToWorld>()
                         .WithAll<CameraRigTag>())
            {
                var isMoving = math.length(camera.Movement) > 0.1f;

                // Update horizontal direction and speed
                if (isMoving)
                {
                    _horizontalDirection = camera.Movement.x * rigLocalToWorld.Right +
                                           camera.Movement.y * rigLocalToWorld.Forward;
                    _horizontalSpeed += cameraSettings.Acceleration;
                }
                else
                {
                    _horizontalSpeed -= cameraSettings.Deceleration;
                }

                // Clamp speed
                _horizontalSpeed = math.clamp(_horizontalSpeed, 0f, cameraSettings.MaxMovementSpeed);

                // Update position
                rigTransform.ValueRW.Position += _horizontalDirection * _horizontalSpeed * SystemAPI.Time.DeltaTime;

                // Update Rotation
                rigTransform.ValueRW.Rotation = quaternion.EulerXYZ(
                    -camera.Rotation.y * cameraSettings.RotationSpeed,
                    camera.Rotation.x * cameraSettings.RotationSpeed,
                    0f);
            }

            foreach (var cameraTransform in SystemAPI.Query<RefRW<LocalTransform>>()
                         .WithAll<MainCameraTag>())
            {
                // Update Zoom
                cameraTransform.ValueRW.Position += cameraTransform.ValueRW.Forward() * camera.Zoom * cameraSettings.ZoomSpeed * SystemAPI.Time.DeltaTime;
            }
        }

        
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
