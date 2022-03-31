using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDetails_", menuName = "Scriptable Objects/Player/Player Details")]
public class PlayerDetailsSO : ScriptableObject
{
	#region Header PLAYER BASE DETAILS
	[Space(10)]
	[Header("PLAYER BASE DETAILS")]
	#endregion Header PLAYER BASE DETAILS
	#region Tooltip
	[Tooltip("Player character name")]
	#endregion Tooltip
	public string playerCharacterName;

	#region Tooltip
	[Tooltip("Prefab gameobject for the player")]
	#endregion Tooltip
	public GameObject playerPrefab;

	#region Tooltip
	[Tooltip("Player runtime animator controller")]
	#endregion Tooltip
	public RuntimeAnimatorController runtimeAnimatorController;

	#region Header HEALTH
	[Space(10)]
	[Header("HEALTH")]
	#endregion Header HEALTH
	#region Tooltip
	[Tooltip("Player starting health amount")]
	#endregion Tooltip
	public int playerHealthAmount;

	#region Header OTHER
	[Space(10)]
	[Header("OTHER")]
	#endregion Header OTHER
	#region Tooltip
	[Tooltip("Player icon sprite to be used in the minimap")]
	#endregion Tooltip
	public Sprite playerMinimapIcon;

	#region Tooltip
	[Tooltip("Player hand sprite")]
	#endregion Tooltip
	public Sprite playerHandSprite;

	#region Validation

#if UNITY_EDITOR

	private void OnValidate()
	{
		HelperUtilities.ValidateCheckEmptyString(this, nameof(playerCharacterName), playerCharacterName);

		HelperUtilities.ValidateCheckNullValues(this, nameof(playerPrefab), playerPrefab);

		HelperUtilities.ValidateCheckPositiveValue(this, nameof(playerHealthAmount), playerHealthAmount, false);

		HelperUtilities.ValidateCheckNullValues(this, nameof(playerMinimapIcon), playerMinimapIcon);

		HelperUtilities.ValidateCheckNullValues(this, nameof(playerHandSprite), playerHandSprite);

		HelperUtilities.ValidateCheckNullValues(this, nameof(runtimeAnimatorController), runtimeAnimatorController);
	}

#endif

	#endregion Validation

}