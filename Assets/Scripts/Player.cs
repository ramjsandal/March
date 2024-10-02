using System.Collections.Generic;
using System.ComponentModel;
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
                Camera.main.transform.position = transform.position + new Vector3(0,0,-5);
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
            _gridPos = value;
            if (_gridPos != null)
            {
                transform.position = _gridManager.GetTileCenter(gridPos.Value);
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

    public void Update()
    {
        if (selected)
        {
            List<Vector2Int> travTiles = _gridManager.IndicateTraversible(gridPos.Value, 4);
            _gridManager.TintTiles(travTiles, Color.red);
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Teleport(travTiles, _gridManager.MouseToGrid());
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
