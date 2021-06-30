using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public BlockManager blockMgr;

    public AudioClip[] clips;

    public float speed, jumpForce;

    public bool isJumping = true;
    public bool isGround = false;
    public bool isRangeIn = false;
    //public bool isSoundPlaying = false;
    bool isWalking = false;
    
    
    Animator anim;
    SpriteRenderer sprRenderer;
    Rigidbody2D rigidbody2d;
    AudioSource audioSource;

    public GameObject effectObject;
    public GameObject jumpObject;
    public Animator effectAnim;

    public int goal;


    int itemCount = 0;
    Vector3 direction;
    Vector2 jumpVelocity;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        sprRenderer = GetComponent<SpriteRenderer>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        Init();
    }

    public void Init()
    {
        isJumping = false;
        isGround = false;
        isRangeIn = false;
        //isSoundPlaying = false;
        isWalking = false;

        jumpVelocity = new Vector2(0, jumpForce);

        anim.SetBool("Idle", true);
        anim.SetBool("Walk", false);
        anim.SetBool("Translate", false);
    }

    void Move()
    {
        direction = Vector3.zero;
        if (Input.GetKey(KeyCode.A))
        {
            direction = Vector2.left;
            anim.SetBool("Walk", true);
            if (!isJumping)
            {
                effectAnim.SetBool("Idle", false);
                effectAnim.SetBool("Walk_Right", false);
                effectAnim.SetBool("Walk_Left", true);
            }
            isWalking = true;

            effectObject.GetComponent<SpriteRenderer>().flipX = false;
            sprRenderer.flipX = false;

        }
        else if (Input.GetKey(KeyCode.D))
        {
            direction = Vector2.right;
            anim.SetBool("Walk", true);
            if (!isJumping)
            {
                effectAnim.SetBool("Idle", false);
                effectAnim.SetBool("Walk_Left", false);
                effectAnim.SetBool("Walk_Right", true);
            }
            isWalking = true;

            effectObject.GetComponent<SpriteRenderer>().flipX = true;
            sprRenderer.flipX = true;

        }
        else
        {
            effectAnim.SetBool("Idle", true);
            effectAnim.SetBool("Walk_Right", false);
            effectAnim.SetBool("Walk_Left", false);
            anim.SetBool("Walk", false);
            isWalking = false;
            audioSource.Stop();
        }
        if(!audioSource.isPlaying && isWalking)
        {
            audioSource.clip = clips[0];
            audioSource.Play();
        }
        transform.position += direction * speed * Time.deltaTime;
    }

    void Jump()
    {
        if (!isJumping) return;

        //Debug.Log(GameManager.instance.currentState);
        //Debug.Log("점프");
        anim.SetTrigger("Jump");
        anim.SetBool("Idle", false);
        effectAnim.SetBool("Jump", true);
        rigidbody2d.velocity = Vector2.zero;

        Instantiate(jumpObject, this.transform.position, Quaternion.identity);

        PlaySound(1);
        rigidbody2d.AddForce(jumpVelocity, ForceMode2D.Impulse);
        isJumping = false;
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.currentState == GameState.PLAYING)
        {
            if ((blockMgr.isChangeGoing) || (blockMgr.isMoveGoing))
            {
            }
            else
            {
                Move();
                Jump();
            }
        }
        else
        {
            audioSource.Stop();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isJumping && isGround)
            {
                isJumping = true;
                isGround = false;
            }
        }
    }

    public void SetAnim(int index, bool tempB)
    {
        switch (index)
        {
            case 0: //변환 애니메이션
                anim.SetBool("Translate", tempB);
                break;
            case 1:
                anim.SetTrigger("Timeout");
                //effectAnim.SetTrigger("Timeout");
                break;
        }
    }
    public void DefeatGame()
    {
        UIManager.instance.DefeatGame();
    }

    public void GroundCheck()
    {
        isJumping = false;
        isGround = true;
        anim.SetBool("Idle", true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            if (SceneManager.GetActiveScene().buildIndex != 4)
            {
                anim.SetTrigger("GetItem");
                GameManager.instance.ClearStage();
            }
            else
            {
                anim.SetTrigger("Clear");
                //GameManager.instance.ClearGame();
            }
        }
    }
    public void GetItem()
    {
        itemCount++;
        if (itemCount == goal)
        {
            anim.SetTrigger("Clear");
        }
    }

    public void ClearGame()
    {
        Time.timeScale = 0;
        GameManager.instance.ClearGame();
    }

    public void PlaySound(int index)
    {
        if (index != 5)
        {
            SoundManager.instance.PlaySound(clips[index]);
        }
        else
        {
            SoundManager.instance.StopSound();
        }
    }
}
