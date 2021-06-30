using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeCheck : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Block" && collision.GetComponent<Block>().colorType != BlockColor.BLACK)
        {
            collision.GetComponent<Block>().isInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Block")
        {
            collision.GetComponent<Block>().isInRange = false;
        }
    }

}
