using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    private GridManager gridManager;

    private void Start()
    {
        gridManager = GridManager.Instance;
        gridManager.SetOccupied(gridManager.GetCellPosition(this.transform.position), true);
    }

    public void TriggerDialogue()
    {
        DialogueManager dm = DialogueManager.Instance;
        dm.StartDialogue(dialogue);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            TriggerDialogue();
        }

    }
}
