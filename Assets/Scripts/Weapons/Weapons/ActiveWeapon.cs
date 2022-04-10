using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SetActiveWeaponEvent))]
[DisallowMultipleComponent]
public class ActiveWeapon : MonoBehaviour
{
	#region Tooltip
	[Tooltip("Populate with the spriteRenderer on the child weapon gameObject")]
	#endregion Tooltip
	[SerializeField] private SpriteRenderer weaponSpriteRenderer;

	#region Tooltip
	[Tooltip("Populate with the PollygonCollider2D on the child weapon gameObject")]
	#endregion Tooltip
	[SerializeField] private PolygonCollider2D weaponPollygonCollider2D;

	#region Tooltip
	[Tooltip("Populate with the transform on the weaponShootPosition gameObject")]
	#endregion Tooltip
	[SerializeField] private Transform weaponShootPositionTransform;

	#region Tooltip
	[Tooltip("Populate with the transform on the weaponEffectPosition gameObject")]
	#endregion Tooltip
	[SerializeField] private Transform weaponEffectPositionTransform;

	private SetActiveWeaponEvent setWeaponEvent;

	private Weapon currentWeapon;

	private void Awake()
	{
		// Load components
		setWeaponEvent = GetComponent<SetActiveWeaponEvent>();
	}

	private void OnEnable()
	{
		setWeaponEvent.OnSetActiveWeapon += SetWeaponEvent_OnSetActiveWeapon;
	}

	private void OnDisable()
	{
		setWeaponEvent.OnSetActiveWeapon -= SetWeaponEvent_OnSetActiveWeapon;
	}

	private void SetWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
	{
		SetWeapon(setActiveWeaponEventArgs.weapon);
	}

	private void SetWeapon(Weapon weapon)
	{
		currentWeapon = weapon;

		// Set current weapon sprite
		weaponSpriteRenderer.sprite = currentWeapon.weaponDetails.weaponSprite;

		// If the weapon has a pollygon collider and a sprite then set it to the weapon sprite physics shape
		if (weaponPollygonCollider2D != null && weaponSpriteRenderer.sprite != null)
		{
			// Get sprite physics shape  - this returns the sprite physics shape points as a list of Vector2s
			List<Vector2> spritePhysicsShapePointsList = new List<Vector2>();

			weaponSpriteRenderer.sprite.GetPhysicsShape(0, spritePhysicsShapePointsList);

			// Set pollygon collider on weapon to pick up physics shape for sprite - set collider points to sprite physics shape points
			weaponPollygonCollider2D.points = spritePhysicsShapePointsList.ToArray();
		}

		// Set weapon shoot position
		weaponShootPositionTransform.localPosition = currentWeapon.weaponDetails.weaponShootPosition;
	}

	public AmmoDetailsSO GetCurrentAmmo()
	{
		return currentWeapon.weaponDetails.weaponCurrentAmmo;
	}

	public Weapon GetCurrentWeapon()
	{
		return currentWeapon;
	}

	public Vector3 GetShootPosition()
	{
		return weaponShootPositionTransform.position;
	}

	public Vector3 GetShootEffectPosition()
	{
		return weaponEffectPositionTransform.position;
	}

	public void RemoveCurrentWeapon()
	{
		currentWeapon = null;
	}

	#region Validation

#if UNITY_EDITOR

	private void OnValidate()
	{
		HelperUtilities.ValidateCheckNullValue(this, nameof(weaponSpriteRenderer), weaponSpriteRenderer);

		HelperUtilities.ValidateCheckNullValue(this, nameof(weaponPollygonCollider2D), weaponPollygonCollider2D);

		HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootPositionTransform), weaponShootPositionTransform);

		HelperUtilities.ValidateCheckNullValue(this, nameof(weaponEffectPositionTransform), weaponEffectPositionTransform);
	}

#endif

	#endregion Validation
}
