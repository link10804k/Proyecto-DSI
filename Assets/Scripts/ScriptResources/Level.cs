using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/Level")]
public class Level : ScriptableObject
{
    [SerializeField]
    private string _puzzle_id;
    [SerializeField]
    private string _puzzle_title;

    [SerializeField]
    private Sprite _puzzle_sprite;
    [SerializeField]
    private Sprite _solution_sprite;

    Vector2Int _puzzle_size;

    int _puzzle_time = 0;

    bool _is_completed = false;

    List<List<bool>> _solution_matrix;

    private void OnEnable()
    {
        _puzzle_size = new Vector2Int(_puzzle_sprite.texture.width, _puzzle_sprite.texture.height);
        
        _solution_matrix = new List<List<bool>>();
        for (int y = 0; y < _puzzle_size.y; y++)
        {
            List<bool> _solution_row = new List<bool>();
            for (int x = _puzzle_size.x - 1; x >= 0; x--)
            {
                if (x == 0) _solution_matrix.Add(_solution_row);

                Color pixel_color = _solution_sprite.texture.GetPixel(x, y);
                if (pixel_color.r < 0.3f && pixel_color.g < 0.3f && pixel_color.b < 0.3f) _solution_row.Add(true);
                else _solution_row.Add(false);
            }
        }
    }

    public string get_id()
    {
        return _puzzle_id;
    }
    public string get_title()
    {
        return _puzzle_title;
    }
    public Sprite get_sprite()
    {
        return _puzzle_sprite;
    }
    public Vector2Int get_size()
    {
        return _puzzle_size;
    }
    public int get_time()
    {
        return _puzzle_time;
    }

    public bool is_completed()
    {
        return _is_completed;
    }
    public List<List<bool>> get_solution_matrix()
    {
        return _solution_matrix;
    }
}
