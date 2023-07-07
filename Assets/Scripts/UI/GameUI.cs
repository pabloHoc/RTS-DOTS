using UnityEngine;
using UnityEngine.UIElements;

namespace RTS.UI
{
    public class GameUI : MonoBehaviour
    {
        public static GameUI Instance { get; private set; }

        public bool BuildButtonClicked;

        private void Awake() 
        { 
            // If there is an instance, and it's not me, delete myself.
    
            if (Instance != null && Instance != this) 
            { 
                Destroy(this); 
            } 
            else 
            { 
                Instance = this; 
            } 
        }
        
        private void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();

            var buildButton = uiDocument.rootVisualElement.Q<Button>("BuildButton");
            
            buildButton.clicked += HandleBuildButtonClicked;
        }

        private void HandleBuildButtonClicked()
        {
            Debug.Log("Clicked");
            BuildButtonClicked = true;
        }

        public void ResetUIState()
        {
            BuildButtonClicked = false;
        }

    }
}