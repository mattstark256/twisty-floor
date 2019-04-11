using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(TileManager))]
public class RaiseTiles : MonoBehaviour
{
    [SerializeField]
    float riseHeight = 0.2f;
    [SerializeField]
    float riseSpeed = 2f;
    [SerializeField]
    float fallSpeed = 1f;

    List<Tile> selectedTiles = new List<Tile>();

    // The risingTileRepeats list has an int corresponding to each element of the risingTiles list. If an tile is to be added to the list but it already contains it, the int is incremented.
    List<Tile> risingTiles = new List<Tile>();
    List<int> risingTileRepeats = new List<int>();

    List<Tile> fallingTiles = new List<Tile>();

    TileManager tileManager;


    // Start is called before the first frame update
    void Start()
    {
        tileManager = GetComponent<TileManager>();

    }


    // Update is called once per frame
    void Update()
    {
        UpdateTileHeights();
    }


    public void UpdateSelectedTiles(Vector2Int cursorPosition)
    {
        while (selectedTiles.Count > 0)
        {
            LowerTile(selectedTiles[0]);
            selectedTiles.RemoveAt(0);
        }

        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 2; x++)
            {
                Vector2Int tileCoords = cursorPosition + new Vector2Int(x, y);
                if (tileManager.IsWithinArray(tileCoords))
                {
                    Tile tile = tileManager.GetTile(tileCoords);
                    RaiseTile(tile);
                    selectedTiles.Add(tile);
                }
            }
        }
    }


    public void RaiseTile(Tile tile)
    {
        if (fallingTiles.Contains(tile))
        {
            fallingTiles.Remove(tile);
        }

        bool tileIsInList = false;
        for (int i = 0; i < risingTiles.Count; i++)
        {
            if (risingTiles[i] == tile)
            {
                tileIsInList = true;
                risingTileRepeats[i]++;
            }
        }
        if (!tileIsInList)
        {
            risingTiles.Add(tile);
            risingTileRepeats.Add(1);
        }
    }


    public void LowerTile(Tile tile)
    {
        int elementToBeRemoved = -1;
        for (int i = 0; i < risingTiles.Count; i++)
        {
            if (risingTiles[i] == tile)
            {
                risingTileRepeats[i]--;
                if (risingTileRepeats[i] == 0)
                {
                    elementToBeRemoved = i;
                }
            }
        }
        if (elementToBeRemoved != -1)
        {
            fallingTiles.Add(risingTiles[elementToBeRemoved]);
            risingTiles.RemoveAt(elementToBeRemoved);
            risingTileRepeats.RemoveAt(elementToBeRemoved);
        }
    }


    void UpdateTileHeights()
    {
        foreach (Tile tile in risingTiles)
        {
            tile.transform.localPosition += Vector3.up * Time.deltaTime * riseSpeed;
            if (tile.transform.localPosition.y > riseHeight)
            {
                tile.transform.localPosition = new Vector3(tile.transform.localPosition.x, riseHeight, tile.transform.localPosition.z);
            }
        }

        List<Tile> tilesToRemove = new List<Tile>();
        foreach (Tile tile in fallingTiles)
        {
            tile.transform.localPosition += Vector3.down * Time.deltaTime * fallSpeed;
            if (tile.transform.localPosition.y < 0)
            {
                tile.transform.localPosition = new Vector3(tile.transform.localPosition.x, 0, tile.transform.localPosition.z);
                tilesToRemove.Add(tile);
            }
        }
        while (tilesToRemove.Count > 0)
        {
            fallingTiles.Remove(tilesToRemove[0]);
            tilesToRemove.RemoveAt(0);
        }
    }
}
