using RTS.SystemGroups;
using Unity.Entities;

namespace RTS.UI
{
    [UpdateInGroup(typeof(UISystemGroup))]
    public partial struct ResetUIStateSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            
        }

        public void OnUpdate(ref SystemState state)
        {
            GameUI.Instance.ResetUIState();
        }

        public void OnDestroy(ref SystemState state)
        {

        }
    }
}