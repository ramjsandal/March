using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GridManager _gridManager;
    public int health = 100;
    private Vector2Int _gridPos;
    public Vector2Int gridPos
    {
        get => _gridPos;
        private set
        {
            _gridPos = value;
            transform.position = _gridManager.GetTileCenter(gridPos);
        }
    }



    // Start is called before the first frame update
    private void Awake()
    {
        _gridManager = GridManager.Instance;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage()
    {
        health -= 10;
    }

    public bool MouseOnEnemy()
    {
        if (_gridManager.MouseToGrid() == gridPos)
        {
            return true;
        }
        return false;

    }
    
}
