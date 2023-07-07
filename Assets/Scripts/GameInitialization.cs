using RTS.Gameplay.UnitSelection.Singletons;
using RTS.GameState;
using RTS.Input;
using Unity.Entities;
using UnityEngine;

namespace RTS
{
    public class GameInitialization : MonoBehaviour
    {
        public void Awake()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            // Game State
            var gameState = entityManager.CreateEntity();
            entityManager.AddComponent<GameStateSingleton>(gameState);
            entityManager.AddComponent<BuildModeTag>(gameState);
            entityManager.SetComponentEnabled<BuildModeTag>(gameState, false);

            // Input
            entityManager.CreateSingleton(new InputSingleton());
            
            // Selection
            entityManager.CreateSingleton(new SelectionSingleton());
        }
    }
}