using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace RTS.Selection
{
    public class SelectionControlComponentAuthoring : MonoBehaviour
    {
        [NonSerialized] public bool IsSelecting;
        [NonSerialized] public float3 CursorPosition;

        public class SelectionControlComponentBaker : Baker<SelectionControlComponentAuthoring>
        {
            public override void Bake(SelectionControlComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity,
                    new SelectionControlComponent
                    {
                        IsSelecting = authoring.IsSelecting,
                        CursorPosition = authoring.CursorPosition
                    });
            }
        }
    }
}
