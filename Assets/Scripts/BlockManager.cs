using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;
#region 기본 구조
[System.Serializable]
public struct BlockRow
{
    public BlockColor[] blocks;
}

public enum BlockColor { DEFAULT, RED, BLUE, GREEN, BROWN, GRAY, BLACK }
#endregion

public class BlockManager : MonoBehaviour
{
    #region 변수 선언
    public static BlockManager instance;

    [SerializeField]
    public BlockRow[] blockArray;
    public GameObject[] blockPrefab;        //빨-파-녹-갈-회-검
    public Sprite[] graySprite;
    public Sprite[] blackSprite;


    public Tilemap tilemap;
    public Block[,] mapArray;               //모든 블럭배열

    //public Tilemap outlineMap;
    //public RuleTile blockOutLine;

    public Transform selectedParent;        //선택된 블럭들의 부모
    public List<Block> selectedBlock;       //위치 변환될 블럭

    public AudioClip[] clips;

    public PlayerScript playerScript;
    public Animator effectAnim;

    public static float frame = 30;
    public bool dataSave = false;

    bool ProcessGoing = true;
    public bool isMoveGoing;                //움직이고 있는지
    public bool isChangeGoing;              //변하고 있는지
    public bool isTargetError = false;


    #endregion

    #region 초기 세팅
    private void InitBlockArray()
    {
        /*
         블럭 배열 데이터를 저장하는 함수
         */
        int maxX = 0, maxY = 0;

        //맵의 최대 길이, 높이를 계산
        for (int i = 0; i < transform.childCount; i++)
        {
            if (maxX < tilemap.WorldToCell(transform.GetChild(i).position).x)
                maxX = tilemap.WorldToCell(transform.GetChild(i).position).x;

            if (maxY < tilemap.WorldToCell(transform.GetChild(i).position).y)
                maxY = tilemap.WorldToCell(transform.GetChild(i).position).y;
        }

        maxX++;
        maxY++;

        blockArray = new BlockRow[maxY];
        for (int i = 0; i < maxY; i++)
        {
            blockArray[i].blocks = new BlockColor[maxX];
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            int x = tilemap.WorldToCell(transform.GetChild(i).position).x;
            int y = tilemap.WorldToCell(transform.GetChild(i).position).y;
            blockArray[y].blocks[x] = transform.GetChild(i).gameObject.GetComponent<Block>().colorType;
        }

        for (int i = 0; i < maxY; i++)
        {
            for (int j = 0; j < maxX; j++)
            {
                if (blockArray[i].blocks[j] == BlockColor.DEFAULT)
                {
                    blockArray[i].blocks[j] = BlockColor.GRAY;
                }
            }
        }
    }

    public void SpawnBlock()
    {
        Block tempBlock;

        mapArray = new Block[blockArray[0].blocks.Length, blockArray.Length];
        for (int y = 0; y < blockArray.Length; y++)
        {
            for (int x = 0; x < blockArray[0].blocks.Length; x++)
            {
                GameObject temp = Instantiate(blockPrefab[(int)blockArray[y].blocks[x]], new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);


                temp.transform.SetParent(transform);
                tempBlock = temp.GetComponent<Block>();

                tempBlock.posIndex = new Vector2Int(x, y);
                tempBlock.blockMgr = this;

                mapArray[x, y] = tempBlock;

                if (mapArray[x, y].colorType == BlockColor.BLACK)
                {
                    int index = UnityEngine.Random.Range(0, 20);
                    mapArray[x, y].GetComponent<SpriteRenderer>().sprite = blackSprite[index];
                }
                else if (mapArray[x, y].colorType == BlockColor.GRAY)
                {
                    int index = UnityEngine.Random.Range(0, 15);
                    mapArray[x, y].GetComponent<SpriteRenderer>().sprite = graySprite[index];
                    mapArray[x, y].GetComponent<SpriteRenderer>().sortingOrder = -2;
                }
            }
        }
    }


    public void ClearBlock()
    {
        foreach (Block block in mapArray)
        {
            Destroy(block.gameObject);
        }
        //Debug.Log(mapArray.Length);

    }

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        if (dataSave)
            InitBlockArray();
        else
        {
            SpawnBlock();
        }
        //Vector2 pos = new Vector2(15, 15);
        //Instantiate(blockOutLine,pos,Quaternion.identity);
    }
    #endregion





    //블록 클릭 처리 함수
    public void GetBlockClick(Block _block)
    {
        //위치 변환과 블럭효과 발동 중에는 블럭 선택 불가
        if (!isChangeGoing && !isMoveGoing)
        {
            bool tempB = false;
            Vector3 temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selectedParent.position = new Vector3((int)temp.x + 0.5f, (int)temp.y + 0.5f, (int)temp.z);
            isChangeGoing = true;

            selectedBlock = new List<Block>();
            selectedBlock.Add(_block);
            _block.isSelected = true;

            //클릭된 블럭의 사방에서 같은 색의 블럭을 찾고 리스트에 넣어줌
            List<Block> nowList = new List<Block>();
            nowList.Add(_block);

            List<Block> nextList = new List<Block>();

            GameManager.instance.ChangeMouseCursor(true);

            while (nowList.Count != 0)
            {
                foreach (Block nowBlock in nowList)
                {
                    foreach (Block block in FindFourWay(nowBlock.posIndex))
                    {
                        //클릭한 블럭이랑 같은 색상이고 선택된 블럭이 아닌 블럭을 리스트에 추가
                        if (block.colorType == _block.colorType && block.isSelected == false)
                        {
                            if (block.isPlayerIn) tempB = true;
                            nextList.Add(block);
                            selectedBlock.Add(block);
                            block.isSelected = true;
                            block.outLine.SetActive(true);
                        }
                    }
                }
                nowList = nextList.ToList();
                nextList.Clear();
            }

            //선택된 블럭 리스트 후처리
            foreach (Block block in selectedBlock)
            {
                if (!tempB)
                {
                    block.isSelected = false;
                    block.transform.parent = selectedParent;
                    block.GetComponent<SpriteRenderer>().sortingOrder = 2;
                    block.GetComponent<BoxCollider2D>().isTrigger = true;
                }
            }

            if (!tempB)
                StartCoroutine(MovingSelected());
            else
                selectedBlock.Clear();
        }
        else
        {

        }
    }

    //블럭 위치 변환 함수
    IEnumerator MovingSelected()
    {
        yield return null;
        playerScript.SetAnim(0, true);
        effectAnim.SetBool("Translate", true);
        effectAnim.SetBool("Idle", false);
        while (isChangeGoing)
        {
            //선택된 블럭의 이동이 타일 한칸 단위로 되게 계산
            if (selectedParent.position.x - Camera.main.ScreenToWorldPoint(Input.mousePosition).x > 1)
                selectedParent.position += Vector3.left;
            if (selectedParent.position.y - Camera.main.ScreenToWorldPoint(Input.mousePosition).y > 1)
                selectedParent.position += Vector3.down;
            if (selectedParent.position.x - Camera.main.ScreenToWorldPoint(Input.mousePosition).x < -1)
                selectedParent.position += Vector3.right;
            if (selectedParent.position.y - Camera.main.ScreenToWorldPoint(Input.mousePosition).y < -1)
                selectedParent.position += Vector3.up;

            //내려놓기
            if (Input.GetMouseButtonDown(0))
            {
                bool tempB = true;
                foreach (Block block in selectedBlock)
                {
                    int x = (int)block.transform.position.x;
                    int y = (int)block.transform.position.y;
                    block.outLine.SetActive(false);
                    if (mapArray[x, y].transform.parent == selectedParent && (block.posIndex.x != x || block.posIndex.y != y) /*|| block.isPlayerIn*/)
                        tempB = false;

                }
                if (tempB)
                {
                    isChangeGoing = false;
                    isMoveGoing = true;
                }
                GameManager.instance.ChangeMouseCursor(false);

            }


            if (Input.GetKeyDown(KeyCode.Q)) //반시계 회전
            {
                SoundManager.instance.PlaySound(clips[1]);
                selectedParent.Rotate(0, 0, 90);
                foreach (Block block in selectedBlock)
                    block.transform.Rotate(0, 0, -90);
            }
            if (Input.GetKeyDown(KeyCode.E)) //시계 회전
            {
                SoundManager.instance.PlaySound(clips[1]);
                selectedParent.Rotate(0, 0, -90);
                foreach (Block block in selectedBlock)
                    block.transform.Rotate(0, 0, 90);
            }

            yield return null;
        }


        Block temp;
        List<Block> targetList = new List<Block>();
        foreach (Block block in selectedBlock)
        {
            block.GetComponent<SpriteRenderer>().sortingOrder = 0;

            if (block.isSolid)
                block.GetComponent<BoxCollider2D>().isTrigger = false;

            int x = (int)block.transform.position.x;
            int y = (int)block.transform.position.y;


            //검정색 블럭이 움직이면 움직임체크
            if (mapArray[x, y].colorType == BlockColor.BLACK)
            {
                BlockBlack blockBlack = mapArray[x, y].GetComponent<BlockBlack>();
                blockBlack.isMoved = true;
            }


            if (mapArray[x, y] == null) continue;


            //같은 위치의 블럭 이동
            mapArray[x, y].transform.position = new Vector3(block.posIndex.x + 0.5f, block.posIndex.y + 0.5f, 0);
            mapArray[x, y].posIndex = block.posIndex;

            temp = block;

            mapArray[block.posIndex.x, block.posIndex.y] = mapArray[x, y];
            mapArray[x, y] = temp;

            mapArray[x, y].posIndex = new Vector2Int(x, y);

        }


        selectedBlock.Clear();
        selectedParent.DetachChildren();

        SoundManager.instance.PlaySound(clips[0]);

        //모든 효과 블럭이 효과를 다할때까지                    
        ProcessGoing = true;
        while (ProcessGoing)
        {
            yield return StartCoroutine(ActivatingBlock());
        }

        //effectAnim.SetTrigger("Translate");
        effectAnim.SetBool("Idle", true);
        playerScript.SetAnim(0, false);

        foreach (Block block in mapArray)
        {
            if (block.colorType == BlockColor.BLACK)
            {
                BlockBlack blockBlack = block.GetComponent<BlockBlack>();
                blockBlack.isMoved = false;
            }
            if (block.colorType == BlockColor.BROWN)
            {
                BlockBrown blockBrown = block.GetComponent<BlockBrown>();
                blockBrown.isBrownChecked = BlockBrown.BrownCheck.UNCHECK;
            }
        }

        isMoveGoing = false;
        isTargetError = false;



    }

    //블럭 효과 발동 함수
    IEnumerator ActivatingBlock()
    {
        //효과 발동 순서 : 빨강-초록-파랑-낙하

        foreach (Block block in mapArray)
        {
            //Debug.Log("빨강");
            if (block.colorType == BlockColor.RED)
                block.ActivateCheck();
        }

        foreach (Block block in mapArray)
        {
            if (block.colorType == BlockColor.GREEN)
                block.ActivateCheck();
        }

        foreach (Block block in mapArray)
        {
            if (block.colorType == BlockColor.BLUE)
                block.ActivateCheck();
        }

        foreach (Block block in mapArray)
        {
            if (block.colorType == BlockColor.BLUE)
            {
                block.GetComponent<BlockBlue>().PreChange();
            }
        }

        foreach (Block block in mapArray)
        {
            if (block.colorType == BlockColor.BLACK || block.colorType == BlockColor.BROWN)
                block.ActivateCheck();
        }

        ProcessGoing = false;
        foreach (Block block in mapArray)
        {
            if (block.isActivated)
            {
                ProcessGoing = true;
                StartCoroutine(block.Activating());
            }
        }
        yield return new WaitForSeconds(frame / 40);


        foreach (Block block in mapArray)
        {
            if (block.colorType == BlockColor.BROWN)
            {
                BlockBrown blockBrown = block.GetComponent<BlockBrown>();
                blockBrown.isBrownChecked = BlockBrown.BrownCheck.UNCHECK;
            }
        }

        Block[,] tempArray = new Block[mapArray.GetLength(0), mapArray.GetLength(1)];
        foreach (Block block in mapArray)
        {
            int x = (int)block.transform.position.x;
            int y = (int)block.transform.position.y;

            block.posIndex = new Vector2Int(x, y);

            tempArray[x, y] = block;
        }
        mapArray = tempArray;
    }


    //주변 사방의 블록을 반환하는 함수
    public List<Block> FindFourWay(Vector2Int _posIndex)
    {
        List<Block> tempList = new List<Block>();

        if (_posIndex.x != 0)
            tempList.Add(mapArray[_posIndex.x - 1, _posIndex.y]);
        if (_posIndex.y != 0)
            tempList.Add(mapArray[_posIndex.x, _posIndex.y - 1]);
        if (_posIndex.x != mapArray.GetLength(0) - 1)
            tempList.Add(mapArray[_posIndex.x + 1, _posIndex.y]);
        if (_posIndex.y != mapArray.GetLength(1) - 1)
            tempList.Add(mapArray[_posIndex.x, _posIndex.y + 1]);

        return tempList;
    }
}
