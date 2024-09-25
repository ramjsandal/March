using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static GridManager;

public class Player : MonoBehaviour
{
    public enum PlayerMode
    {
        MOVE,
        ATTACK
    };

    private Transform _transform;

    private GridManager _gridManager;

    public PlayerMode mode = PlayerMode.MOVE;

    private Vector2Int _gridPos;
    public Vector2Int gridPos
    {
        get => _gridPos;
        private set
        {
            _gridPos = value;
            _transform.position = _gridManager.GetTileCenter(gridPos);
        }
    }

    public void Start()
    {
        _transform = GetComponent<Transform>();
        _gridManager = GridManager.Instance;
    }

    public void SwapMode()
    {
        mode++;
        if (mode > PlayerMode.ATTACK)
        {
            mode = PlayerMode.MOVE;
        }
    }

    public bool Move(Direction dir)
    {
        Vector2Int updatedPos = gridPos;
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
        if (Input.GetKeyDown(KeyCode.W)) {
            Move(Direction.UP);
        } else if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Direction.LEFT);
        } else if (Input.GetKeyDown(KeyCode.S))
        {
            Move(Direction.DOWN);
        } else if (Input.GetKeyDown (KeyCode.D))
        {
            Move(Direction.RIGHT);
        }

    }
}
