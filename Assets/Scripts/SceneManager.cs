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

    private void change_scene(State state)
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

        set_scene(state);
    }

    private void set_scene(State state)
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
                set_level(current_scene);
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
                Level level = level_resources[i];
                // Callback para entrar a un nivel
                level_button.RegisterCallback<MouseDownEvent>((MouseDownEvent evt) =>
                {
                    level_resource = level;
                    change_scene(State.Level);
                });
                // Callback para mostrar el tooltip del nivel
                level_button.RegisterCallback<MouseEnterEvent>((MouseEnterEvent evt) =>
                {
                    level_resource = level;
                    set_level_selection_tooltip(root);  
                });
                // Mostrar icono nivel solo si este ha sido completado
                //if (level_resource.is_completed())
                (level_button.Children().First() as Image).sprite = level.get_sprite();
                
            }
        }
    }
    private void set_level_selection_tooltip(VisualElement root)
    {
        //if (level_resource.is_completed())
        VisualElement tooltip_image = root.Q("TooltipImage");
        tooltip_image.style.backgroundImage = Background.FromSprite(level_resource.get_sprite());

        Label tooltip_id = root.Q<Label>("ID");
        tooltip_id.text = level_resource.get_id();

        //if (level_resource.is_completed())
        Label tooltip_name = root.Q<Label>("NAME");
        tooltip_name.text = level_resource.get_title();

        Label tooltip_size = root.Q<Label>("SIZE");
        Vector2Int size = level_resource.get_size();
        tooltip_size.text = size.x.ToString() + "x" + size.y.ToString();

        Label tooltip_time = root.Q<Label>("TIMER");
        int seconds = level_resource.get_time();
        int minutes = seconds / 60;
        seconds %= 60;
        int hours = minutes / 60;
        minutes %= 60;
        string string_seconds = (seconds < 10 ? "0" : "") + seconds.ToString();
        string string_minutes = (minutes < 10 ? "0" : "") + minutes.ToString();
        string string_hours = (hours < 10 ? "0" : "") + hours.ToString();

        tooltip_time.text = string_hours + ":" + string_minutes + ":" + string_seconds;
    }
    private void set_pause_menu(VisualElement root)
    {

    }

    enum CellState
    {
        Blank,
        Filled,
        Marked
    }

    List<List<CellState>> cell_state_matrix;
    List<List<VisualElement>> cell_matrix;
    Level level_resource;

    private void set_level(VisualElement root)
    {
        cell_matrix = new List<List<VisualElement>>();
        //cell_matrix.ForEach((cell_row) => cell_row = new List<VisualElement>(level_resource.get_size().x));
        //cell_state_matrix = new List<List<CellState>>();

        VisualTreeAsset cell_template = Resources.Load<VisualTreeAsset>("Templates/PuzzleCell");
        VisualElement board = root.Q("Tablero");
        // Crear matriz de casillas
        for (int y = 0; y < level_resource.get_size().y; y++)
        {
            List<VisualElement> cell_row = new List<VisualElement>();
            cell_matrix.Add(cell_row);
            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            board.Add(row);
            for (int x = 0; x < level_resource.get_size().x; x++)
            {
                // Instanciamos una casilla
                VisualElement cell = cell_template.Instantiate();
                cell_row.Add(cell);
                row.Add(cell);

                cell.RegisterCallback<MouseDownEvent>((MouseDownEvent evt) =>
                {
                    if (evt.button == 0) // Clic izquierdo
                    {
                        fill_cell(x, y);
                    }
                    else if (evt.button == 1) // Clic derecho
                    {
                        mark_cell(x, y);
                    }
                });
            }
        }
    }
    private void fill_cell(int x, int y)
    {
        if (cell_state_matrix[x][y] == CellState.Blank)
        {
            cell_state_matrix[x][y] = CellState.Filled;
            // Cambiar sprite
            foreach (VisualElement state_sprite in cell_matrix[x][y].Children())
            {
                if (state_sprite.name == "Filled") state_sprite.style.display = DisplayStyle.Flex;
                else state_sprite.style.display = DisplayStyle.Flex;
            }
        }
        else
        {
            cell_state_matrix[x][y] = CellState.Blank;
            // Cambiar sprite
            foreach (VisualElement state_sprite in cell_matrix[x][y].Children())
            {
                if (state_sprite.name == "Blank") state_sprite.style.display = DisplayStyle.Flex;
                else state_sprite.style.display = DisplayStyle.Flex;
            }
        }
        check_solution();
    }
    private void mark_cell(int x, int y)
    {
        if (cell_state_matrix[x][y] == CellState.Blank)
        {
            cell_state_matrix[x][y] = CellState.Marked;
            // Cambiar sprite
            foreach (VisualElement state_sprite in cell_matrix[x][y].Children())
            {
                if (state_sprite.name == "Marked") state_sprite.style.display = DisplayStyle.Flex;
                else state_sprite.style.display = DisplayStyle.Flex;
            }
        }
        else
        {
            cell_state_matrix[x][y] = CellState.Blank;
            // Cambiar sprite
            foreach (VisualElement state_sprite in cell_matrix[x][y].Children())
            {
                if (state_sprite.name == "Blank") state_sprite.style.display = DisplayStyle.Flex;
                else state_sprite.style.display = DisplayStyle.Flex;
            }
        }
        check_solution();
    }
    private bool check_solution() 
    {
        List<List<bool>> solution_matrix = level_resource.get_solution_matrix();
        for (int y = 0; y < solution_matrix.Count(); y++)
        {
            for (int x = 0; x < solution_matrix[0].Count(); x++)
            {
                if ((cell_state_matrix[x][y] == CellState.Filled && !solution_matrix[x][y]) ||
                    (cell_state_matrix[x][y] == CellState.Blank && solution_matrix[x][y]) ||
                    (cell_state_matrix[x][y] == CellState.Marked && solution_matrix[x][y])) 
                    return false;
            }
        }
        return true;
    }
}
