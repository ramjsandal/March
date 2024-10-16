using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GridManager;

public class Player : Agent
{
    public int attackRange = 1;
    public int attackDamage = 10;
    private List<Vector2Int> _attackableTiles = new List<Vector2Int>();
    protected void Start()
    {
        actionPoints = 2;
        _gridManager = GridManager.Instance;
        gridPos = _gridManager.GetCellPosition(transform.position);
        _attackableTiles = new List<Vector2Int>();
        moving = false;
    }
    public void Update()
    {
        if (moving)
        {
            return;
        }

        // NOT IN BATTLE
        if (selected && !battling)
        {
            _selectedAction = SelectedAction.MOVING;
            if (_selectedAction == SelectedAction.MOVING)
            {
                List<NodeInfo> travTiles = _gridManager.IndicateMovable(gridPos.Value, moveRange);
                _paths = travTiles;
                List<Vector2Int> coords = travTiles.Select(a => a.position).ToList();
                _gridManager.TintTiles(coords, Color.red);
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    StartCoroutine(MoveAlongPath(PathToSquare(_gridManager.MouseToGrid())));
                }
            }
            return;
        }

        // IN BATTLE
        if (selected && battling)
        {
            // if we have no action points, we cant do anything
            if (actionPoints <= 0)
            {
                _gridManager.NoTint();
                return;
            }

            if (_selectedAction == SelectedAction.MOVING)
            {
                List<NodeInfo> travTiles = _gridManager.IndicateMovable(gridPos.Value, moveRange);
                _paths = travTiles;
                List<Vector2Int> coords = travTiles.Select(a => a.position).ToList();
                _gridManager.TintTiles(coords, Color.red);
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    if (coords.Contains(_gridManager.MouseToGrid()))
                    {
                        StartCoroutine(MoveAlongPath(PathToSquare(_gridManager.MouseToGrid())));
                        actionPoints--;
                    }
                }
            }
            else if (_selectedAction == SelectedAction.ATTACKING)
            {
                List<NodeInfo> attackableTiles = IndicateAttackable();
                List<Vector2Int> coords = attackableTiles.Select(a => a.position).ToList();
                _attackableTiles = coords;
                _gridManager.TintTiles(coords, Color.red);
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    if (coords.Contains(_gridManager.MouseToGrid()))
                    {
                        Attack();
                        actionPoints--;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                ChangeSelectedAction();
            }
        }
    }

    protected virtual List<NodeInfo> IndicateAttackable()
    {
        return _gridManager.IndicateMeleeable(gridPos.Value, attackRange);
    }

    protected virtual void Attack()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();
            if (enemy != null && _attackableTiles.Contains(enemy.gridPos.Value))
            {
                enemy.TakeDamage(attackDamage);
            }
        }
    }

}
