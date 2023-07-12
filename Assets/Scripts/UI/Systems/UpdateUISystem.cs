using RTS.Gameplay.Players.Singletons;
using RTS.Gameplay.Resources;
using RTS.Gameplay.UnitBuilding;
using RTS.Gameplay.Units;
using RTS.Gameplay.UnitSelection;
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
            state.RequireForUpdate<HumanPlayerSingleton>();
            state.RequireForUpdate<ResourceConfigSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            UpdateResources(ref state);
            UpdateUnitProductionMenu(ref state);
        }

        private void UpdateResources(ref SystemState state)
        {
            var resourceText = "";
            var player = SystemAPI.GetSingletonEntity<HumanPlayerSingleton>();
            var resources = SystemAPI.GetBuffer<ResourceBufferElement>(player);
            
            // This can be turn into a job, maybe
            foreach (var resource in resources)
            {
                if (resource.Type == ResourceType.Stored)
                {
                    resourceText += $"{resource.Name}: {resource.Value} |";
                }
            }
            
            GameUI.Instance.UpdateResourceLabels(resourceText);
        }

        private void UpdateUnitProductionMenu(ref SystemState state)
        {
            ref var unitDatabase = ref SystemAPI.GetSingleton<UnitDatabaseSingleton>().Data.Value;

            foreach (var (_, building) in SystemAPI.Query<SelectedUnitTag, BuilderComponent>())
            {
                // ref var buildingData = ref unitDatabase.Units[building.Index];
                // GameUI.Instance.UpdateUnitButtons(buildingData.UnitIds.ToArray());
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}