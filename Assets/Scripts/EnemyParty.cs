using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyParty : MonoBehaviour
{
    public List<Enemy> partyMembers = new List<Enemy>();
    private List<Vector2Int> aggroSquares = new List<Vector2Int>();

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
        }
    }

    void Start()
    {
        foreach (Enemy enemy in partyMembers)
        {
            enemy.Initialize();
        }
        GenerateAggroSquares();
    }

    private void GenerateAggroSquares()
    {
        List<Vector2Int> aggro = new List<Vector2Int>();
        foreach (Enemy enemy in partyMembers)
        {
            aggro.AddRange(enemy.GetAggroSquares());
        }
        aggroSquares = aggro;
    }
    public List<Vector2Int> GetAggroSquares()
    {
        if (IsAlive())
        {
            return aggroSquares;
        }
        return new List<Vector2Int>();
    }


    public void ResetActionPoints()
    {

        foreach (var partyMember in partyMembers)
        {
            if (partyMember.alive)
            {
                partyMember.ResetActionPoints();
            }
        }
    }
    public void ReplenishActionPoints()
    {
        foreach (var partyMember in partyMembers)
        {
            if (partyMember.alive)
            {
                partyMember.ReplenishActionPoints();
            }
        }
    }

    public int NumAliveMembers()
    {
        int retVal = 0;
        foreach (var partyMember in partyMembers)
        {
            if (partyMember.alive) retVal++;
        }
        return retVal;
    }

    public bool IsAlive()
    {
        return NumAliveMembers() > 0;
    }
}
