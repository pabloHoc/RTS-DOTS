using RTS.Authoring.Gameplay.Unit;
using RTS.Common;
using RTS.Data;
using RTS.Gameplay.Units;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace RTS.UI
{
    public class GameUI : MonoBehaviour
    {
        public static GameUI Instance { get; private set; }
        public bool BuildButtonClicked { get; private set; }
        public int BuildingIndex { get; private set; }
        public bool IsMouseOverUI { get; private set; }

        private VisualElement _root;
        private Label _resourcesLabel;
        private GroupBox _unitProductionGroupBox;
        
        private void Awake() 
        { 
            if (Instance != null && Instance != this) 
            { 
                Destroy(this); 
            } 
            else 
            { 
                Instance = this; 
            } 
        }
        
        private void Start()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            
            _root.RegisterCallback<MouseEnterEvent>(HandleMouseEnterUI);
            _root.RegisterCallback<MouseLeaveEvent>(HandleMouseLeaveUI);

            _resourcesLabel = _root.Q<Label>("ResourcesLabel");
            _unitProductionGroupBox = _root.Q<GroupBox>("UnitProductionGroupBox");

            GenerateUI();
        }
        
        // UI Generation

        private void GenerateUI()
        {
            GenerateBuildingButtons();
            GenerateResourceLabels();
        }

        private void GenerateBuildingButtons()
        {
            var buildingBox = _root.Q<GroupBox>("BuildingGroupBox");
            var buildingsData = GameObject.Find("UnitDatabase").GetComponent<UnitDatabaseAuthoring>().UnitsData.Buildings;

            for (var i = 0; i < buildingsData.Count; i++)
            {
                var buildingButton = new Button
                {
                    text = buildingsData[i].Name,
                    userData = new
                    {
                        BuildingIndex = i
                    }
                };
                var temp = i;
                buildingButton.clicked += () => HandleBuildButtonClicked(temp);
                buildingBox.Add(buildingButton);
            }
        }
        
        private void GenerateResourceLabels()
        {
            
        }
        
        // Handlers

        private void HandleMouseLeaveUI(MouseLeaveEvent evt)
        {
            IsMouseOverUI = false;
        }

        private void HandleMouseEnterUI(MouseEnterEvent evt)
        {
            IsMouseOverUI = true;
        }

        private void HandleBuildButtonClicked(int i)
        {
            BuildButtonClicked = true;
            BuildingIndex = i;
        }

        private void HandleBuildUnitClicked(int i)
        {
            Debug.Log($"Build Unit {i}");
        }   

        // Updates
        
        public void UpdateResourceLabels(string resources)
        {
            _resourcesLabel.text = resources;
        }

        // TODO: we shouldn't have buffer elements here
        public void UpdateUnitButtons(int builderUnitId)
        {
            var unitBox = _root.Q<GroupBox>("UnitGroupBox");
            unitBox.Clear();
            // cache this
            var unitDatabase = GameObject.Find("UnitDatabase").GetComponent<UnitDatabaseAuthoring>().UnitsData;
            var builderUnitData = unitDatabase.Buildings[builderUnitId];

            Debug.Log($"UNIT SELECTED {builderUnitData.BuildableUnitIds.Count}");

            for (var i = 0; i < builderUnitData.BuildableUnitIds.Count; i++)
            {
                var buildableUnitId = builderUnitData.BuildableUnitIds[i];
                var buildableUnitData = unitDatabase.Units[buildableUnitId];
                
                var unitButton = new Button
                {
                    text = buildableUnitData.Name,
                    userData = new
                    {
                        UnitIndex = buildableUnitData
                    }
                };
                
                var temp = i;
                unitButton.clicked += () => HandleBuildUnitClicked(temp);
                unitBox.Add(unitButton);
            }
        }
        
        public void ShowUnitProductionMenu()
        {
            _unitProductionGroupBox.visible = true;
        }
        
        public void HideUnitProductionMenu()
        {
            _unitProductionGroupBox.visible = false;
        }

        public void ResetUIState()
        {
            BuildButtonClicked = false;
        }

    }
}