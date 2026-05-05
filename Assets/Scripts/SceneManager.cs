using NUnit.Framework;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour
{
    private VisualElement root;

    private VisualTreeAsset main_menu;
    private VisualTreeAsset level_selection_menu;
    private VisualTreeAsset pause_menu;
    private VisualTreeAsset level;

    private enum State
    {
        MainMenu,
        LevelSelectionMenu,
        PauseMenu,
        Level
    }

    private VisualElement current_scene;

    List<Level> level_resources;

    private void OnEnable()
    {
        level_resources = Resources.LoadAll<Level>("LevelResources/").ToList();

        main_menu = Resources.Load<VisualTreeAsset>("Scenes/MainMenu");
        level_selection_menu = Resources.Load<VisualTreeAsset>("Scenes/LevelSelectionMenu");
        pause_menu = Resources.Load<VisualTreeAsset>("Scenes/PauseMenu");
        level = Resources.Load<VisualTreeAsset>("Scenes/Level");

        root = GetComponent<UIDocument>().rootVisualElement;

        change_scene(State.MainMenu);
    }

    private void change_scene(State state, Level level_resource = null)
    {
        switch (state)
        {
            case State.MainMenu:
                root.Clear();
                current_scene = main_menu.Instantiate();
                break;
            case State.LevelSelectionMenu:
                root.Clear();
                current_scene = level_selection_menu.Instantiate();
                break;
            case State.PauseMenu:
                current_scene = pause_menu.Instantiate();
                break;
            case State.Level:
                root.Clear();
                current_scene = level.Instantiate();
                break;
            default:
                current_scene = null; 
                break;
        }
        Debug.Assert(current_scene != null);
        root.Add(current_scene);

        set_scene(state, level_resource);
    }

    private void set_scene(State state, Level level_resource)
    {
        switch (state)
        {
            case State.MainMenu:
                set_main_menu(current_scene);
                break;
            case State.LevelSelectionMenu:
                set_level_selection_menu(current_scene);
                break;
            case State.PauseMenu:
                set_pause_menu(current_scene);
                break;
            case State.Level:
                set_level(current_scene, level_resource);
                break;
        }
    }

    private void set_main_menu(VisualElement root)
    {
        VisualElement start_button = root.Q("Start");

        start_button.RegisterCallback<MouseDownEvent>((MouseDownEvent evt) =>
        {
            change_scene(State.LevelSelectionMenu);
        });
    }
    private void set_level_selection_menu(VisualElement root)
    {
        VisualElement return_button = root.Q("Arrow");

        return_button.RegisterCallback<MouseDownEvent>((MouseDownEvent evt) =>
        {
            change_scene(State.MainMenu);
        });

        List<VisualElement> level_buttons = root.Q("Levels").Children().ToList();
        for (int i = 0; i < level_buttons.Count(); i++)
        {
            
            if (level_resources.Count() > i)
            {
                VisualElement level_button = level_buttons[i];
                Level level_resource = level_resources[i];
                // Callback para entrar a un nivel
                level_button.RegisterCallback<MouseDownEvent>((MouseDownEvent evt) =>
                {
                    change_scene(State.Level, level_resource);
                });
                // Callback para mostrar el tooltip del nivel
                level_button.RegisterCallback<MouseEnterEvent>((MouseEnterEvent evt) =>
                {
                    set_level_selection_tooltip(level_resource);
                });
                // Mostrar icono nivel solo si este ha sido completado
                //if (level_resource.is_completed())
                (level_button.Children().First() as Image).sprite = level_resource.get_sprite();
                
            }
        }
    }
    private void set_level_selection_tooltip(Level level_resource)
    {

    }
    private void set_pause_menu(VisualElement root)
    {

    }
    private void set_level(VisualElement root, Level level_resource)
    {
        Debug.Assert(level_resource != null);
    }
}
