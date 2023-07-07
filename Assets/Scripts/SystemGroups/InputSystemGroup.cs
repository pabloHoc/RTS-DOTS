using Unity.Entities;

namespace RTS.SystemGroups
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class InputSystemGroup : ComponentSystemGroup
    {
        
    }
}