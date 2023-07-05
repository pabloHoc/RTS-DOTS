using RTS.GameState;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace RTS.Building
{
    public partial struct BuildingInstantiationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildingReferencesComponent>();
            state.RequireForUpdate<GameStateComponent>();
            state.RequireForUpdate<BuildModeTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var gameState = SystemAPI.GetSingletonEntity<GameStateComponent>();
            var buildingReferences = SystemAPI.GetSingleton<BuildingReferencesComponent>();

            if (SystemAPI.IsComponentEnabled<BuildModeTag>(gameState))
            {
                state.EntityManager.Instantiate(buildingReferences.SomeBuilding);
                SystemAPI.SetComponentEnabled<BuildModeTag>(gameState, false);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}