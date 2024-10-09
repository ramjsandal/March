using System.Collections.Generic;
using UnityEngine;

public class EnemyParty : MonoBehaviour
{
    public List<Enemy> partyMembers = new List<Enemy>();
    public List<Vector2Int> aggroSquares = new List<Vector2Int>();

    void Start()
    {
        foreach (Enemy enemy in partyMembers)
        {
            enemy.Initialize();
        }

        GetAggroSquares();
    }

    void Update()
    {

    }

    public void GetAggroSquares()
    {
        List<Vector2Int> aggro = new List<Vector2Int>();
        foreach (Enemy enemy in partyMembers)
        {
            aggro.AddRange(enemy.GetAggroSquares());
        }
        aggroSquares = aggro;
    }
}
