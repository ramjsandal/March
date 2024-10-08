using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : Agent
{
    private int aggroRange = 5;
    private List<Vector2Int> aggroSquares;
    private void Start()
    {
        _gridManager = GridManager.Instance;
        gridPos = _gridManager.GetCellPosition(transform.position);
    }
    /*
    public List<Vector2Int> GetAggroSquares()
    {
        return _gridManager.IndicateTraversible(gridPos.Value, aggroRange).ToList();
    }
    */

}
