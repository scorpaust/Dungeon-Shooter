using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ActivateRooms : MonoBehaviour
{
	#region Header POPULATE WITH MINIMAP CAMERA
	[Header("POPULATE WITH MINIMAP CAMERA")]
	#endregion Header POPULATE WITH MINIMAP CAMERA
	[SerializeField] private Camera miniMapCamera;

	private void Start()
	{
		InvokeRepeating("EnableRooms", 0.5f, 0.75f);
	}

	private void EnableRooms()
	{
		// Iterate through dungeon rooms
		foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
		{
			Room room = keyValuePair.Value;

			HelperUtilities.CameraWorldPositionBounds(out Vector2Int minimapCameraWorldPositionLowerBounds, out Vector2Int minimapCameraWorldPositionUpperBounds, miniMapCamera);

			// If room is within miniMap camera viewport then activate room game object
			if ((room.lowerBounds.x <= minimapCameraWorldPositionUpperBounds.x && room.lowerBounds.y <= minimapCameraWorldPositionUpperBounds.y) &&
				(room.upperBounds.x >= minimapCameraWorldPositionLowerBounds.x && room.upperBounds.y >= minimapCameraWorldPositionLowerBounds.y))
			{
				room.instanciatedRoom.gameObject.SetActive(true);
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
