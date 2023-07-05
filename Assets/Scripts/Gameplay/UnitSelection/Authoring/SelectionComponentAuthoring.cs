using System;
using RTS.Gameplay.UnitSelection.Singletons;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace RTS.Gameplay.UnitSelection
{
    public class SelectionComponentAuthoring : MonoBehaviour
    {
        [NonSerialized] public float3 StartPosition;
        [NonSerialized] public float3 EndPosition;
        [NonSerialized] public bool IsActive;
        [NonSerialized] public bool KeepCurrentlySelected;
        [NonSerialized] public Entity SelectedEntity;

        public class SelectionComponentBaker : Baker<SelectionComponentAuthoring>
        {
            public override void Bake(SelectionComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity,
                    new SelectionComponent
                    {
                        StartPosition = authoring.StartPosition, 
                        EndPosition = authoring.EndPosition, 
                        IsActive = authoring.IsActive,
                        KeepCurrentlySelected = authoring.KeepCurrentlySelected,
                        SelectedEntity = authoring.SelectedEntity
                    });
            }
        }
    }
}