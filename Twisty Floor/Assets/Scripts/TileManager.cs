using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField]
    Vector2Int arraySize = new Vector2Int(10, 10);
    [SerializeField]
    Tile defaultTilePrefab;

    Tile[,] tiles;


    void Start()
    {
        tiles = new Tile[arraySize.x, arraySize.y];


        // Insert existing tiles into the array
        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            Vector2Int tileCoords = WorldPositionToTileCoords(tile.transform.position);
            if (IsWithinArray(tileCoords))
            {
                tiles[tileCoords.x, tileCoords.y] = tile;
                tile.transform.SetParent(transform, true);
                tile.tileCoords = tileCoords;
                tile.direction = LocalRotationToTileDirection(tile.transform.localRotation);
            }
        }


        // Populate the rest of the array with new tiles
        for (int y = 0; y < arraySize.y; y++)
        {
            for (int x = 0; x < arraySize.x; x++)
            {
                if (tiles[x, y] == null)
                {
                    Tile newTile = Instantiate(defaultTilePrefab, transform);
                    newTile.tileCoords = new Vector2Int(x, y);
                    newTile.transform.localPosition = new Vector3(x, 0, y);
                    tiles[x, y] = newTile;
                }
            }
        }
    }


    public bool IsWithinArray(Vector2Int tileCoords)
    {
        return
            tileCoords.x >= 0 &&
            tileCoords.y >= 0 &&
            tileCoords.x < arraySize.x &&
            tileCoords.y < arraySize.y;
    }


    public Vector2Int WorldPositionToTileCoords(Vector3 worldPosition)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
        return new Vector2Int(Mathf.RoundToInt(localPosition.x), Mathf.RoundToInt(localPosition.z));
    }


    // I could make this convert from global rotation but it's not really necessary at this point.
    public int LocalRotationToTileDirection(Quaternion localRotation)
    {
        return Mathf.RoundToInt(localRotation.eulerAngles.y / 90);
    }


    public Vector3 TileCoordsToWorldPosition(Vector2Int tileCoords) { return TileCoordsToWorldPosition((Vector2)tileCoords); }
    public Vector3 TileCoordsToWorldPosition(Vector2 tileCoords)
    {
        return transform.TransformPoint(new Vector3(tileCoords.x, 0, tileCoords.y));
    }


    public Tile GetTile(Vector2Int tileCoords)
    {
        return tiles[tileCoords.x, tileCoords.y];
    }


    public void SetTile(Vector2Int tileCoords, Tile tile)
    {
        tiles[tileCoords.x, tileCoords.y] = tile;
    }
}
