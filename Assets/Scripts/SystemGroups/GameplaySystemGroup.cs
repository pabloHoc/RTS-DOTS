using Unity.Entities;

namespace RTS.SystemGroups
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class GameplaySystemGroup : ComponentSystemGroup
    {
        
    }
}