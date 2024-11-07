using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Archer : Player
{
    
    void Start()
    {
        base.Start();
        this.attackRange = 3;
        this.attackDamage = 4;
    }
    protected override void Attack()
    {
        base.Attack();
    }

    protected override List<GridManager.NodeInfo> IndicateAttackable()
    {
        return _gridManager.IndicateVisible(this.gridPos.Value, attackRange);
    }

}
