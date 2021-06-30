using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject pausepanel;
    public GameObject timeOutpanel;
    public GameObject clearpanel;


    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void PauseButton()
    {
        Debug.Log("일시정지");
        pausepanel.SetActive(true);
        GameManager.instance.PauseGame();
    }

    public void ResumeButton()
    {
        pausepanel.SetActive(false);
        GameManager.instance.ResumeGame();
    }

    public void ReStartButton()
    {
        pausepanel.SetActive(false);
        clearpanel.SetActive(false);
        timeOutpanel.SetActive(false);
        GameManager.instance.RetryGame();

    }

    public void ClearGame()
    {
        clearpanel.SetActive(true);
    }

    public void DefeatGame()
    {
        //Time.timeScale = 0;
        timeOutpanel.SetActive(true);
    }

    public void ExitButton()
    {
        Application.Quit();
        //pausepanel.SetActive(false);
    }

    public void TitleButton()
    {
        GameManager.instance.ChangeScene(0);
    }

    public void SceneChangeButton(int sceneIndex)
    {
        GameManager.instance.ChangeScene(sceneIndex);
    }
}
