using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public enum GameState { PAUSE, PLAYING, GAMEOVER, GAMECLEAR,STAGECLEAR};

public class GameManager : MonoBehaviour
{
    #region 변수선언

    public static GameManager instance;

    public GameState currentState = GameState.PAUSE;

    public GameObject playerPrefab;
    public Transform playerSpawner;

    public Texture2D[] mouseCursor;
    public Texture2D currentCursor;
    public Vector2 mouseHotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    public TutorialManager tutorialManager;
    public GameObject targetObject;


    //public Animator fadeAnim;
    public bool isPlaying = false;

    public bool isClicked = false;

    public CanvasGroup   fadePanel;

    float endTimer = 0;
    private float fadeDuration = 1;

    #endregion

    #region 초기설정
    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            StartGame();
        }
    }
    #endregion

    public void StartGame()
    {
        currentCursor = mouseCursor[0];
        Cursor.SetCursor(currentCursor, mouseHotSpot, cursorMode);
        Time.timeScale = 1;
        SpawnCharacter();
        SetGameState(GameState.PLAYING);
    }
    
    public void ClearStage()
    {
        StartCoroutine(EndingGame());
    }

    public void ClearGame()
    {
        UIManager.instance.ClearGame();
        //fadeAnim.SetTrigger("Clear");
    }

    //0 : 기본, 1 : 선택가능, 2 : 선택
    public void ChangeMouseCursor(bool _isClicked)
    {
        isClicked = _isClicked;
    }

    public void DefeatGame()
    {
        //StartCoroutine(EndingGame());
        SetGameState(GameState.GAMEOVER);
        playerPrefab.GetComponent<PlayerScript>().SetAnim(1,true); //타임아웃 애니메이션 실행
    }

    public void RetryGame()
    {
        Time.timeScale = 1;
        SpawnCharacter();
        playerPrefab.GetComponent<PlayerScript>().Init();
        BlockManager.instance.ClearBlock();
        BlockManager.instance.SpawnBlock();
        TimerManager.instance.Init();
        tutorialManager.Init();
        SetGameState(GameState.PLAYING);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        SetGameState(GameState.PAUSE);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        SetGameState(GameState.PLAYING);
    }

    public void ChangeScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    void SetGameState(GameState newState)
    {
        currentState = newState;
    }

    private void Update()
    {
        if (currentState == GameState.PLAYING)
        {
            if(isClicked)
            {
                currentCursor = mouseCursor[2];
            }else
            {            
                if(GetCursorOverObject())
                {
                    currentCursor = mouseCursor[1];
                }else{
                    currentCursor = mouseCursor[0];
                }
            }
            Cursor.SetCursor(currentCursor, mouseHotSpot, cursorMode);

        }else if(currentState == GameState.GAMEOVER)
        {
            //endTimer += Time.deltaTime;

            //if (endTimer >= 3.0f)
            //{
            //}
        }else if(currentState == GameState.STAGECLEAR)
        {
            //Starg
        }
    }

    IEnumerator EndingGame()
    {
        yield return new WaitForSeconds(1.0f);
        for (; endTimer < fadeDuration;) {
            endTimer += Time.deltaTime;

            //Debug.Log(endTimer);
            fadePanel.alpha = endTimer / fadeDuration;
            yield return null;
            if (endTimer > fadeDuration)
            {
                //ChangeScene(SceneManager.GetActiveScene().buildIndex+1);
            } 
        }

        ChangeScene(SceneManager.GetActiveScene().buildIndex + 1);
        //UIManager.instance.DefeatGame();
    }

    public bool GetCursorOverObject()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D raycastHit2D = Physics2D.Raycast(pos, Vector2.zero, 0f);

        if(raycastHit2D.collider != null)
        {
            targetObject = raycastHit2D.collider.gameObject;
            if (targetObject.tag == "Block")
            {
                if (targetObject.GetComponent<Block>().isInRange)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void SpawnCharacter()
    {
        //Instantiate(playerPrefab, playerSpawner.position, Quaternion.identity);
        playerPrefab.transform.position = new Vector3(playerSpawner.position.x, playerSpawner.position.y, playerSpawner.position.z);

    }
}
