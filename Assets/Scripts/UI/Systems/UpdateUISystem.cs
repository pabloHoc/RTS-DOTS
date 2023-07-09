using RTS.Gameplay.Players;
using RTS.Gameplay.Players.Singletons;
using RTS.Gameplay.Resources;
using RTS.SystemGroups;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace RTS.UI
{
    [UpdateInGroup(typeof(UISystemGroup))]
    public partial struct UpdateUISystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ResourceConfigSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            UpdateResources(ref state);
        }

        private void UpdateResources(ref SystemState state)
        {
            var resourceText = "";
            var player = SystemAPI.GetSingletonEntity<HumanPlayerSingleton>();
            var resources = SystemAPI.GetBuffer<ResourceBufferElement>(player);
            
            // This can be turn into a job, maybe
            foreach (var resource in resources)
            {
                resourceText += $"{resource.Name}: {resource.Value} |";
            }
            
            GameUI.Instance.UpdateResources(resourceText);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}