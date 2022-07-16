using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMap : SingletonMonobehaviour<DungeonMap>
{
	#region Header GameObject References
	[Space(10)]
	[Header("GameObject References")]
	#endregion Header GameObject References
	#region Tooltip
	[Tooltip("Populate with the Minimap UI game object")]
	#endregion Tooltip
	[SerializeField] private GameObject minimapUI;

	private Camera dungeonMapCamera;

	private Camera cameraMain;

	private void Start()
	{
		// Cache main camera
		cameraMain = Camera.main;

		// Get player transform
		Transform playerTransform = GameManager.Instance.GetPlayer().transform;

		// Populate player as the cinemachine camera target
		CinemachineVirtualCamera cinemachineVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();

		cinemachineVirtualCamera.Follow = playerTransform;

		// Get dungeon map camera
		dungeonMapCamera = GetComponentInChildren<Camera>();

		dungeonMapCamera.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && GameManager.Instance.gameState == GameState.dungeonOverviewMap)
		{
			GetRoomClicked();
		}
	}

	private void GetRoomClicked()
	{
		Vector3 worldPosition = dungeonMapCamera.ScreenToWorldPoint(Input.mousePosition);

		worldPosition = new Vector3(worldPosition.x, worldPosition.y, 0f);

		Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(new Vector2(worldPosition.x, worldPosition.y), 1f);

		foreach (Collider2D collider2D in collider2DArray)
		{
			if (collider2D.GetComponent<InstantiatedRoom>() != null)
			{
				InstantiatedRoom instantiatedRoom = collider2D.GetComponent<InstantiatedRoom>();

				if (instantiatedRoom.room.isClearedOfEnemies && instantiatedRoom.room.isPreviouslyVisited)
				{
					StartCoroutine(MovePlayerToRoom(worldPosition, instantiatedRoom.room));
				}
			}
		}
	}

	private IEnumerator MovePlayerToRoom(Vector3 worldPosition, Room room)
	{
		StaticEventHandler.CallRoomChangedEvent(room);

		yield return StartCoroutine(GameManager.Instance.Fade(0f, 1f, 0f, Color.black));

		ClearDungeonOverviewMap();

		GameManager.Instance.GetPlayer().playerControl.DisablePlayer();

		Vector3 spawnPosition = HelperUtilities.GetSpawnPositionNearestToPlayer(worldPosition);

		GameManager.Instance.GetPlayer().transform.position = spawnPosition;

		yield return StartCoroutine(GameManager.Instance.Fade(1f, 0f, 1f, Color.black));

		GameManager.Instance.GetPlayer().playerControl.EnablePlayer();
	}

	public void DisplayDungeonOverviewMap()
	{
		// Set game state
		GameManager.Instance.previousGameState = GameManager.Instance.gameState;

		GameManager.Instance.gameState = GameState.dungeonOverviewMap;

		// Disable player
		GameManager.Instance.GetPlayer().playerControl.DisablePlayer();

		// Disable main camera and enable dungeon overview camera
		cameraMain.gameObject.SetActive(false);

		dungeonMapCamera.gameObject.SetActive(true);

		// Ensure all rooms are active so they can be displayed
		ActivateRoomsForDisplay();

		// Disable small minimap UI
		minimapUI.SetActive(false);
	}

	public void ClearDungeonOverviewMap()
	{
		// Set game state
		GameManager.Instance.gameState = GameManager.Instance.previousGameState;

		GameManager.Instance.previousGameState = GameState.dungeonOverviewMap;

		// Enable player
		GameManager.Instance.GetPlayer().playerControl.EnablePlayer();

		// Enable main camera and disable dungeon overview camera
		cameraMain.gameObject.SetActive(true);

		dungeonMapCamera.gameObject.SetActive(false);

		// Enable small minimap UI
		minimapUI.SetActive(true);
	}

	private void ActivateRoomsForDisplay()
	{
		// Iterate through dungeon rooms
		foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
		{
			Room room = keyValuePair.Value;

			room.instanciatedRoom.gameObject.SetActive(true);
		}
	}
}
