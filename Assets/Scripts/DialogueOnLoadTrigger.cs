using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueOnLoadTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    private void Start()
    {
        TriggerDialogue();
    }
    public void TriggerDialogue()
    {
        DialogueManager dm = DialogueManager.Instance;
        dm.StartDialogue(dialogue);
    }



}
