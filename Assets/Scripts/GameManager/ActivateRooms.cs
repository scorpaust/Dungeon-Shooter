using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ActivateRooms : MonoBehaviour
{
	#region Header POPULATE WITH MINIMAP CAMERA
	[Header("POPULATE WITH MINIMAP CAMERA")]
	#endregion Header POPULATE WITH MINIMAP CAMERA
	[SerializeField] private Camera miniMapCamera;

	private Camera cameraMain;

	private void Start()
	{
		// Cache main camera
		cameraMain = Camera.main;

		InvokeRepeating("EnableRooms", 0.5f, 0.75f);
	}

	private void EnableRooms()
	{
		if (GameManager.Instance.gameState == GameState.dungeonOverviewMap)
			return;

		HelperUtilities.CameraWorldPositionBounds(out Vector2Int minimapCameraWorldPositionLowerBounds, out Vector2Int minimapCameraWorldPositionUpperBounds, miniMapCamera);

		HelperUtilities.CameraWorldPositionBounds(out Vector2Int mainCameraWorldPositionLowerBounds, out Vector2Int mainCameraWorldPositionUpperBounds, cameraMain);

		// Iterate through dungeon rooms
		foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
		{
			Room room = keyValuePair.Value;

			// If room is within miniMap camera viewport then activate room game object
			if ((room.lowerBounds.x <= minimapCameraWorldPositionUpperBounds.x && room.lowerBounds.y <= minimapCameraWorldPositionUpperBounds.y) &&
				(room.upperBounds.x >= minimapCameraWorldPositionLowerBounds.x && room.upperBounds.y >= minimapCameraWorldPositionLowerBounds.y))
			{
				room.instanciatedRoom.gameObject.SetActive(true);

				// If room is within the main camera viewport then activate environment game objects
				if ((room.lowerBounds.x <= mainCameraWorldPositionUpperBounds.x && room.lowerBounds.y <= mainCameraWorldPositionUpperBounds.y) &&
				(room.upperBounds.x >= mainCameraWorldPositionLowerBounds.x && room.upperBounds.y >= mainCameraWorldPositionLowerBounds.y))
				{
					room.instanciatedRoom.ActivateEnvironmentGameObjects();
				}
				else
				{
					room.instanciatedRoom.DeactivateEnvironmentGameObjects();
				}
			}
			else
			{
				room.instanciatedRoom.gameObject.SetActive(false);
			}

		}
	}

	#region Validation

#if UNITY_EDITOR

	private void OnValidate()
	{
		HelperUtilities.ValidateCheckNullValue(this, nameof(miniMapCamera), miniMapCamera);
	}

#endif

	#endregion Validation
}
