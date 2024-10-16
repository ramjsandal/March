using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rider : Player
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        moveRange = 6;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }
    protected override void Attack()
    {
        base.Attack();
    }

    protected override List<GridManager.NodeInfo> IndicateAttackable()
    {
        return base.IndicateAttackable();
    }
}
