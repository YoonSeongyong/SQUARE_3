using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class BlockBlack : Block
{
    public bool isMoved;

    /*
     *  검정 블럭 효과
     *  위치가 변환된 후 부터 이동, 낙하 가능
     */
    
    public override void ActivateCheck()
    {
        if(isMoved && posIndex.y != 0 && !isBurn)
        {
            if (!blockMgr.mapArray[posIndex.x,posIndex.y-1].isSolid && blockMgr.mapArray[posIndex.x, posIndex.y - 1].colorType != BlockColor.BLUE)
            {
                isActivated = true;
                return;
            }
        }

        isActivated = false;

    }

    public override IEnumerator Activating()
    {
        StartCoroutine(MovingDown());

        yield return null;
    }
}
