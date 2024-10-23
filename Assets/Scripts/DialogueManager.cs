using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager _instance;
    public static DialogueManager Instance { get { return _instance; } }

    public Canvas dialogueCanvas;
    public TMP_Text speakerName;
    public TMP_Text speakerSentence;

    private Queue<string> sentences;
    private bool inDialogue;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        sentences = new Queue<string>();
        dialogueCanvas.enabled = false;
        inDialogue = false;
    }

    public void StartDialogue(Dialogue dl)
    {
        // if were already in a different dialogue we shouldnt be able to trigger a new one
        if (inDialogue)
        {
            return;
        }

        inDialogue = true;
        dialogueCanvas.enabled = true;
        sentences.Clear();
        speakerName.text = dl.name;
        foreach (string sentence in dl.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    IEnumerator TypeSentence(string sentance)
    {
        Debug.Log("typing next");
        speakerSentence.text = "";

        foreach (char c in sentance)
        {
            speakerSentence.text += c;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void DisplayNextSentence()
    {
        Debug.Log("displaying next");
        if (sentences.Count <= 0)
        {
            Debug.Log("no more sentences");
            EndDialogue();
            return;
        }

        string sen = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sen));
    }

    public void EndDialogue()
    {
        dialogueCanvas.enabled = false;
        inDialogue = false;
    }

}
