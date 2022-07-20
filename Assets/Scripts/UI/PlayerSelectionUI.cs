using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerSelectionUI : MonoBehaviour
{
	#region Tooltip
	[Tooltip("Populate with the sprite renderer on child gameobject WeaponAnchorPosition/WeaponRotationPoint/Hand")]
	#endregion Tooltip
	public SpriteRenderer playerHandSpriteRenderer;

	#region Tooltip
	[Tooltip("Populate with the sprite renderer on child gameobject HandNoWeapon")]
	#endregion Tooltip
	public SpriteRenderer playerHandNoWeaponSpriteRenderer;

	#region Tooltip
	[Tooltip("Populate with the sprite renderer on child gameobject WeaponAnchorPosition/WeaponRotationPoint/Weapon")]
	#endregion Tooltip
	public SpriteRenderer playerWeaponSpriteRenderer;

	#region Tooltip
	[Tooltip("Populate with the animator component")]
	#endregion Tooltip
	public Animator animator;

	#region Validation

#if UNITY_EDITOR

	private void OnValidate()
	{
		HelperUtilities.ValidateCheckNullValue(this, nameof(playerHandSpriteRenderer), playerHandSpriteRenderer);

		HelperUtilities.ValidateCheckNullValue(this, nameof(playerHandNoWeaponSpriteRenderer), playerHandNoWeaponSpriteRenderer);

		HelperUtilities.ValidateCheckNullValue(this, nameof(playerWeaponSpriteRenderer), playerWeaponSpriteRenderer);

		HelperUtilities.ValidateCheckNullValue(this, nameof(animator), animator);
	}

#endif

	#endregion Validation
}
