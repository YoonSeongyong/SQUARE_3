using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockBrown : Block
{
    /*
     *갈색 블럭 효과 : 다른 갈색 블록과 연결되어 있을 경우 기체블록 위에도 떠있을 수 있다.
     */

    public enum BrownCheck { UNCHECK, CHECK, FALL, STAY };
    public BrownCheck isBrownChecked; //다른 갈색 블럭이 낙하체크를 했는지


    public override void ActivateCheck()
    {
        if(isBurn)
        {
            isActivated = false;
            isBrownChecked = BrownCheck.CHECK;
            return;
        }
        if (isBrownChecked == BrownCheck.UNCHECK)
        {
            List<BlockBrown> brownList = new List<BlockBrown>();
            List<BlockBrown> nowList = new List<BlockBrown>();
            List<BlockBrown> nextList = new List<BlockBrown>();

            brownList.Add(this);
            nowList.Add(this);
            isBrownChecked = BrownCheck.CHECK;

            while (nowList.Count != 0)
            {
                foreach (BlockBrown nowBlock in nowList)
                {
                    foreach (BlockBrown block in FindFourWay(nowBlock.posIndex))
                    {
                        if (block.isBrownChecked == BrownCheck.UNCHECK)
                        {
                            nextList.Add(block);
                            brownList.Add(block);
                            block.isBrownChecked = BrownCheck.CHECK;
                        }
                    }
                }
                nowList = nextList.ToList();
                nextList.Clear();
            }

            foreach (BlockBrown blockBrown in brownList)
            {
                if (blockBrown.posIndex.y == 0)
                {
                    isBrownChecked = BrownCheck.STAY;
                    break;
                }
                else if ((blockMgr.mapArray[blockBrown.posIndex.x, blockBrown.posIndex.y - 1].isSolid && blockMgr.mapArray[blockBrown.posIndex.x, blockBrown.posIndex.y - 1].colorType != BlockColor.BROWN) || blockMgr.mapArray[blockBrown.posIndex.x, blockBrown.posIndex.y - 1].colorType == BlockColor.BLUE)
                {
                    isBrownChecked = BrownCheck.STAY;
                    break;
                }
                else
                {
                    isBrownChecked = BrownCheck.FALL;
                }
            }

            foreach (BlockBrown blockBrown in brownList)
            {
                blockBrown.isBrownChecked = isBrownChecked;
                if (isBrownChecked == BrownCheck.FALL)
                    blockBrown.isActivated = true;
                if (isBrownChecked == BrownCheck.STAY)
                    blockBrown.isActivated = false;
            }
        }
    }
    public List<BlockBrown> FindFourWay(Vector2Int _posIndex)
    {
        List<Block> tempList = new List<Block>();
        List<BlockBrown> brownList = new List<BlockBrown>();

        if (_posIndex.x != 0)
            tempList.Add(blockMgr.mapArray[_posIndex.x - 1, _posIndex.y]);
        if (_posIndex.y != 0)
            tempList.Add(blockMgr.mapArray[_posIndex.x, _posIndex.y - 1]);
        if (_posIndex.x != blockMgr.mapArray.GetLength(0) - 1)
            tempList.Add(blockMgr.mapArray[_posIndex.x + 1, _posIndex.y]);
        if (_posIndex.y != blockMgr.mapArray.GetLength(1) - 1)
            tempList.Add(blockMgr.mapArray[_posIndex.x, _posIndex.y + 1]);

        foreach (Block block in tempList)
        {
            if (block.colorType == BlockColor.BROWN && !block.isBurn)
            {
                BlockBrown blockBrown = block.GetComponent<BlockBrown>();
                brownList.Add(blockBrown);
            }
        }
        return brownList;
    }

    public override IEnumerator Activating()
    {
        int x = posIndex.x;
        int y = posIndex.y;
        bool isTargetExist = false;

        Block target = blockMgr.mapArray[x, y - 1];

        effectAnim.SetTrigger("Activate");

        Vector3 myPos = transform.position;
        Vector3 myGoal = transform.position + Vector3.down;
        Vector3 targetPos = target.transform.position;
        Vector3 goalPos = Vector3.zero;


        if (target.colorType != BlockColor.BROWN)
        {
            int checkY = posIndex.y;
            isTargetExist = true;
            while (checkY != blockMgr.mapArray.GetLength(1) - 1 && blockMgr.mapArray[posIndex.x, checkY + 1].colorType == BlockColor.BROWN)
            {
                checkY++;
            }
            goalPos = blockMgr.mapArray[x, checkY].transform.position;
        }

        for (int i = 0; i <= BlockManager.frame; i++)
        {
            transform.position = Vector3.Lerp(myPos, myGoal, i / BlockManager.frame);
            if (isTargetExist)
                target.transform.position = Vector3.Lerp(targetPos, goalPos, i / BlockManager.frame);
            yield return null;
        }

        isBrownChecked = BrownCheck.UNCHECK;

    }


}
