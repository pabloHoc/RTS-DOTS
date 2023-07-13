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

        private int _currentSelectedUnitId = -1;
        
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
            _unitProductionGroupBox = _root.Q<GroupBox>("BuildingGroupBox");
            
            GenerateUI();
        }
        
        // UI Generation

        private void GenerateUI()
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
            Debug.Log($"CLICKED {i}");
            BuildButtonClicked = true;
            BuildingIndex = i;
        }

        // Updates
        
        public void UpdateResourceLabels(string resources)
        {
            _resourcesLabel.text = resources;
        }

        // TODO: we shouldn't have buffer elements here
        public void UpdateUnitButtons(int builderUnitId)
        {
            if (builderUnitId == _currentSelectedUnitId)
            {
                return;
            }

            _currentSelectedUnitId = builderUnitId;
            
            _unitProductionGroupBox.Clear();
            // cache this
            var unitDatabase = GameObject.Find("UnitDatabase").GetComponent<UnitDatabaseAuthoring>().UnitsData;
            var builderUnitData = unitDatabase.Units[builderUnitId];

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
                
                var temp = buildableUnitId;
                unitButton.clicked += () => HandleBuildButtonClicked(temp);
                _unitProductionGroupBox.Add(unitButton);
            }
        }

        public void ClearUnitProductionMenu()
        {
            _unitProductionGroupBox.Clear();
            _currentSelectedUnitId = -1;
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