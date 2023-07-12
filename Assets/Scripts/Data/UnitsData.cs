using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Data
{
    [Serializable]
    public struct ResourceData
    {
        public string Name;
        public int Value;
    }
    
    [Serializable]
    public struct UnitData
    {
        public string Name;
        public int BuildTime;
        public List<ResourceData> Cost;
        public GameObject Prefab;
        public List<int> BuildableUnitIds;
    }
    
    [CreateAssetMenu(fileName = "UnitsData", menuName = "RTS/Units Data", order = 0)]
    public class UnitsData : ScriptableObject
    {
        public List<UnitData> Buildings;
        public List<UnitData> Units;
    }
}