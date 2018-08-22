using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {

    int rowSize;

    public float moveSpeed = 0.5f;

    public List<TileController> tiles;

    public bool inputLock = false;

    public int shuffleCount = 40;
    public float shuffleSpeed = 0.1f;

    private void Start()
    {
        rowSize = (int) Mathf.Sqrt(tiles.Count);
        //ShuffleBoard();
        StartCoroutine(Shuffle());
    }

    public bool TilePressed (TileController tile, bool shuffle)
    {
        //Debug.Log("Tile pressed: "+ tile.gameObject.name);
        //SwithTiles(tiles[2],tiles[1]);

        if (inputLock)
            return false;

        TileController emptyNeighbour = EmptyNeighbour(tile);
        if (emptyNeighbour == null)
            return false;
        float speed = shuffle ? shuffleSpeed : moveSpeed;
        SwitchTiles(tile, emptyNeighbour,speed);
        return true;
    }

    IEnumerator Shuffle()
    {
        for(int i = 0; i < shuffleCount; i++)
        {
            if(TilePressed(tiles[Random.Range(0, tiles.Count)],true))
                yield return new WaitForSeconds(shuffleSpeed);

        }


    }
    /*
    void ShuffleBoard()
    {
        for(int i = 0; i < 100; i++)
        {
            TilePressed(tiles[Random.Range(0, tiles.Count)]);
        }
    }
    */
    void SwitchTiles(TileController tile1, TileController tile2,float speed)
    {
        inputLock = true;

        //change place on screen
        Vector3 pos = tile1.gameObject.transform.position;
        LeanTween.move(tile1.gameObject, tile2.gameObject.transform.position, speed)
            .setEase(LeanTweenType.easeInCubic);
       
        LeanTween.move(tile2.gameObject, pos, speed)
            .setEase(LeanTweenType.easeInCubic)
            .setOnComplete(() => 
            {
                inputLock = false;
            });

        //tile1.gameObject.transform.position = tile2.gameObject.transform.position;
        //tile2.gameObject.transform.position = pos;

        //change position in list
        int index1 = TileIndex(tile1);
        int index2 = TileIndex(tile2);

        tiles[index1] = tile2;
        tiles[index2] = tile1;

    }

    List<TileController> Neighbours(TileController tile)
    {
       
        int index = TileIndex(tile);
        int over = index - rowSize;
        List<TileController> neighbours = new List<TileController>();
        if(InRange(over))
            neighbours.Add(tiles[over]);

        int under = index + rowSize;
        if (InRange(under))
        {
            neighbours.Add(tiles[under]);
        }

        int right = index + 1;
        if (InRange(right) && (index +1) % rowSize != 0)
            neighbours.Add(tiles[right]);

        int left = index - 1;
        if (InRange(left) && index % rowSize != 0)
            neighbours.Add(tiles[left]);

        return neighbours;
    }

    bool InRange(int index)
    {
        return (index >= 0 && index < tiles.Count);
    }

    int TileIndex (TileController tile)
    {
        return tiles.IndexOf(tile);
    }

    TileController EmptyNeighbour(TileController tile)
    {
        foreach(TileController t in Neighbours(tile))
        {
            if(t.empty)
            {
                return t;
            }
        }
        return null;
    }
}
