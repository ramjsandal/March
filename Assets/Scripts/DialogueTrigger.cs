using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public int triggerDistance = 1;
    public Party playerParty;
    private Vector2Int gridPos;

    private GridManager gridManager;

    private void Start()
    {
        gridManager = GridManager.Instance;
        gridManager.SetOccupied(gridManager.GetCellPosition(this.transform.position), true);
        playerParty = FindFirstObjectByType<Party>();
        gridPos = gridManager.GetCellPosition(this.gameObject.transform.position);
    }

    public void TriggerDialogue()
    {
        DialogueManager dm = DialogueManager.Instance;
        Vector2Int playerPos = playerParty.GetSelectedPlayerPosition();
        if (gridManager.IndicateVisible(gridPos, triggerDistance).Select(a => a.position).Contains(playerPos))
        {
            dm.StartDialogue(dialogue);
        }
        else
        {
            Debug.Log("too far lmao ");
        }

    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            TriggerDialogue();
        }

    }
}
