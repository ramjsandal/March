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
        protected set
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

    private bool _moving = false;
    public bool moving
    {
        get
        {
            return _moving;
        }

        protected set
        {
            _moving = value;
            Camera.main.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, Camera.main.transform.position.z);
            if (_moving == false)
            {
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

        protected set
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

    public Sprite portrait;

    public int health = 10;

    public int moveRange = 4;

    public bool alive = true;

    public event EventHandler AgentDied;
    public void OnAgentDied(EventArgs e)
    {
        if (AgentDied != null)
        {
            AgentDied(this, e);
        }
    }

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
        DamageAnimWrapper();
        if (health <= 0)
        {
            gridPos = null;
            Debug.Log("Died");
            alive = false;
            animating = false;
            gameObject.SetActive(false);
        }
    }

    public bool animating = false;
    IEnumerator DamageAnim()
    {
        animating = true;
        var spr = GetComponent<SpriteRenderer>();
        spr.color = Color.red;
        yield return new WaitForSeconds(.1f);
        Debug.Log("MADE IT HERE");
        spr.color = Color.white;
        animating = false;
    }

    public void DamageAnimWrapper()
    {
        StartCoroutine(DamageAnim());
    }


    public void Collapse()
    {
        gridPos = null;
        gameObject.SetActive(false);
    }

}
