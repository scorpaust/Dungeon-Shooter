using UnityEngine;
[System.Serializable]

public class Doorway
{
	Vector2Int position;
	public Orientation orientation;
	public GameObject doorPrefab;

	#region Header
	[Header("The upper left position to start copying from.")]
	#endregion Header

	public Vector2Int doorwayStartCopyPosition;

	#region Header
	[Header("The width of the tiles in the doorway to copy over.")]
	#endregion Header

	public int doorwayCopyTileWidth;

	#region Header
	[Header("The height of the tiles in the doorway to copy over.")]
	#endregion Header

	public int doorwayCopyTileHeight;

	[HideInInspector]
	public bool isConnected = false;

	[HideInInspector]
	public bool isUnavailable = false;
}
