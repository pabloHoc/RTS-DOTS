using Unity.Entities;

namespace RTS.SystemGroups
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class UISystemGroup : ComponentSystemGroup
    {
        
    }
}