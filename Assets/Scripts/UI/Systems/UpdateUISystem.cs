using RTS.Gameplay.Player;
using RTS.Gameplay.Resources;
using RTS.SystemGroups;
using Unity.Burst;
using Unity.Entities;

namespace RTS.UI
{
    [UpdateInGroup(typeof(UISystemGroup))]
    public partial struct UpdateUISystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        public void OnUpdate(ref SystemState state)
        {
            UpdateResources(ref state);
        }

        private void UpdateResources(ref SystemState state)
        {
            var resourceText = "";
            
            // This can be turn into a job, maybe
            foreach (var (_, player) in SystemAPI.Query<PlayerComponent>().WithEntityAccess())
            {
                var resources = SystemAPI.GetBuffer<ResourceBufferElement>(player);

                foreach (var resource in resources)
                {
                    resourceText += $"{resource.Name}: {resource.Quantity} |";
                }
            }    
            
            GameUI.Instance.UpdateResources(resourceText);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}