using UnityEngine;
using UnityEngine.UIElements;

public class SceneManager : MonoBehaviour
{
    private VisualTreeAsset main_menu;
    private VisualTreeAsset level_selection_menu;
    private VisualTreeAsset pause_menu;
    private VisualTreeAsset level;
    private void OnEnable()
    {
        UIDocument uIDocument = GetComponent<UIDocument>();
        VisualElement root = uIDocument.rootVisualElement;

        
    }
}
