using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RuleTileTest : MonoBehaviour
{
    public Tilemap _tilemap; // assigned in CTOR
    private RuleTile _ruleTile; // assigned in CTOR
    [SerializeField]
    private RuleTile _ruleTileSO; // The RuleTile ScriptableObject is dragged here
    Vector3Int _pos = new Vector3Int(10,10,10);

    void Start()
    {
        DoStuff();
    }

    private void DoStuff()
    {
        _ruleTile = (RuleTile)_tilemap.GetTile(_pos);
        if(_ruleTile == null)
        {
            _ruleTile = (RuleTile)ScriptableObject.Instantiate(_ruleTileSO);
            _tilemap.SetTile(_pos, _ruleTile);
            //_ruleTile.RefreshTile(_pos, _tilemap); // Doesn't compile
        }
    }
    //Tilemap.RefreshAllTiles;
}
