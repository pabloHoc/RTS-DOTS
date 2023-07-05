using Unity.Entities;
using UnityEngine;

namespace RTS.GameState
{
    public class GameStateComponentAuthoring : MonoBehaviour
    {
        public GameState GameState;

        public class GameStateComponentBaker : Baker<GameStateComponentAuthoring>
        {
            public override void Bake(GameStateComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new GameStateComponent { GameState = authoring.GameState });
            }
        }
    }
}