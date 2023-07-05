using Unity.Entities;
using UnityEngine;

namespace RTS.GameState
{
    public enum GameState
    {
        Paused,
        Building, 
        Playing
    }
    
    public struct GameStateComponent : IComponentData
    {
        public GameState GameState;
    }
}