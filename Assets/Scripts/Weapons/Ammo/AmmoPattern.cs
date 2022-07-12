using UnityEngine;


public class AmmoPattern : MonoBehaviour, IFireable
{
	#region Tooltip
	[Tooltip("Populate the array with the child ammo game objects")]
	#endregion Tooltip
	[SerializeField] private Ammo[] ammoArray;

	private float ammoRange;

	private float ammoSpeed;

	private Vector3 fireDirectionVector;

	private float fireDirectionAngle;

	private AmmoDetailsSO ammoDetails;

	private float ammoChargeTimer;

	public GameObject GetGameObject()
	{
		return gameObject;
	}

	public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector,
		bool overrideAmmoMovement = false)
	{
		this.ammoDetails = ammoDetails;

		this.ammoSpeed = ammoSpeed;

		// Set fire direction
		SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

		// Set ammo range
		ammoRange = ammoDetails.ammoRange;

		// Activate ammo pattern game object
		gameObject.SetActive(true);

		// Loop through all child ammo and initialize it
		foreach (Ammo ammo in ammoArray)
		{
			ammo.InitialiseAmmo(ammoDetails, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector, true);
		}

		// Set ammo charge timer - this will hold the ammo briefly
		if (ammoDetails.ammoChargeTime > 0f)
		{
			ammoChargeTimer = ammoDetails.ammoChargeTime;
		}
		else
		{
			ammoChargeTimer = 0f;
		}
	}

	private void Update()
	{
		// Ammo charge effect
		if (ammoChargeTimer > 0f)
		{
			ammoChargeTimer -= Time.deltaTime;

			return;
		}

		// Calculate distance vector to move ammo
		Vector3 distanceVector = fireDirectionVector * ammoSpeed * Time.deltaTime;

		transform.position += distanceVector;

		// Rotate ammo
		transform.Rotate(new Vector3(0f, 0f, ammoDetails.ammoRotationSpeed * Time.deltaTime));

		// Disable after max range reached
		ammoRange -= distanceVector.magnitude;

		if (ammoRange < 0f)
		{
			DisableAmmo();
		}
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

		// Set ammo fire direction
		fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
	}

	private void DisableAmmo()
	{
		gameObject.SetActive(false);
	}

	#region Validation

#if UNITY_EDITOR

	private void OnValidate()
	{
		HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoArray), ammoArray);
	}

#endif

	#endregion Validation
}
