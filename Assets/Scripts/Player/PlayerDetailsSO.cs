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
	#region Tooltip
	[Tooltip("Select if has immunity period imediately after being hit. If so, specify the immunity time in seconds in the other field")]
	#endregion Tooltip
	public bool isImmuneAfterHit = false;
	#region Tooltip
	[Tooltip("Immunity time in seconds after being hit")]
	#endregion Tooltip
	public float hitImmunityTime; 

	#region Header WEAPON
	[Space(10)]
	[Header("WEAPON")]
	#endregion Header WEAPON
	#region Tooltip
	[Tooltip("Player initial starting weapon")]
	#endregion
	public WeaponDetailsSO startingWeapon;
	#region Tooltip
	[Tooltip("Populate with the list of startingWeapons")]
	#endregion
	public List<WeaponDetailsSO> startingWeaponList;

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

		HelperUtilities.ValidateCheckNullValue(this, nameof(playerPrefab), playerPrefab);

		HelperUtilities.ValidateCheckPositiveValue(this, nameof(playerHealthAmount), playerHealthAmount, false);

		HelperUtilities.ValidateCheckNullValue(this, nameof(startingWeapon), startingWeapon);

		HelperUtilities.ValidateCheckNullValue(this, nameof(playerMinimapIcon), playerMinimapIcon);

		HelperUtilities.ValidateCheckNullValue(this, nameof(playerHandSprite), playerHandSprite);

		HelperUtilities.ValidateCheckNullValue(this, nameof(runtimeAnimatorController), runtimeAnimatorController);

		HelperUtilities.ValidateCheckEnumerableValues(this, nameof(startingWeaponList), startingWeaponList);

		if (isImmuneAfterHit)
		{
			HelperUtilities.ValidateCheckPositiveValue(this, nameof(hitImmunityTime), hitImmunityTime, false);
		}
	}

#endif

	#endregion Validation

}
