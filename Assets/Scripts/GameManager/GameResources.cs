using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Resources.Load<GameResources>("GameResources");
			}
			return instance;
		}
	}

	#region Header DUNGEON
	[Space(10)]
	[Header("DUNGEON")]
	#endregion Header

	#region Tooltip
	[Tooltip("Populate with the dungeon RoomNodeTypeListSO")]
	#endregion Tooltip
	public RoomNodeTypeListSO roomNodeTypeList;

	#region Header PLAYER
	[Space(10)]
	[Header("PLAYER")]
	#endregion Header PLAYER

	#region Tooltip
	[Tooltip("The current player scriptable object - this is used to reference the current player between scenes")]
	#endregion Tooltip
	public CurrentPlayerSO currentPlayer;

	#region Header MATERIALS
	[Space(10)]
	[Header("MATERIALS")]
	#endregion Header MATERIALS

	#region Tooltip
	[Tooltip("Dimmed Material")]
	#endregion Tooltip
	public Material dimmedMaterial;

	#region Tooltip
	[Tooltip("Sprite-Lit-Default Material")]
	#endregion Tooltip
	public Material litMaterial;

	#region Tooltip
	[Tooltip("Populate with the variable lit shader")]
	#endregion Tooltip
	public Shader variableLitShader;

	#region Validation

#if UNITY_EDITOR

	private void OnValidate()
	{
		HelperUtilities.ValidateCheckNullValues(this, nameof(roomNodeTypeList), roomNodeTypeList);

		HelperUtilities.ValidateCheckNullValues(this, nameof(currentPlayer), currentPlayer);

		HelperUtilities.ValidateCheckNullValues(this, nameof(litMaterial), litMaterial);

		HelperUtilities.ValidateCheckNullValues(this, nameof(dimmedMaterial), dimmedMaterial);

		HelperUtilities.ValidateCheckNullValues(this, nameof(variableLitShader), variableLitShader);
	}

#endif

	#endregion Validation
}
