﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GridManager;

public class Player : Agent
{
    public int attackRange = 1;
    public int attackDamage = 10;
    private List<Vector2Int> _attackableTiles = new List<Vector2Int>();
    //private Color tint = new Color(255f / 255f, 127f / 255f, 127f / 255f);
    public static Color tint = new Color(219f / 255f, 65f / 255f, 97f / 255f);
    protected void Start()
    {
        actionPoints = 2;
        _gridManager = GridManager.Instance;
        gridPos = _gridManager.GetCellPosition(transform.position);
        _attackableTiles = new List<Vector2Int>();
        moving = false;
    }
    public void BattleActions()
    {
        // if we have no action points, we cant do anything
        if (actionPoints <= 0)
        {
            _gridManager.NoTint();
            return;
        }
        else if (_selectedAction == SelectedAction.MOVING)
        {
            MoveAction(true);
        }
        else if (_selectedAction == SelectedAction.ATTACKING)
        {
            AttackAction();
        }
    }

    public void AttackAction()
    {
        List<NodeInfo> attackableTiles = IndicateAttackable();
        List<Vector2Int> coords = attackableTiles.Select(a => a.position).ToList();
        _attackableTiles = coords;
        _gridManager.TintTiles(coords, tint);
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (coords.Contains(_gridManager.MouseToGrid()) && EnemyAtMouse())
            {
                Attack();
                actionPoints--;
            }
        }

    }

    public void MoveAction(bool useActionPoint)
    {
        List<NodeInfo> travTiles = _gridManager.IndicateMovable(gridPos.Value, moveRange);
        _paths = travTiles;
        List<Vector2Int> coords = travTiles.Select(a => a.position).ToList();
        _gridManager.TintTiles(coords, tint);
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {

            // IF THIS IS NOT A VALID POSITION, DONT MOVE
            var posn = _gridManager.MouseToGrid();
            if (!coords.Contains(posn))
            {
                return;
            }

            StartCoroutine(MoveAlongPath(PathToSquare(posn)));
            if (useActionPoint)
            {
                actionPoints--;
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
                if (!BattleManager.Instance.EnemiesAlive())
                {
                    BattleManager.Instance.EndTurnWrapper();
                }
            }
        }
    }

    private bool EnemyAtMouse()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                return true;
            }
        }

        return false;
    }

}
