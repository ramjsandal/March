using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using static GridManager;

public class Player : Agent
{
    public int moveRange = 4;
    public int meleeRange = 1;
    public int meleeDamage = 10;
    private void Start()
    {
        _gridManager = GridManager.Instance;
        gridPos = _gridManager.GetCellPosition(transform.position);
    }
    public void Update()
    {
        if (selected)
        {
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
            else if (_selectedAction == SelectedAction.ATTACKING)
            {
                List<NodeInfo> meleeTiles = _gridManager.IndicateMeleeable(gridPos.Value, meleeRange);
                List<Vector2Int> coords = meleeTiles.Select(a => a.position).ToList();
                _gridManager.TintTiles(coords, Color.red);
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    MeleeAttack();
                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                ChangeSelectedAction();
            }
        }
    }

    public void MeleeAttack()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(meleeDamage); 
            }
        }
    }

}
