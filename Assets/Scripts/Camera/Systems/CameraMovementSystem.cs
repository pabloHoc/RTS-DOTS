using Unity.Burst;
using Unity.Collections;
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
        private NativeReference<float> _horizontalSpeedReference; 
        private NativeReference<float3> _horizontalDirectionReference; 

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CameraSettingsSingleton>();
            state.RequireForUpdate<CameraMovementSingleton>();
            
            _horizontalSpeedReference = new NativeReference<float>(0f, Allocator.Persistent);
            _horizontalDirectionReference = new NativeReference<float3>(float3.zero, Allocator.Persistent);
        }
    
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var camera = SystemAPI.GetSingleton<CameraMovementSingleton>();
            var cameraSettings = SystemAPI.GetSingleton<CameraSettingsSingleton>();
            var deltaTime = SystemAPI.Time.DeltaTime;

            var updateCameraRigPositionJob = new UpdateCameraRigPositionJob
            {
                Camera = camera,
                CameraSettings = cameraSettings,
                DeltaTime = deltaTime,
                HorizontalSpeed = _horizontalSpeedReference,
                HorizontalDirection = _horizontalDirectionReference
            };
            updateCameraRigPositionJob.Schedule();
            
            var updateCameraPositionJob = new UpdateCameraPositionJob
            {
                Camera = camera,
                CameraSettings = cameraSettings,
                DeltaTime = deltaTime
            };
            updateCameraPositionJob.Schedule();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }

    [WithAll(typeof(CameraRigSingleton))]
    [BurstCompile]
    public partial struct UpdateCameraRigPositionJob : IJobEntity
    {
        public CameraMovementSingleton Camera;
        public CameraSettingsSingleton CameraSettings;
        public float DeltaTime;
        public NativeReference<float> HorizontalSpeed;
        public NativeReference<float3> HorizontalDirection;
        
        public void Execute(ref LocalTransform transform, in LocalToWorld localToWorld)
        {
            var isMoving = math.length(Camera.Movement) > 0.1f;

            // Update horizontal direction and speed
            if (isMoving)
            {
                HorizontalDirection.Value = Camera.Movement.x * localToWorld.Right +
                                      Camera.Movement.y * localToWorld.Forward;
                HorizontalSpeed.Value += HorizontalSpeed.Value + CameraSettings.Acceleration;
            }
            else
            {
                HorizontalSpeed.Value -= CameraSettings.Deceleration;
            }

            // Clamp speed
            HorizontalSpeed.Value = math.clamp(HorizontalSpeed.Value, 0f, CameraSettings.MaxMovementSpeed);

            // Update position
            transform.Position += HorizontalDirection.Value * HorizontalSpeed.Value * DeltaTime;

            // Update Rotation
            transform.Rotation = quaternion.EulerXYZ(
                -Camera.Rotation.y * CameraSettings.RotationSpeed,
                Camera.Rotation.x * CameraSettings.RotationSpeed,
                0f);
        }
    }

    [WithAll(typeof(MainCameraSingleton))]
    [BurstCompile]
    public partial struct UpdateCameraPositionJob : IJobEntity
    {
        public CameraMovementSingleton Camera;
        public CameraSettingsSingleton CameraSettings;
        public float DeltaTime;
        
        public void Execute(ref LocalTransform transform)
        {
            transform.Position += transform.Forward() * Camera.Zoom * CameraSettings.ZoomSpeed * DeltaTime;
        }
    }
}
