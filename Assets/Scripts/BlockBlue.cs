using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBlue : Block
{

    /*
     파랑 블럭 효과 : 아래에 다른색 블록이 있는 경우 아래 블록과 위치를 바꾼다.(움직이지 않은 검정 블럭 제외)
     */


    public override void ActivateCheck()
    {
        if(posIndex.y != 0 && !isBurn)
        {
            if (blockMgr.mapArray[posIndex.x, posIndex.y - 1].colorType == BlockColor.BLACK)
            {
                BlockBlack blockBlack = blockMgr.mapArray[posIndex.x, posIndex.y - 1].GetComponent<BlockBlack>();
                if (blockBlack.isMoved)
                {
                    isActivated = true;
                    return;
                }
                else
                    isActivated = false;
            }
            else if (blockMgr.mapArray[posIndex.x, posIndex.y - 1].colorType != BlockColor.BLUE)
            {
                isActivated = true;
                return;
            }

        }

        isActivated = false;
    }
    
    public void PreChange()
    {
        if (isActivated)
        {
            int x = posIndex.x;
            int y = posIndex.y;
            blockMgr.mapArray[x, y] = blockMgr.mapArray[x, y - 1];
            blockMgr.mapArray[x, y - 1] = this;

            blockMgr.mapArray[x, y].posIndex = new Vector2Int(x, y);
            blockMgr.mapArray[x, y - 1].posIndex = new Vector2Int(x, y - 1);
        }
    }

    public override IEnumerator Activating()
    {
        Block target = blockMgr.mapArray[posIndex.x, posIndex.y + 1];

        Vector3 targetPos = target.transform.position;
        Vector3 myPos = gameObject.transform.position;

        effectObject.SetActive(true);
        effectAnim.SetTrigger("Activate");
        for (int i = 0; i <= BlockManager.frame; i++)
        {
            transform.position = Vector3.Lerp(myPos, targetPos, i / BlockManager.frame);
            target.transform.position = Vector3.Lerp(targetPos, myPos, i / BlockManager.frame);
            yield return null;
        }

        //effectObject.SetActive(false);
        //effectAnim.SetTrigger("Activate");

    }

}
