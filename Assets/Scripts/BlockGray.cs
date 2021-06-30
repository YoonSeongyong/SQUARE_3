using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGray : Block
{
     /*
      * 회색 블럭 : 효과 없음.
      * 
      */


    public override void ActivateCheck()
    {

    }

    public override IEnumerator Activating()
    {
        yield return null;
    }

}
