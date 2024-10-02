using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using static GridManager;

public class Player : MonoBehaviour
{
    public enum PlayerMode
    {
        MOVE,
        ATTACK
    };

    private GridManager _gridManager;

    private List<NodeInfo> _paths;

    private bool _selected = false;
    public bool selected
    {
        get => _selected;
        set
        {
            _selected = value;
            if (selected)
            {
                Camera.main.transform.parent = transform;
                Camera.main.transform.position = transform.position + new Vector3(0, 0, -5);
            }
        }
    }

    public PlayerMode mode = PlayerMode.MOVE;

    private Vector2Int? _gridPos;
    public Vector2Int? gridPos
    {
        get => _gridPos;
        set
        {
            Vector2Int? oldPos = _gridPos;

            if (oldPos != null)
            {
                _gridManager.SetTraversability(oldPos.Value, true);
            }

            _gridPos = value;
            if (_gridPos != null)
            {
                transform.position = _gridManager.GetTileCenter(gridPos.Value);
                _gridManager.SetTraversability(gridPos.Value, false);

            }
        }
    }

    public void Start()
    {
        _gridManager = GridManager.Instance;
        gridPos = _gridManager.GetCellPosition(transform.position);
    }

    public void SwapMode()
    {
        mode++;
        if (mode > PlayerMode.ATTACK)
        {
            mode = PlayerMode.MOVE;
        }
    }

    public bool Teleport(Vector2Int pos)
    {
        if (_gridManager.map.ContainsKey(pos))
        {
            TileInfo updatedLocation = _gridManager.map[pos];
            if (updatedLocation.traversable)
            {
                gridPos = pos;
                return true;
            }
        }

        return false;
    }

    public bool Teleport(List<Vector2Int> viablePos, Vector2Int attemptedPos)
    {

        if (viablePos.Contains(attemptedPos))
        {
            return Teleport(attemptedPos);
        }
        return false;
    }

    public bool Move(Direction dir)
    {
        Vector2Int updatedPos = gridPos.Value;
        switch (dir)
        {
            case Direction.UP:
                updatedPos.y += 1;
                break;
            case Direction.DOWN:
                updatedPos.y -= 1;
                break;
            case Direction.LEFT:
                updatedPos.x -= 1;
                break;
            case Direction.RIGHT:
                updatedPos.x += 1;
                break;
            default:
                throw new InvalidEnumArgumentException();
        }

        if (_gridManager.map.ContainsKey(updatedPos))
        {
            TileInfo updatedLocation = _gridManager.map[updatedPos];
            if (updatedLocation.traversable)
            {
                gridPos = updatedPos;
                return true;
            }
        }

        return false;
    }

    public List<Vector2Int> PathToSquare(Vector2Int pos)
    {
        // find the node info that has this position
        NodeInfo current = _paths.Find(a => a.position == pos);
        List<Vector2Int> path = new List<Vector2Int>();

        while (current != null)
        {
            path.Add(current.position); 
            current = current.parent;
        }

        path.RemoveAt(path.Count - 1);
        path.Reverse();

        return path;
    }

    IEnumerator MoveAlongPath(List<Vector2Int> path)
    {
        for (int i = 0; i < path.Count; i++) { 
            Teleport(path[i]);
            yield return new WaitForSeconds(.1f);
        }
    }

    public void Update()
    {
        if (selected)
        {
            List<NodeInfo> travTiles = _gridManager.IndicateTraversible(gridPos.Value, 4);
            _paths = travTiles;
            List<Vector2Int> coords = travTiles.Select(a => a.position).ToList(); 
            _gridManager.TintTiles(coords, Color.red);
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                //Teleport(coords, _gridManager.MouseToGrid());
                StartCoroutine(MoveAlongPath(PathToSquare(_gridManager.MouseToGrid())));
            }
        }

    }

    public bool MouseOnPlayer()
    {
        if (_gridManager.MouseToGrid() == gridPos)
        {
            return true;
        }
        return false;

    }

    public List<Vector2Int> GetClosestSquares(int numSquares)
    {
        return _gridManager.FindClosestTraversible(gridPos.Value, numSquares);
    }

}
