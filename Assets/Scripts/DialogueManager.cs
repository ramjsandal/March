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
        dialogueCanvas.gameObject.SetActive(false);
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
        dialogueCanvas.gameObject.SetActive(true);
        sentences.Clear();
        speakerName.text = dl.name;
        foreach (string sentence in dl.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    private bool typing;
    private string currentSentence;
    IEnumerator TypeSentence(string sentance)
    {
        currentSentence = sentance;
        typing = true;
        speakerSentence.text = "";

        foreach (char c in sentance)
        {
            speakerSentence.text += c;
            yield return new WaitForSeconds(0.01f);
        }

        typing = false;
    }

    public void DisplayNextSentence()
    {
        if (typing)
        {
            StopAllCoroutines();
            speakerSentence.text = currentSentence;
            typing = false;
            return;
        }

        if (sentences.Count <= 0)
        {
            EndDialogue();
            return;
        }

        string sen = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sen));
    }

    public void EndDialogue()
    {
        dialogueCanvas.gameObject.SetActive(false);
        inDialogue = false;
    }

}
