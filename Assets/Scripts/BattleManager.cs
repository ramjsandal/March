using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Agent;
using static Party;

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
    public Image moveButton;
    public Image attackButton;

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
            portScript.healthText.text = $"\n{playerParty.partyMembers[i].health.ToString()}";
            portScript.image.sprite = playerParty.partyMembers[i].portrait;
            portScript.actionPointsText.text = $"\n{playerParty.partyMembers[i].actionPoints.ToString()}";
            portScript.playerIndex = i;
            currentPortrait.gameObject.SetActive(true);
            playerPortraitList.Add(currentPortrait);
        }
    }


    private void UpdateHealth(object sender, EventArgs e)
    {
        for (int i = 0; i < playerPortraitList.Count; i++)
        {
            playerPortraitList[i].healthText.text = $"\n{playerParty.partyMembers[i].health.ToString()}";
        }
    }

    private void UpdateActionPoints(object sender, EventArgs e)
    {
        if (playerPortraitList == null)
        {
            return;
        }

        for (int i = 0; i < playerPortraitList.Count; i++)
        {
            playerPortraitList[i].actionPointsText.text = $"\n{playerParty.partyMembers[i].actionPoints.ToString()}";
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
            battleCanvas.gameObject.SetActive(false);
            playerTurn = true;
            playerParty = GameObject.FindObjectOfType<Party>();
            enemyPartyList = new List<EnemyParty>();
            enemyPartyList.AddRange(GameObject.FindObjectsOfType<EnemyParty>());
            playerParty.SelectedPartyMember += UpdateSelectedPlayer;
            foreach (Player p in playerParty.partyMembers)
            {
                p.StoppedMoving += CheckBattleStart;
                p.TookDamage += UpdateHealth;
                p.UsedActionPoint += UpdateActionPoints;
            }
        }
    }

    private void OnDestroy()
    {
        foreach (Player p in playerParty.partyMembers)
        {
            p.StoppedMoving -= CheckBattleStart;
            p.TookDamage -= UpdateHealth;
            p.UsedActionPoint -= UpdateActionPoints;
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

        if (playerParty.collapsed) playerParty.ExpandParty();
        enemyBattling = fightingGroup;
        battling = true;
        playerParty.battling = true;
        enemyPartyList[fightingGroup].battling = true;
        battleCanvas.gameObject.SetActive(true);
        if (playerPortraitList == null)
        {
            GeneratePlayerPortraits();
            playerParty.SelectPartyMember(playerParty.SelectedMemberIdx);
        }

        HighlightAction();
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

    public IEnumerator EndTurn()
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
                        if (playerParty.NumAliveMembers() == 0)
                        {
                            EndGame();
                            yield break;
                        }
                        bool playerAnimating = playerParty.partyMembers.Where(a => a.animating).Count() > 0;
                        if (!e.moving && !playerAnimating)
                        {
                            e.MakeMove(playerParty.partyMembers.Where(a => a.alive).ToList());
                        }
                        while (e.moving || playerAnimating)
                        {
                            yield return new WaitForSeconds(.25f);
                            playerAnimating = playerParty.partyMembers.Where(a => a.animating).Count() > 0;
                        }
                    }
                }

                playerTurn = true;
                playerParty.ReplenishActionPoints();
                enemyPartyList[enemyBattling].ResetActionPoints();
                playerParty.FocusCameraOnSelectedPlayer();
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
                playerParty.battling = false;
                battleCanvas.gameObject.SetActive(false);
                enemyBattling = -1;
                playerParty.ReplenishActionPoints();
                playerParty.CollapseParty();
            }
        }
        else
        {
            Debug.Log("SHOULD NEVER HAPPEN");
        }

        yield return null;
    }

    //called in ui
    public void EndTurnWrapper()
    {
        if (playerTurn)
        {
            StartCoroutine(EndTurn());
        }
    }

    public void SelectPartyMember(int index)
    {
        playerParty.SelectPartyMember(index);
    }

    private void UpdateSelectedPlayer(object sender, IndexEventArgs e)
    {
        if (!battling)
        {
            return;
        }
        MoveSelectedPortrait(e.OldIndex, e.NewIndex);
        HighlightAction();
    }

    private void HighlightAction()
    {
        Color tint = Player.tint;
        if (playerParty.partyMembers[playerParty.SelectedMemberIdx].GetSelectedAction() == SelectedAction.MOVING)
        {
            moveButton.color = tint;
            attackButton.color = Color.white;
        }
        else
        {
            moveButton.color = Color.white;
            attackButton.color = tint;
        }

    }

    private void MoveSelectedPortrait(int oldIdx, int newIdx)
    {
        Vector3 bottomLeftPos = playerPortraitTemplate.transform.position;
        GameObject currentlySelectedPortrait = playerPortraitList[oldIdx].gameObject;
        Vector3 currentlySelectedPosition = currentlySelectedPortrait.transform.position;

        GameObject nextSelectedPortrait = playerPortraitList[newIdx].gameObject;
        Vector3 nextPos = nextSelectedPortrait.transform.position;

        nextSelectedPortrait.transform.position = currentlySelectedPosition;
        currentlySelectedPortrait.transform.position = nextPos;
    }

    // called in UI
    public void SetSelectedPlayerAction(int action)
    {
        playerParty.SetSelectedPartyMemberMode((SelectedAction)action);
        HighlightAction();
    }

    private void EndGame()
    {
        SceneManager.LoadScene(0);
    }

    public bool EnemiesAlive()
    {

        var enemyParty = enemyPartyList[enemyBattling];
        return enemyParty.NumAliveMembers() > 0;
    }

}
