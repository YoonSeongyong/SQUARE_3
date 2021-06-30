using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPull : MonoBehaviour
{
    public static ObjectPull instance;

    public GameObject[] blockPrefab;
    List<GameObject> bluePoolList;
    List<GameObject> redPoolList;


    void Start()
    {
        
    }

    //public GameObject SpawnObject()
    //{
    //    if (currentList == 0) AddMissile();
    //    foreach (Missile missile in missileList_Pool)
    //    {
    //        if (!missile.gameObject.activeSelf)
    //        {
    //            currentList--;
    //            return missile.gameObject;
    //        }
    //    }
    //    return null;
    //}

    //public void DespawnObject(GameObject _missile)
    //{
    //    currentList++;
    //    _missile.SetActive(false);
    //}


}
