using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : Agent
{
    private int aggroRange = 3;
    private List<Vector2Int> aggroSquares;
    private int health = 10;

    public void Initialize()
    {
        _gridManager = GridManager.Instance;
        gridPos = _gridManager.GetCellPosition(transform.position);
        GetAggroSquares();
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            gridPos = null;
            gameObject.SetActive(false);
        }
    }
    public List<Vector2Int> GetAggroSquares()
    {
        aggroSquares = _gridManager.IndicateVisible(gridPos.Value, aggroRange).Select(a => a.position).ToList();
        return aggroSquares; ;
    }

}
