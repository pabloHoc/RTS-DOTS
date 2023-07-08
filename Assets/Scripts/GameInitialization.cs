using RTS.Gameplay.Player;
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
            var player = _entityManager.CreateEntity();
            _entityManager.AddComponent<PlayerComponent>(player);
            var resourceBuffer = _entityManager.AddBuffer<ResourceBufferElement>(player);

            PopulatePlayerResources(resourceBuffer);
        }

        private void PopulatePlayerResources(DynamicBuffer<ResourceBufferElement> resourceBuffer)
        {
            var resourcesData = GameObject.Find("ResourcesConfig").GetComponent<ResourceConfigSingletonAuthoring>().ResourcesData;

            foreach (var resourceData in resourcesData)
            {
                resourceBuffer.Add(new ResourceBufferElement
                {
                    Name = resourceData.Name,
                    Quantity = resourceData.InitialQuantity
                });
            }
        }
    }
}