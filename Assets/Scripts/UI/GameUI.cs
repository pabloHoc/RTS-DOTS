using RTS.Gameplay.Building;
using UnityEngine;
using UnityEngine.UIElements;

namespace RTS.UI
{
    public class GameUI : MonoBehaviour
    {
        public static GameUI Instance { get; private set; }

        public bool BuildButtonClicked;
        public int BuildingIndex;
        
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
            var uiDocument = GetComponent<UIDocument>();
            var buildingBox = uiDocument.rootVisualElement.Q<GroupBox>("BuildingBox");
            
            // Get building data

            var buildingsData = GameObject.Find("BuildingConfig")
                .GetComponent<BuildingConfigSingletonAuthoring>().BuildingsData.ToArray();

            for (int i = 0; i < buildingsData.Length; i++)
            {
                var buildingButton = new Button
                {
                    text = buildingsData[i].Name,
                    userData = new
                    {
                        BuildingIndex = i
                    }
                };
                var i1 = i;
                buildingButton.clicked += () => HandleBuildButtonClicked(i1);
                buildingBox.Add(buildingButton);
            }
        }

        private void HandleBuildButtonClicked(int i)
        {
            Debug.Log($"Clicked {i}");
            BuildButtonClicked = true;
            BuildingIndex = i;
        }

        public void ResetUIState()
        {
            BuildButtonClicked = false;
        }

    }
}