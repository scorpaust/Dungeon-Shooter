using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector] public Room room;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groundTilemap;
    [HideInInspector] public Tilemap decoration1Tilemap;
    [HideInInspector] public Tilemap decoration2Tilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    [HideInInspector] public Tilemap minimapTilemap;
    [HideInInspector] public int[,] aStarMovementPenalty;
    [HideInInspector] public Bounds roomColliderBounds;

    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    #endregion Header OBJECT REFERENCES
    #region Tooltip
    [Tooltip("Populate with the environment child placeholder game object")]
    #endregion Tooltip
    [SerializeField] private GameObject environmentGameObject;

	private BoxCollider2D boxCollider2D;

	private void Awake()
	{
        boxCollider2D = GetComponent<BoxCollider2D>();

        roomColliderBounds = boxCollider2D.bounds;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag(Settings.playertag) && room != GameManager.Instance.GetCurrentRoom())
		{
            this.room.isPreviouslyVisited = true;

            StaticEventHandler.CallRoomChangedEvent(room);
		}
	}

	public void Initialise(GameObject roomGameObject)
	{
        PopulateTilemapMemberVariables(roomGameObject);

        BlockOffUnusedDoorways();

        AddObstaclesAndPreferedPaths();

        AddDoorsToRooms();

        DisableCollisionTilemapRenderer();
	}

    private void PopulateTilemapMemberVariables(GameObject roomGameObject)
	{
        grid = roomGameObject.GetComponentInChildren<Grid>();

        Tilemap[] tilemaps = roomGameObject.GetComponentsInChildren<Tilemap>();

        foreach (Tilemap tilemap in tilemaps)
		{
            if (tilemap.gameObject.CompareTag("groundTilemap"))
			{
                groundTilemap = tilemap;
			}

            if (tilemap.gameObject.CompareTag("decoration1Tilemap"))
            {
                decoration1Tilemap = tilemap;
            }

            if (tilemap.gameObject.CompareTag("decoration2Tilemap"))
            {
                decoration2Tilemap = tilemap;
            }

            if (tilemap.gameObject.CompareTag("frontTilemap"))
            {
                frontTilemap = tilemap;
            }

            if (tilemap.gameObject.CompareTag("collisionTilemap"))
            {
                collisionTilemap = tilemap;
            }

            if (tilemap.gameObject.CompareTag("minimapTilemap"))
            {
                minimapTilemap = tilemap;
            }
        }
	}

    private void BlockOffUnusedDoorways()
	{
        foreach (Doorway doorway in room.doorwayList)
		{
            if (doorway.isConnected)
                continue;

            if (collisionTilemap != null)
			{
                BlockADoorwayOnTilemapplayer(collisionTilemap, doorway);
			}

            if (minimapTilemap != null)
			{
                BlockADoorwayOnTilemapplayer(minimapTilemap, doorway);
            }

            if (groundTilemap != null)
            {
                BlockADoorwayOnTilemapplayer(groundTilemap, doorway);
            }

            if (decoration1Tilemap != null)
            {
                BlockADoorwayOnTilemapplayer(decoration1Tilemap, doorway);
            }

            if (decoration2Tilemap != null)
            {
                BlockADoorwayOnTilemapplayer(decoration2Tilemap, doorway);
            }

            if (frontTilemap != null)
            {
                BlockADoorwayOnTilemapplayer(frontTilemap, doorway);
            }
        }
	}

    private void BlockADoorwayOnTilemapplayer(Tilemap tilemap, Doorway doorway)
	{
        switch  (doorway.orientation)
		{
            case Orientation.north:
            case Orientation.south:
                BlockDoorwayHorizontally(tilemap, doorway);
                break;

            case Orientation.east:
            case Orientation.west:
                BlockDoorwayVertically(tilemap, doorway);
                break;

            case Orientation.none:
                break;

            default:
                break;
        }
	}

    private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
	{
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
		{
            for (int yPos  = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
			{
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

                tilemap.SetTile(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0),
                    tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), transformMatrix);
			}
		}
	}

    private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
	{
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
		{
            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
			{
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

                tilemap.SetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0),
                    tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), transformMatrix);
            }

        }
    }

    private void AddObstaclesAndPreferedPaths()
	{
        // this array will be populated with all obstacles
        aStarMovementPenalty = new int[room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y - room.templateLowerBounds.y + 1];

        // Loop through all grid squares
        for (int x = 0; x < (room.templateUpperBounds.x - room.templateLowerBounds.x + 1); x++)
		{
            for (int y = 0; y < (room.templateUpperBounds.y - room.templateLowerBounds.y + 1); y++)
			{
                // Set default movement penalty for grid squares
                aStarMovementPenalty[x, y] = Settings.defaultAStarMovementPenalty;

                // Add obstacles for collision tiles the enemy can't walk on
                TileBase tile = collisionTilemap.GetTile(new Vector3Int(x + room.templateLowerBounds.x, y + room.templateLowerBounds.y, 0));

                foreach (TileBase collisionTile in GameResources.Instance.enemyUnwalkableCollisionTilesArray)
				{
                    if (tile == collisionTile)
					{
                        aStarMovementPenalty[x, y] = 0;
                        break;
					}
				}

                if (tile == GameResources.Instance.preferedEnemyPathTile)
				{
                    aStarMovementPenalty[x, y] = Settings.preferedPathAStarMovementPenalty;
				}
			}
		}
	}

    private void AddDoorsToRooms()
	{
        if (room.roomNodeType.isCorridorEW || room.roomNodeType.isCorridorNS) return;

        foreach (Doorway doorway in room.doorwayList)
		{
            if (doorway.doorPrefab != null && doorway.isConnected)
			{
                float tileDistance = Settings.tileSizePixels / Settings.pixelsPerUnit;

                GameObject door = null;

                if (doorway.orientation == Orientation.north)
				{
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);

                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2, doorway.position.y + tileDistance, 0f);
				}
                else if (doorway.orientation == Orientation.south)
				{
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);

                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2, doorway.position.y, 0f);
                }
                else if (doorway.orientation == Orientation.east)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);

                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance, doorway.position.y + tileDistance * 1.25f, 0f);
                }
                else if (doorway.orientation == Orientation.west)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);

                    door.transform.localPosition = new Vector3(doorway.position.x, doorway.position.y + tileDistance, 0f);
                }

                Door doorComponent = door.GetComponent<Door>();

                if (room.roomNodeType.isBossRoom)
				{
                    doorComponent.isBossRoomDoor = true;

                    doorComponent.LockDoor();
				}
            }
		}
	}

    private void DisableCollisionTilemapRenderer()
	{
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
	}

    public void DisableRoomCollider()
	{
        boxCollider2D.enabled = false;
	}

    public void EnableRoomCollider()
	{
        boxCollider2D.enabled = true;
	}

    public void ActivateEnvironmentGameObjects()
	{
        if (environmentGameObject != null)
            environmentGameObject.SetActive(true);
	}

    public void DeactivateEnvironmentGameObjects()
	{
        if (environmentGameObject != null)
            environmentGameObject.SetActive(false);
    }

    public void LockDoors()
	{
        Door[] doorArray = GetComponentsInChildren<Door>();

        // Trigger lock doors
        foreach (Door door in doorArray)
		{
            door.LockDoor();
		}

        // Disable room trigger collider
        DisableRoomCollider();
	}

    public void UnlockDoors(float doorUnlockDelay)
	{
        StartCoroutine(UnlockDoorsRoutine(doorUnlockDelay));
	}

    private IEnumerator UnlockDoorsRoutine(float doorUnlockDelay)
	{
        if (doorUnlockDelay > 0f)
            yield return new WaitForSeconds(doorUnlockDelay);

        Door[] doorArray = GetComponentsInChildren<Door>();

        // Trigger open doors
        foreach (Door door in doorArray)
		{
            door.UnlockDoor();
		}
	}

	#region Validation

#if UNITY_EDITOR

	private void OnValidate()
	{
        HelperUtilities.ValidateCheckNullValue(this, nameof(environmentGameObject), environmentGameObject);
	}

#endif

	#endregion Validation
}
