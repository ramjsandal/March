using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GridManager;

public class Player : Agent
{
    public int moveRange = 4;
    public int meleeRange = 1;
    public int meleeDamage = 10;
    private List<Vector2Int> _meleeTiles = new List<Vector2Int>();
    private void Start()
    {
        _gridManager = GridManager.Instance;
        gridPos = _gridManager.GetCellPosition(transform.position);
        _meleeTiles = new List<Vector2Int>();
        moving = false;
    }
    public void Update()
    {
        if (moving)
        {
            return;
        }

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
                _meleeTiles = coords;
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
            if (enemy != null && _meleeTiles.Contains(enemy.gridPos.Value))
            {
                enemy.TakeDamage(meleeDamage);
            }
        }
    }

}
