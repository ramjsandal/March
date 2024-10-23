using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

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
