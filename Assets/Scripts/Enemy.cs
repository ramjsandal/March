using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GridManager;

public class Enemy : Agent
{
    private int aggroRange = 3;
    private List<Vector2Int> aggroSquares;

    public void Initialize()
    {
        _gridManager = GridManager.Instance;
        gridPos = _gridManager.GetCellPosition(transform.position);
        GetAggroSquares();
    }

    public List<Vector2Int> GetAggroSquares()
    {
        aggroSquares = _gridManager.IndicateVisible(gridPos.Value, aggroRange).Select(a => a.position).ToList();
        return aggroSquares; ;
    }

    public void MakeMove(List<Player> players)
    {
        if (moving)
        {
            return;
        }

        // hit closest player
        // check 4 surrounding squares
        List<NodeInfo> meleeTiles = _gridManager.IndicateMeleeable(gridPos.Value, 1);
        List<Vector2Int> coords = meleeTiles.Select(a => a.position).ToList();
        List<Vector2Int> playersInMeleeRange = coords.Intersect(players.Select(a => a.gridPos.Value)).ToList();
        if (playersInMeleeRange.Count > 0)
        {
            // hit the first one
            Vector2Int victimPos = playersInMeleeRange[0];
            Player victim = players.First(a => a.gridPos == victimPos);
            victim.TakeDamage(5);
            actionPoints--;
            return;
        }

        // move to closest player
        _paths = _gridManager.IndicateMovable(gridPos.Value, moveRange);
        List<Vector2Int> sqauresInRange = _paths.Select(a => a.position).ToList();
        List<(Vector2Int, int)> closestSquares = new List<(Vector2Int, int)>();
        foreach (Player player in players)
        {
            (Vector2Int, int) posAndRange = _gridManager.FindClosestSquare(sqauresInRange, player.gridPos.Value);
            closestSquares.Add(posAndRange);
        }
        closestSquares.Sort((a, b) => a.Item2.CompareTo(b.Item2));
        StartCoroutine(MoveAlongPath(PathToSquare(closestSquares[0].Item1)));
        //Teleport(closestSquares[0].Item1);
        actionPoints--;
        return;
    }

}
