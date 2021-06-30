using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public TutorialManager tutorialMgr;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            TriggerDialogue();
            gameObject.SetActive(false);
        }

    }


    public void TriggerDialogue()
    {
        tutorialMgr.StartDialogue(dialogue);
    }

}
