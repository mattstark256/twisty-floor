using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct RotationRequest
{
    public Vector2Int tileCoords;
    public int amount;

    public RotationRequest(Vector2Int _tileCoords, int _amount)
    {
        tileCoords = _tileCoords;
        amount = _amount;
    }
}


[RequireComponent(typeof(TileManager))]
public class TileRotator : Interactive
{
    [SerializeField]
    private float rotationDuration = 0.2f;
    [SerializeField]
    GameObject cursorPrefab;

    TileManager tileManager;
    GameObject cursorObject;
    bool hovering;
    Vector2Int hoveredTile;
    List<RotationRequest> rotationRequests = new List<RotationRequest>();


    private void Awake()
    {
        tileManager = GetComponent<TileManager>();

        cursorObject = Instantiate(cursorPrefab, transform);
    }


    public override void Hover(RaycastHit hitInfo)
    {
        base.Hover(hitInfo);

        hovering = true;
        hoveredTile = tileManager.WorldPositionToTileCoords(hitInfo.point - new Vector3(0.5f, 0, 0.5f));
    }


    public override void LeftClick(RaycastHit hitInfo)
    {
        base.LeftClick(hitInfo);
        
        RotateTiles(hoveredTile, 1);
    }


    public override void RightClick(RaycastHit hitInfo)
    {
        base.LeftClick(hitInfo);
        
        RotateTiles(hoveredTile, -1);
    }


    private void RotateTiles(Vector2Int tileCoords, int amount)
    {
        // Check if the request is valid
        bool requestIsValid = true;
        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 2; x++)
            {
                if (!tileManager.IsWithinArray(new Vector2Int(tileCoords.x + x, tileCoords.y + y)))
                {
                    requestIsValid = false;
                }
            }
        }

        if (requestIsValid)
        {
            // Add it to the stack
            rotationRequests.Add(new RotationRequest(tileCoords, amount));
        }
    }


    private void Update()
    {
        // check if the bottom request on the stack can be carried out
        if (rotationRequests.Count > 0 && RequestCanBeExecuted(rotationRequests[0]))
        {
            Debug.Log("about to execute request");
            ExecuteRequest(rotationRequests[0]);
            rotationRequests.RemoveAt(0);
        }

        if (hovering)
        {
            cursorObject.SetActive(true);
            cursorObject.transform.position = tileManager.TileCoordsToWorldPosition(hoveredTile + Vector2.one * 0.5f);
            hovering = false;
        }
        else
        {
            cursorObject.SetActive(false);
        }
    }


    private bool RequestCanBeExecuted(RotationRequest rotationRequest)
    {
        bool canBeExecuted = true;

        // Check if any of the tiles are locked
        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 2; x++)
            {
                if (tileManager.GetTile(new Vector2Int(rotationRequest.tileCoords.x + x, rotationRequest.tileCoords.y + y)).locked)
                {
                    canBeExecuted = false;
                }
            }
        }

        return canBeExecuted;
    }


    private void ExecuteRequest(RotationRequest rotationRequest) { StartCoroutine(RotateCoroutine(rotationRequest)); }
    private IEnumerator RotateCoroutine(RotationRequest rotationRequest)
    {
        Debug.Log("executing request");

        List<Tile> tiles = new List<Tile>();
        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 2; x++)
            {
                tiles.Add(tileManager.GetTile(new Vector2Int(rotationRequest.tileCoords.x + x, rotationRequest.tileCoords.y + y)));
            }
        }

        // Create a pivot object
        GameObject pivotObject = new GameObject("Pivot Object");
        pivotObject.transform.position = tileManager.TileCoordsToWorldPosition(rotationRequest.tileCoords + new Vector2(0.5f, 0.5f));
        Vector3 initialPivotPosition = pivotObject.transform.position;

        // Attach the tiles to the pivot
        foreach (Tile tile in tiles)
        {
            tile.locked = true;
            tile.transform.SetParent(pivotObject.transform, true);
        }


        float f = 0;
        while (f < 1)
        {
            f += Time.deltaTime / rotationDuration;
            f = Mathf.Clamp01(f);

            // Don't bother avoiding corner intersections
            float smoothedF = Mathf.SmoothStep(0, 1, f);
            pivotObject.transform.rotation = Quaternion.Euler(0, smoothedF * 90 * rotationRequest.amount, 0);

            // Tilt to get corners under
            //pivotObject.transform.rotation = Quaternion.Euler(15 * Mathf.Sin(f * Mathf.PI), 90 * rotationRequest.amount * (1 - Mathf.Cos(f * Mathf.PI)) * 0.5f, 0);

            // Use scaling or raising to get corners under
            //pivotObject.transform.rotation = Quaternion.Euler(0, 90 * rotationRequest.amount * (1 - Mathf.Cos(f * Mathf.PI)) * 0.5f, 0);
            //pivotObject.transform.position = initialPivotPosition + Vector3.down * 0.3f * Mathf.Sin(f * Mathf.PI);
            //pivotObject.transform.localScale = Vector3.one * (1 - 0.3f * Mathf.Sin(f * Mathf.PI));
            // pivotObject.transform.localScale = new Vector3(1 - 0.3f * Mathf.Sin(f * Mathf.PI), 1, 1 - 0.3f * Mathf.Sin(f * Mathf.PI));

            yield return null;
        }


        // Put the tiles back in the grid
        foreach (Tile tile in tiles)
        {
            tile.locked = false;
            tile.transform.SetParent(tileManager.transform, true);
            Vector2Int tileCoords = tileManager.WorldPositionToTileCoords(tile.transform.position);
            tileManager.SetTile(tileCoords, tile);
            tile.tileCoords = tileCoords;
            tile.direction = tileManager.LocalRotationToTileDirection(tile.transform.localRotation);
        }
    }
}
