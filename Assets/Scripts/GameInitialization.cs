using RTS.Gameplay.Players;
using RTS.Gameplay.Players.Singletons;
using RTS.Gameplay.Resources;
using RTS.Gameplay.UnitSelection.Singletons;
using RTS.GameState;
using RTS.Input;
using Unity.Entities;
using UnityEngine;

namespace RTS
{
    public class GameInitialization : MonoBehaviour
    {
        private EntityManager _entityManager;
        
        public void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            // Game State
            var gameState = _entityManager.CreateEntity();
            _entityManager.AddComponent<GameStateSingleton>(gameState);
            _entityManager.AddComponent<BuildModeTag>(gameState);
            _entityManager.SetComponentEnabled<BuildModeTag>(gameState, false);

            // Input
            _entityManager.CreateSingleton(new InputSingleton());
            
            // Selection
            _entityManager.CreateSingleton(new SelectionSingleton());

            CreatePlayer();
        }

        private void CreatePlayer()
        {
            var player = _entityManager.CreateSingleton<HumanPlayerSingleton>();
            _entityManager.AddComponent<PlayerComponent>(player);
            var resourceBuffer = _entityManager.AddBuffer<ResourceBufferElement>(player);

            PopulatePlayerResources(resourceBuffer);
        }

        private static void PopulatePlayerResources(DynamicBuffer<ResourceBufferElement> resourceBuffer)
        {
            var resourcesData = GameObject.Find("ResourcesConfig").GetComponent<ResourceConfigAuthoring>().ResourcesData.ToArray();

            for (var i = 0; i < resourcesData.Length; i++)
            {
                resourceBuffer.Add(new ResourceBufferElement
                {
                    Name = resourcesData[i].Name,
                    Value = resourcesData[i].Value,
                    Type = ResourceType.Stored
                });
            }
        }
    }
}