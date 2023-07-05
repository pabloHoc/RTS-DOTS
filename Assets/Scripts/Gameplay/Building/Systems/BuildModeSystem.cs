using RTS.Building;
using RTS.GameState;
using RTS.Input;
using RTS.UI;
using Unity.Entities;
using UnityEngine;

namespace RTS.Gameplay.Building
{
    public partial struct BuildModeSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameStateComponent>();
            state.RequireForUpdate<InputComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var gameState = SystemAPI.GetSingletonEntity<GameStateComponent>();
            // var buildMode = SystemAPI.GetC
            var input = SystemAPI.GetSingleton<InputComponent>();

            if (GameUI.Instance.BuildButtonClicked && !SystemAPI.IsComponentEnabled<BuildModeTag>(gameState))
            {
                Debug.Log("Build mode entered");
                // gameState.ValueRW.GameState = GameState.GameState.Building;
                SystemAPI.SetComponentEnabled<BuildModeTag>(gameState, true);
            }

            if (input.CancelActionPressed)
            {
                Debug.Log("Build mode exited");
                // gameState.ValueRW.GameState = GameState.GameState.Playing;
                SystemAPI.SetComponentEnabled<BuildModeTag>(gameState, false);
            }
        }

        public void OnDestroy(ref SystemState state)
        {

        }
    }
}