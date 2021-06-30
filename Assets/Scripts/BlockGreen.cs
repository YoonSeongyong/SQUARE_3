using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGreen : Block
{
    public SpriteRenderer sprite;
    public BoxCollider2D boxCol;
    public Sprite[] blockSprite; 

    /*
     * 초록 블럭 효과 :
     * 아래블럭이 고체 블록이 있는 경우 고체 블록이 되지만, 
     * 없을 경우 기체 블록이 된다.
     */



    public override void ActivateCheck()
    {
        bool tempSolid;

        if (isBurn)
        {
            isActivated = false;
            return;
        }

        if (posIndex.y != 0)
        {
            if (blockMgr.mapArray[posIndex.x, posIndex.y - 1].isSolid)
            {
                tempSolid = true;
            }
            else
            {
                tempSolid = false;
            }
        }
        else
        {
            tempSolid = false;
        }
        if (isSolid != tempSolid)
            isActivated = true;
        else
            isActivated = false;

    }

    public override IEnumerator Activating()
    {
        effectObject.SetActive(true);

        if (!isSolid)
            effectAnim.SetTrigger("Solid");
        else
            effectAnim.SetTrigger("Air");

        for (int i = 0; i <= BlockManager.frame; i++)
            {
                if (isSolid)
                {
                    sprite.color = new Color(1, 1, 1, 1 - i / BlockManager.frame * 0.5f);
                gameObject.GetComponent<SpriteRenderer>().sprite = blockSprite[0]; // outLine.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1); //
            }
                else
                {
                    sprite.color = new Color(1, 1, 1, 0.25f + i / BlockManager.frame * 0.5f);
                gameObject.GetComponent<SpriteRenderer>().sprite = blockSprite[1]; //   outLine.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.25f);//
            }
                yield return null;
            }
        if (isSolid)
        {
            sprite.color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            sprite.color = new Color(1, 1, 1,1);
        }

        isSolid = !isSolid;
        boxCol.isTrigger = !boxCol.isTrigger;

    }

}
