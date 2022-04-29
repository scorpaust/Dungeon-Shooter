using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyWeaponAI : MonoBehaviour
{
	#region Tooltip
	[Tooltip("Select the layers that enemy bullets will hit")]
	#endregion Tooltip
	[SerializeField] private LayerMask layerMask;

	#region Tooltip
	[Tooltip("Populate this with the WeaponShootPosition child game object transform")]
	#endregion Tooltip
	[SerializeField] private Transform weaponShootPosition;

	private Enemy enemy;

	private EnemyDetailsSO enemyDetails;

	private float firingIntervalTimer;

	private float firingDurationTimer;

	private void Awake()
	{
		// Load components
		enemy = GetComponent<Enemy>();
	}

	private void Start()
	{
		enemyDetails = enemy.enemyDetails;

		firingIntervalTimer = WeaponShootInterval();

		firingDurationTimer = WeaponShootDuration();
	}

	private void Update()
	{
		// Update timers
		firingIntervalTimer -= Time.deltaTime;

		// Interval timer
		if (firingIntervalTimer < 0f)
		{
			if (firingDurationTimer >= 0)
			{
				firingDurationTimer -= Time.deltaTime;

				FireWeapon();
			}
			else
			{
				// Reset timers
				firingIntervalTimer = WeaponShootInterval();

				firingDurationTimer = WeaponShootDuration();
			}
		}
	}

	private float WeaponShootDuration()
	{
		// Calculate a random weapon shoot duration
		return Random.Range(enemyDetails.firingDurationMin, enemyDetails.firingDurationMax);
	}

	private float WeaponShootInterval()
	{
		// Calculate a random weapon shoot interval
		return Random.Range(enemyDetails.firingIntervalMin, enemyDetails.firingIntervalMax);
	}

	private void FireWeapon()
	{
		// Player distance
		Vector3 playerDirectionVector = GameManager.Instance.GetPlayer().GetPlayerPosition() - transform.position;

		// Calculate direction vector of player from weapon shoot position
		Vector3 weaponDirection = (GameManager.Instance.GetPlayer().GetPlayerPosition() - weaponShootPosition.position);

		// Get weapon to player angle
		float weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);

		// Get enemy to player angle
		float enemyAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirectionVector);

		// Set enemy aim direction
		AimDirection enemyAimDirection = HelperUtilities.GetAimDirection(enemyAngleDegrees);

		// Trigger weapon aim event
		enemy.aimWeaponEvent.CallAimWeaponEvent(enemyAimDirection, enemyAngleDegrees, weaponAngleDegrees, weaponDirection);

		// Only fire if enemy has a weapon
		if (enemyDetails.enemyWeapon != null)
		{
			// Get ammo range
			float enemyAmmoRange = enemyDetails.enemyWeapon.weaponCurrentAmmo.ammoRange;

			// Is the player in range
			if (playerDirectionVector.magnitude <= enemyAmmoRange)
			{
				// Does this enemy require line of sight to the player before firing
				if (enemyDetails.firingLineOfSightRequired && !IsPlayerInLineOfSight(weaponDirection, enemyAmmoRange)) return;

				// Trigger fire weapon event
				enemy.fireWeaponEvent.CallFireWeaponEvent(true, true, enemyAimDirection, enemyAngleDegrees, weaponAngleDegrees, weaponDirection);
			}
		}
	}

	private bool IsPlayerInLineOfSight(Vector3 weaponDirection, float enemyAmmoRange)
	{
		RaycastHit2D rayCastHit2D = Physics2D.Raycast(weaponShootPosition.position, (Vector2)weaponDirection, enemyAmmoRange, layerMask);

		if (rayCastHit2D && rayCastHit2D.transform.CompareTag(Settings.playertag))
		{
			return true;
		}

		return false;
	}

	#region Validation

#if UNITY_EDITOR

	private void OnValidate()
	{
		HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootPosition), weaponShootPosition);
	}

#endif

	#endregion Validation
}
