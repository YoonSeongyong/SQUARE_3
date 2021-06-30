using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRed : Block
{
    public Block grayBlockPrefab;
    /*
    빨강 블럭 효과 :위에 있는 붉은색, 회색 블록을 제외한 다른색 블록을 회색블록으로 바꾼다.
    */

    GameObject targetObject; //없앨 블럭
    public override void ActivateCheck()
    {
        if (posIndex.y != blockMgr.mapArray.GetLength(1) - 1)
        {
            Block targetBlockScript = blockMgr.mapArray[posIndex.x, posIndex.y + 1];

            if(targetBlockScript.colorType == BlockColor.BLACK)
            {
                BlockBlack blockBlack = targetBlockScript.GetComponent<BlockBlack>();
                if(blockBlack.isMoved)
                {
                    targetObject = targetBlockScript.gameObject;
                    targetBlockScript.isBurn = true;
                    isActivated = true;

                    Block grayBlock = Instantiate(grayBlockPrefab, new Vector3(posIndex.x + 0.5f, posIndex.y + 1.5f, 0), Quaternion.identity);

                    blockMgr.mapArray[posIndex.x, posIndex.y + 1] = grayBlock;
                    grayBlock.posIndex = new Vector2Int(posIndex.x, posIndex.y + 1);

                    return;
                }
            }
            if (targetBlockScript.colorType == BlockColor.BLUE || targetBlockScript.colorType == BlockColor.GREEN || targetBlockScript.colorType == BlockColor.BROWN)
            {
                targetObject = targetBlockScript.gameObject;
                targetBlockScript.isBurn = true;
                isActivated = true;

                Block grayBlock = Instantiate(grayBlockPrefab, new Vector3(posIndex.x + 0.5f, posIndex.y + 1.5f, 0), Quaternion.identity);

                blockMgr.mapArray[posIndex.x, posIndex.y + 1] = grayBlock;
                grayBlock.posIndex = new Vector2Int(posIndex.x, posIndex.y + 1);

                return;
            }
        }
        isActivated = false;

    }

    public override IEnumerator Activating()
    {
        effectObject.SetActive(true);
        effectAnim.SetTrigger("Activate");
        yield return new WaitForSeconds(BlockManager.frame /120);


        Destroy(targetObject);

        yield return new WaitForSeconds(BlockManager.frame /120);
    }
}
