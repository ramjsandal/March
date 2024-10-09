using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GridManager;

public class Agent : MonoBehaviour
{
    protected GridManager _gridManager;

    protected List<NodeInfo> _paths;

    private Vector2Int? _gridPos;
    public Vector2Int? gridPos
    {
        get => _gridPos;
        set
        {
            Vector2Int? oldPos = _gridPos;

            if (oldPos != null)
            {
                _gridManager.SetOccupied(oldPos.Value, false);
            }

            _gridPos = value;
            if (_gridPos != null)
            {
                transform.position = _gridManager.GetTileCenter(gridPos.Value);
                _gridManager.SetOccupied(gridPos.Value, true);

            }
        }
    }

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

    protected enum SelectedAction
    {
        MOVING,
        ATTACKING
    };

    protected SelectedAction _selectedAction;


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

    public bool MouseOnAgent()
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

        if (path.Count == 0)
        {
            return path;
        }

        path.RemoveAt(path.Count - 1);
        path.Reverse();

        return path;
    }

    protected IEnumerator MoveAlongPath(List<Vector2Int> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            Teleport(path[i]);
            yield return new WaitForSeconds(.25f);
        }
    }

    protected void ChangeSelectedAction()
    {
        _selectedAction++;
        if (_selectedAction > SelectedAction.ATTACKING)
        {
            _selectedAction = SelectedAction.MOVING;
        }
    }




}
