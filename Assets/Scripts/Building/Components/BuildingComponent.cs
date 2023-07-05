using Unity.Entities;
using UnityEngine;

namespace RTS.Building
{
    public struct BuildingComponent : IComponentData
    {
        public float BuildingTime;
    }
}