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
    public UIPortrait playerPortraitTemplate;
    private List<UIPortrait> playerPortraitList;

    public void GeneratePlayerPortraits()
    {
        playerPortraitList = new List<UIPortrait>();
        var defaultPos = playerPortraitTemplate.gameObject.transform.position;
        var width = playerPortraitTemplate.GetComponent<RectTransform>().sizeDelta.x;
        for (int i = 0; i < playerParty.partyMembers.Count; i++)
        {
            var currentPortrait = Instantiate(playerPortraitTemplate, new Vector3(defaultPos.x + (i * width), defaultPos.y, defaultPos.z), Quaternion.identity);
            currentPortrait.transform.SetParent(battleCanvas.transform);
            var portScript = currentPortrait.GetComponent<UIPortrait>();
            portScript.text.text = playerParty.partyMembers[i].health.ToString();
            portScript.image.sprite = playerParty.partyMembers[i].portrait;
            currentPortrait.enabled = true;
            playerPortraitList.Add(currentPortrait);
        }
    }

    private void UpdatePortraits(object sender, EventArgs e)
    {
        for (int i = 0; i < playerPortraitList.Count; i++)
        {
            playerPortraitList[i].text.text = playerParty.partyMembers[i].health.ToString();
        }
    }
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
                p.TookDamage += UpdatePortraits;
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
        GeneratePlayerPortraits();
        battleCanvas.enabled = true;
        Debug.Log("STARTED A BATTLE");
    }

    private int PlayerInAggro()
    {
        List<Vector2Int> playerSquares = playerParty.GetPartyPositions();

        for (int i = 0; i < enemyPartyList.Count; i++)
        {
            if (enemyPartyList[i].GetAggroSquares().Intersect(playerSquares).Any())
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

            var enemyParty = enemyPartyList[enemyBattling];
            var enemyPartyMembers = enemyParty.partyMembers;
            if (playerTurn)
            {
                playerTurn = false;
                GridManager.Instance.NoTint();
                playerParty.ResetActionPoints();
                enemyPartyList[enemyBattling].ReplenishActionPoints();

                // do enemy turn
                foreach (Enemy e in enemyPartyMembers)
                {
                    while (e.alive && e.actionPoints > 0)
                    {

                        e.MakeMove(playerParty.partyMembers.Where(a => a.alive).ToList());
                    }
                }

            }
            else
            {
                playerTurn = true;
                playerParty.ReplenishActionPoints();
                enemyPartyList[enemyBattling].ResetActionPoints();
            }

            // CHECK IF BATTLE OVER
            if (enemyParty.NumAliveMembers() <= 0)
            {
                battling = false;
                enemyPartyList[enemyBattling].battling = false;
                enemyPartyList[enemyBattling].alive = false;
                playerParty.battling = false;
                battleCanvas.enabled = false;
                enemyBattling = -1;
                playerParty.ReplenishActionPoints();
            }
        }
        else
        {
            Debug.Log("SHOULD NEVER HAPPEN");
        }
    }

}
