using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Ammo : MonoBehaviour, IFireable
{
	#region Tooltip
	[Tooltip("Populate with child TrailRenderer component")]
	[SerializeField] private TrailRenderer trailRenderer;
	#endregion

	private float ammoRange = 0f; // the range of each ammo

	private float ammoSpeed;

	private Vector3 fireDirectionVector;

	private float fireDirectionAngle;

	private SpriteRenderer spriteRenderer;

	private AmmoDetailsSO ammoDetails;

	private float ammoChargeTimer;

	private bool isAmmoMaterialSet = false;

	private bool overrideAmmoMovement;

	private bool isColliding = false;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();	
	}

	private void Update()
	{
		if (ammoChargeTimer > 0f)
		{
			ammoChargeTimer -= Time.deltaTime;

			return;
		}

		else if (!isAmmoMaterialSet)
		{
			SetAmmoMaterial(ammoDetails.ammoMaterial);

			isAmmoMaterialSet = true;
		}

		// Calculate distance vector to move ammo
		Vector3 distanceVector = fireDirectionVector * ammoSpeed * Time.deltaTime;

		transform.position += distanceVector;

		// Disable after max range reached
		ammoRange -= distanceVector.magnitude;

		if (ammoRange < 0f)
		{
			DisableAmmo();
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// If already colliding with something return
		if (isColliding) return;

		// Deal damage to collision object
		DealDamage(collision);

		// Show ammo hit effect
		AmmoHitEffect();

		DisableAmmo();
	}

	private void DealDamage(Collider2D collision)
	{
		Health health = collision.GetComponent<Health>();

		if (health != null)
		{
			// Set is colliding to prevent ammo dealing damage multiple times
			isColliding = true;

			health.TakeDamage(ammoDetails.ammoDamage);
		}
	}

	public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
	{
		#region Ammo

		this.ammoDetails = ammoDetails;

		// Initialize isColliding
		isColliding = false;

		// Set fire direction
		SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

		// Set ammo sprite
		spriteRenderer.sprite = ammoDetails.ammoSprite;

		// Set initial ammo material depending on whether there is an ammo charge period
		if (ammoDetails.ammoChargeTime > 0f)
		{
			// Set ammo charge timer
			ammoChargeTimer = ammoDetails.ammoChargeTime;

			SetAmmoMaterial(ammoDetails.ammoChargeMaterial);

			isAmmoMaterialSet = false;
		}
		else
		{
			ammoChargeTimer = 0f;

			SetAmmoMaterial(ammoDetails.ammoMaterial);

			isAmmoMaterialSet = true;
		}

		// Set the ammo range
		ammoRange = ammoDetails.ammoRange;

		// Set the ammo speed
		this.ammoSpeed = ammoSpeed;

		// Override ammo movement
		this.overrideAmmoMovement = overrideAmmoMovement;

		// Activate ammo game object
		gameObject.SetActive(true);

		#endregion Ammo

		#region Trail

		if (ammoDetails.isAmmoTrail)
		{
			trailRenderer.gameObject.SetActive(true);

			trailRenderer.emitting = true;

			trailRenderer.material = ammoDetails.ammoTrailMaterial;

			trailRenderer.startWidth = ammoDetails.ammoTrailStartWidth;

			trailRenderer.endWidth = ammoDetails.ammoTrailEndWidth;

			trailRenderer.time = ammoDetails.ammoTrailTime;
		}
		else
		{
			trailRenderer.emitting = false;

			trailRenderer.gameObject.SetActive(false);
		}

		#endregion Trail
	}

	private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
	{
		// Calculate random spread value between min and max values
		float randomSpread = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);

		// Get a random spread toggle of 1 or -1
		int spreadToggle = Random.Range(0, 2) * 2 - 1;

		if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
		{
			fireDirectionAngle = aimAngle;
		}
		else
		{
			fireDirectionAngle = weaponAimAngle;
		}

		// Adjust ammo fire angle by random spread
		fireDirectionAngle += spreadToggle * randomSpread;

		// Set ammo rotation
		transform.eulerAngles = new Vector3(0f, 0f, fireDirectionAngle);

		// Set ammo fire direction
		fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
	}

	private void DisableAmmo()
	{
		gameObject.SetActive(false);
	}

	private void AmmoHitEffect()
	{
		// Process if a hit effect has been specified
		if (ammoDetails.ammoHitEffect != null && ammoDetails.ammoHitEffect.ammoHitEffectPrefab != null)
		{
			// Get ammo hit effect gameObject from the pool (with particle system component)
			AmmoHitEffect ammoHitEffect = (AmmoHitEffect)PoolManager.Instance.ReuseComponent(ammoDetails.ammoHitEffect.ammoHitEffectPrefab, transform.position, Quaternion.identity);

			// Set hit effect
			ammoHitEffect.SetHitEffect(ammoDetails.ammoHitEffect);

			ammoHitEffect.gameObject.SetActive(true);
		}
	}

	public void SetAmmoMaterial(Material material)
	{
		spriteRenderer.material = material;
	}

	public GameObject GetGameObject()
	{
		return gameObject;
	}

	#region Validation

#if UNITY_EDITOR

	private void OnValidate()
	{
		HelperUtilities.ValidateCheckNullValue(this, nameof(trailRenderer), trailRenderer);
	}

#endif

	#endregion Validation

}
