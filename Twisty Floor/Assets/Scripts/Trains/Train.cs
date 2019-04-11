using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : Interactive
{
    [SerializeField]
    float defaultSpeed = 1.5f;
    [SerializeField]
    float speed = 1.5f;
    [SerializeField]
    float acceleration = 7f;
    // Distance along the current rail
    [SerializeField]
    float railDistance = 0.5f;
    // Whether the train's forward direction matches the forward direction of the track it is on
    [SerializeField]
    bool alignedWithRail = true;
    // How near the train needs to be to a dead end before it tries changing direction
    [SerializeField]
    float changeDirectionDistance = 0.3f;

    TileManager tileManager;
    Rail currentRail;
    float targetSpeed;


    void Start()
    {
        tileManager = GetComponentInParent<TileManager>();
        if (tileManager == null)
        {
            Debug.Log("No TileManager component found in train parent!");
        }

        currentRail = GetComponentInParent<Rail>();
        if (currentRail == null)
        {
            Debug.Log("No Rail component found in train parent!");
        }

        targetSpeed = speed;
    }


    void Update()
    {
        // accelerate towards target speed
        if (speed > targetSpeed) { speed -= Time.deltaTime * acceleration; if (speed < targetSpeed) speed = targetSpeed; }
        if (speed < targetSpeed) { speed += Time.deltaTime * acceleration; if (speed > targetSpeed) speed = targetSpeed; }

        float railSpeed = alignedWithRail ? speed : -speed;
        railDistance += Time.deltaTime * railSpeed;

        // If they are near the end of the tile and there's no tile to transfer to, change direction
        if (targetSpeed != 0)
        {
            if (railDistance < changeDirectionDistance && !RailIsConnected(currentRail, false))
            {
                targetSpeed = alignedWithRail ? defaultSpeed : -defaultSpeed;
            }
            if (railDistance > currentRail.trackLength - changeDirectionDistance && !RailIsConnected(currentRail, true))
            {
                targetSpeed = alignedWithRail ? -defaultSpeed : defaultSpeed;
            }
        }

        // If their position is outside the limits of the tile, try moving to the next one.
        if (railDistance < 0 && RailIsConnected(currentRail, false) ||
            railDistance > currentRail.trackLength && RailIsConnected(currentRail, true))
        {
            TransferToNextRail(railDistance > 0);
        }

        currentRail.PositionVehicleOnRail(transform, railDistance, alignedWithRail);
    }


    bool RailIsConnected(Rail rail, bool atRailExit)
    {
        Tile railTile = rail.GetComponent<Tile>();

        // Find the adjacent tile in the specified direction
        int connectionDirection = atRailExit ? rail.exitDirection : rail.entryDirection;
        connectionDirection += railTile.direction;
        connectionDirection %= 4;
        Vector2Int adjacentTileCoords = railTile.tileCoords + DirectionToVector(connectionDirection);
        if (!tileManager.IsWithinArray(adjacentTileCoords)) return false;
        Tile adjacentTile = tileManager.GetTile(adjacentTileCoords);

        // Check if it has a connecting rail
        Rail[] adjacentRails = adjacentTile.GetComponents<Rail>();
        int adjacentConnectionDirection = (connectionDirection + 2 - adjacentTile.direction + 4) % 4;
        foreach (Rail adjacentRail in adjacentRails)
        {
            if (adjacentRail.entryDirection == adjacentConnectionDirection || adjacentRail.exitDirection == adjacentConnectionDirection) return true;
        }
        return false;
    }


    // This should only be done if RailIsConnected check returns true
    void TransferToNextRail(bool atRailExit)
    {
        Tile railTile = currentRail.GetComponent<Tile>();

        // Find the adjacent tile in the specified direction
        int connectionDirection = atRailExit ? currentRail.exitDirection : currentRail.entryDirection;
        connectionDirection += railTile.direction;
        connectionDirection %= 4;
        Vector2Int adjacentTileCoords = railTile.tileCoords + DirectionToVector(connectionDirection);
        Tile adjacentTile = tileManager.GetTile(adjacentTileCoords);

        // Transfer to the connecting rail
        Rail[] adjacentRails = adjacentTile.GetComponents<Rail>();
        int adjacentConnectionDirection = (connectionDirection + 2 - adjacentTile.direction + 4) % 4;
        foreach (Rail adjacentRail in adjacentRails)
        {
            if (adjacentRail.entryDirection == adjacentConnectionDirection || adjacentRail.exitDirection == adjacentConnectionDirection)
            {
                bool atAdjacentRailExit = (adjacentConnectionDirection == adjacentRail.exitDirection);

                float overshoot = atRailExit ? railDistance - currentRail.trackLength : -railDistance;
                currentRail = adjacentRail;
                railDistance = atAdjacentRailExit ? adjacentRail.trackLength - overshoot : overshoot;
                if (atRailExit == atAdjacentRailExit) alignedWithRail = !alignedWithRail;
            }
        }
    }


    Vector2Int DirectionToVector(int direction)
    {
        switch (direction)
        {
            case 0:
                return Vector2Int.up;
            case 1:
                return Vector2Int.right;
            case 2:
                return Vector2Int.down;
            case 3:
                return Vector2Int.left;
            default:
                Debug.Log("Invalid argument for DirectionToVector function");
                return Vector2Int.up;
        }
    }


    public override void LeftClick(RaycastHit hitInfo)
    {
        base.LeftClick(hitInfo);

        targetSpeed = (targetSpeed == 0) ? defaultSpeed : 0;
    }


    public override void RightClick(RaycastHit hitInfo)
    {
        base.LeftClick(hitInfo);

        targetSpeed = (targetSpeed == 0) ? -defaultSpeed : 0;
    }
}
