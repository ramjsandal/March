using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    private static BattleManager _instance;
    public static BattleManager Instance
    {
        get { return _instance; }
    }

    private Party playerParty;
    private List<EnemyParty> enemyPartyList;
    private bool battling = false;
    private bool playerTurn = true;
    private int enemyBattling;

    public Canvas battleCanvas;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            battling = false;
            battleCanvas.enabled = false;
            playerTurn = true;
            playerParty = GameObject.FindObjectOfType<Party>();
            enemyPartyList = new List<EnemyParty>();
            enemyPartyList.AddRange(GameObject.FindObjectsOfType<EnemyParty>());
            foreach (Player p in playerParty.partyMembers)
            {
                p.StoppedMoving += CheckBattleStart;
            }
        }
    }

    private void CheckBattleStart(object sender, EventArgs e)
    {
        if (battling)
        {
            return;
        }
        int fightingGroup = PlayerInAggro();

        if (fightingGroup == -1) { return; }

        enemyBattling = fightingGroup;
        battling = true;
        playerParty.battling = true;
        enemyPartyList[fightingGroup].battling = true;
        battleCanvas.enabled = true;
        Debug.Log("STARTED A BATTLE");
    }

    private int PlayerInAggro()
    {
        List<Vector2Int> playerSquares = playerParty.GetPartyPositions();

        for (int i = 0; i < enemyPartyList.Count; i++)
        {
            if (enemyPartyList[i].aggroSquares.Intersect(playerSquares).Any())
            {
                return i;
            }
        }

        return -1;
    }

    public void EndTurn()
    {
        if (battling)
        {
            if (playerTurn)
            {
                playerTurn = false;
                playerParty.ResetActionPoints();
                enemyPartyList[enemyBattling].ReplenishActionPoints();
                var enemyParty = enemyPartyList[enemyBattling].partyMembers;

                // do enemy turn
                int numAliveEnemies = 0;
                foreach (Enemy e in enemyParty)
                {
                    while (e.actionPoints > 0)
                    {
                        if (e.alive)
                        {
                            e.MakeMove(playerParty.partyMembers.Where(a => a.alive).ToList());
                        }
                    }

                    if (e.alive)
                    {
                        numAliveEnemies++;
                    }
                }
                if (numAliveEnemies <= 0)
                {
                    battling = false;
                }
            }
            else
            {
                playerTurn = true;
                playerParty.ReplenishActionPoints();
                enemyPartyList[enemyBattling].ResetActionPoints();
            }
        }
    }

    private void Battle()
    {

    }


}
