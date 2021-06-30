using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject scriptPanel;
    public GameObject[] scriptCollider;
    public Text dialogue, speakerTxt;

    private Queue<string> sentences;
    private Queue<string> speakers;
    public int dlgIndex = 0;
    //public Queue<GameObject> dialogueCol;

    private void Start()
    {
        sentences = new Queue<string>();
        speakers = new Queue<string>();
    }

    public void Init()
    {
        dlgIndex = 0;
        foreach(GameObject gameObject in scriptCollider)
        {
            gameObject.SetActive(true);
        }
    }



    public void StartDialogue(Dialogue dialogue)
    {
        //Debug.Log("Starting Conversation" + dialogue.name);

        GameManager.instance.PauseGame();
        scriptPanel.SetActive(true);
        sentences.Clear();

        foreach (string tempS in dialogue.sentences)
            sentences.Enqueue(tempS);

        foreach (string name in dialogue.speaker)
            speakers.Enqueue(name);

        DisplayNextSentences();
    }

    public void DisplayNextSentences()
    {
        if (sentences.Count == 0)
        {
            //대화 종료
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        string speaker = speakers.Dequeue();
        dialogue.text = sentence;
        speakerTxt.text = speaker;


    }

    private void Update()
    {
        if (GameManager.instance.currentState == GameState.PAUSE)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DisplayNextSentences();
            }
        }
    }


    public void EndDialogue()
    {
        scriptPanel.SetActive(false);
        GameManager.instance.ResumeGame();
        // scriptCollider[dlgIndex++].SetActive(false);
    }

}
