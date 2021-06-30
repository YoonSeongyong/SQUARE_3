using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Block : MonoBehaviour
{
    public Vector2Int posIndex;     //타일 인덱스
    public BlockManager blockMgr;

    public BlockColor colorType;
    public GameObject outLine;

    public bool isSolid;            //true : 고체, false : 기체
    public bool isPlayerIn = false;
    public bool isInRange = false;
    public bool isBurn;
    public bool isActivated;
    public bool isSelected;

    public GameObject effectObject;
    public Animator effectAnim;

    private void OnMouseDown()
    {
        if (isInRange && colorType != BlockColor.BLACK && colorType != BlockColor.GRAY && !isPlayerIn)
        {
            blockMgr.GetBlockClick(this);
            outLine.SetActive(true);
        }
    }

    public virtual IEnumerator Activating()
    {
        yield return null;
    }

    public virtual void ActivateCheck() { }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && colorType != BlockColor.GRAY)
        {
            isPlayerIn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && colorType != BlockColor.GRAY)
        {
            isPlayerIn = false;
        }
    }


    protected IEnumerator MovingDown()
    {
        Block target = blockMgr.mapArray[posIndex.x, posIndex.y - 1];

        Vector3 targetPos = target.transform.position;
        Vector3 myPos = transform.position;
        Vector3 myGoal = transform.position + Vector3.down;


        for (int i = 0; i <= BlockManager.frame; i++)
        {
            transform.position = Vector3.Lerp(myPos, myGoal, i / BlockManager.frame);
            target.transform.position = Vector3.Lerp(targetPos, myPos, i / BlockManager.frame);
            yield return null;
        }
    }

}