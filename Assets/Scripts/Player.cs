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
    private void Start()
    {
        _gridManager = GridManager.Instance;
        gridPos = _gridManager.GetCellPosition(transform.position);
    }
    public void Update()
    {
        if (selected)
        {
            List<NodeInfo> travTiles = _gridManager.IndicateTraversible(gridPos.Value, 4);
            _paths = travTiles;
            List<Vector2Int> coords = travTiles.Select(a => a.position).ToList();
            _gridManager.TintTiles(coords, Color.red);
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                //Teleport(coords, _gridManager.MouseToGrid());
                StartCoroutine(MoveAlongPath(PathToSquare(_gridManager.MouseToGrid())));
            }
        }
    }

}
