using RTS.Common;
using RTS.Gameplay.Players.Singletons;
using RTS.Gameplay.Resources;
using RTS.GameState;
using RTS.Input;
using RTS.SystemGroups;
using RTS.UI;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace RTS.Gameplay.Buildings
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct BuildingConstructionSystem : ISystem
    {
        private float _currentBuildingRotation;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildingDatabaseSingleton>();
            state.RequireForUpdate<GameStateSingleton>();
            state.RequireForUpdate<InputSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var gameState = SystemAPI.GetSingletonEntity<GameStateSingleton>();

            var buildingDatabase = SystemAPI.GetSingleton<BuildingDatabaseSingleton>().Data;
            var buildingRepository = SystemAPI.GetSingletonEntity<BuildingDatabaseSingleton>();
            var buildingEntityBuffer = SystemAPI.GetBuffer<EntityBufferElement>(buildingRepository);
            
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            
            var input = SystemAPI.GetSingleton<InputSingleton>();

            var player = SystemAPI.GetSingletonEntity<HumanPlayerSingleton>();

            if (GameUI.Instance.BuildButtonClicked && !SystemAPI.IsComponentEnabled<BuildModeTag>(gameState))
            {
                SystemAPI.SetComponentEnabled<BuildModeTag>(gameState, true);
                var buildingEntity = state.EntityManager.Instantiate(buildingEntityBuffer[GameUI.Instance.BuildingIndex].Entity);
                ecb.AddComponent<BuildingPositioningTag>(buildingEntity);
            }
            
            if (SystemAPI.IsComponentEnabled<BuildModeTag>(gameState))
            {
                var building = buildingEntityBuffer[GameUI.Instance.BuildingIndex].Entity;
                    
                _currentBuildingRotation += input.ScrollAmount * 2f;
                
                var job = new BuildBuildingJob
                {
                    Ecb = ecb.AsParallelWriter(),
                    Player = player,
                    Building = building,
                    BuildingPosition = input.CursorWorldPosition,
                    BuildingRotation = _currentBuildingRotation,
                    BuildBuilding = input.WasPrimaryActionPressedThisFrame,
                    CancelBuilding = input.IsCancelActionPressed,
                    BuildingDatabase = buildingDatabase
                };

                if (input.IsCancelActionPressed)
                {
                    SystemAPI.SetComponentEnabled<BuildModeTag>(gameState, false);
                }

                state.Dependency = job.ScheduleParallel(state.Dependency);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
    
        [WithAll(typeof(BuildingComponent), typeof(BuildingPositioningTag))]
        [BurstCompile]
        public partial struct BuildBuildingJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter Ecb;
            public Entity Player;
            public Entity Building;
            public float3 BuildingPosition;
            public bool BuildBuilding;
            public bool CancelBuilding;
            public float BuildingRotation;
            public BlobAssetReference<BuildingsData> BuildingDatabase;

            [BurstCompile]
            public void Execute(
                [ChunkIndexInQuery] int index,
                Entity entity,
                ref LocalTransform transform,
                in WorldRenderBounds renderBounds)
            {
                transform.Position = BuildingPosition;
                transform.Position.y = renderBounds.Value.Center.y;
                transform.Rotation = quaternion.RotateY(BuildingRotation); 
                
                if (BuildBuilding)
                {
                    ref var buildingData = ref BuildingDatabase.Value.Buildings[0];
                    
                    var instantiatedBuilding = Ecb.Instantiate(index, Building);
                    // Add transform
                    Ecb.AddComponent<LocalTransform>(index, instantiatedBuilding);
                    Ecb.SetComponent(index, instantiatedBuilding, transform);
                    // Add ownership
                    Ecb.AddComponent<EntityOwnerComponent>(index, instantiatedBuilding);
                    Ecb.SetComponent(index, instantiatedBuilding, new EntityOwnerComponent
                    {
                        Entity = Player
                    });

                    // Add created tag
                    Ecb.AddComponent<EntityCreatedTag>(index, instantiatedBuilding);
                }

                if (CancelBuilding)
                {
                    Ecb.DestroyEntity(index, entity);
                }
            }
        }
}