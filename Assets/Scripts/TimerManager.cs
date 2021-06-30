using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    public static TimerManager instance;
    public float gameTime;
    float timer = 0, second = 0;
    TextMeshPro timerText;
    public Image secondHand;
    public Image clockLeft;

    void Awake()
    {
        if(instance == null)
            instance = this;
    }

    // Update is called once per frame
    public void Init()
    {
        timer = 0;
        second = 0;
        clockLeft.fillAmount = 1;
    }

    void Update()
    {
        if (GameManager.instance.currentState == GameState.PLAYING)
        {
            timer += Time.deltaTime;
            second += Time.deltaTime;
            if (second >= 1.0f)
            {
                //clockLeft.fillAmount = Vector2.Lerp(myPos, myGoal, i / BlockManager.frame);
                clockLeft.fillAmount -= 1 / gameTime;
                secondHand.transform.Rotate(0, 0, -(360 / gameTime));
                second = 0f;
                if (timer > gameTime)
                {
                    GameManager.instance.DefeatGame();
                    timer = 0f;
                }
            }
        }
    }
}
