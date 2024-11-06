using System;
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
                Camera.main.transform.position = transform.position + new Vector3(0, 0, -5);
                OnAgentSelected(null);
            }
        }
    }

    public event EventHandler AgentSelected;
    public void OnAgentSelected(EventArgs e)
    {
        if (AgentSelected != null)
        {
            AgentSelected(this, e);
        }
    }

    private bool _moving = false;
    public bool moving
    {
        get
        {
            return _moving;
        }

        set
        {
            _moving = value;
            if (_moving == false)
            {
                Camera.main.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, Camera.main.transform.position.z);
                OnStoppedMoving(null);
            }

        }
    }

    private int _actionPoints;
    public int actionPoints
    {
        get
        {
            return _actionPoints;
        }

        set
        {
            _actionPoints = value;
            OnUsedActionPoint(null);
        }
    }

    public event EventHandler UsedActionPoint;
    public void OnUsedActionPoint(EventArgs e)
    {
        if (UsedActionPoint != null)
        {
            UsedActionPoint(this, e);
        }
    }

    private bool _battling;
    public bool battling
    {
        get
        {
            return _battling;
        }

        set
        {
            _battling = value;

            if (_battling == true)
            {
                actionPoints = 2;
            }
        }
    }

    public Sprite portrait;

    public int health = 10;

    public int moveRange = 4;

    public bool alive = true;

    public event EventHandler StoppedMoving;

    public void OnStoppedMoving(EventArgs e)
    {
        if (StoppedMoving != null)
        {
            StoppedMoving(this, e);
        }
    }

    public enum SelectedAction
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
        moving = true;
        for (int i = 0; i < path.Count; i++)
        {
            Teleport(path[i]);
            yield return new WaitForSeconds(.25f);
        }
        moving = false;
    }

    protected void ChangeSelectedAction()
    {
        _selectedAction++;
        if (_selectedAction > SelectedAction.ATTACKING)
        {
            _selectedAction = SelectedAction.MOVING;
        }
    }

    public void SetSelectedAction(SelectedAction action)
    {
        _selectedAction = action;
    }

    public SelectedAction GetSelectedAction()
    {
        return _selectedAction;
    }

    public void ReplenishActionPoints()
    {
        actionPoints = 2;
    }
    public void ResetActionPoints()
    {
        actionPoints = 0;
    }


    public event EventHandler TookDamage;

    public void OnTookDamage(EventArgs e)
    {
        if (TookDamage != null)
        {
            TookDamage(this, e);
        }
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        OnTookDamage(null);
        if (health <= 0)
        {
            gridPos = null;
            Debug.Log("Died");
            alive = false;
            gameObject.SetActive(false);
        }
    }



}
