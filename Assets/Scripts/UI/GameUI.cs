using RTS.Gameplay.Buildings;
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
            var buildingBox = _root.Q<GroupBox>("BuildingBox");
            var buildingsData = GameObject.Find("BuildingsConfig")
                .GetComponent<BuildingDatabaseAuthoring>().BuildingsData;

            for (int i = 0; i < buildingsData.Count; i++)
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

        // Updates
        
        public void UpdateResources(string resources)
        {
            _resourcesLabel.text = resources;
        }

        
        public void ResetUIState()
        {
            BuildButtonClicked = false;
        }

    }
}