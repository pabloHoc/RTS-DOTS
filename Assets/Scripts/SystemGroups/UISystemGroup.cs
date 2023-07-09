using Unity.Entities;

namespace RTS.SystemGroups
{
    // Not sure why can't we update it in PresentationSystemGroup
    // (it resets UI before giving time to gameplay to act)
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class UISystemGroup : ComponentSystemGroup
    {
        
    }
}