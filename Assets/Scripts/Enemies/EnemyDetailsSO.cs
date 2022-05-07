using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetails_", menuName = "Scriptable Objects/Enemy/EnemyDetails")]
public class EnemyDetailsSO : ScriptableObject
{
	#region Header BASE ENEMY DETAILS
	[Space(10)]
	[Header("BASE ENEMY DETAILS")]
	#endregion Header BASE ENEMY DETAILS

	#region Tooltip
	[Tooltip("The name of the enemy")]
	#endregion Tooltip
	public string enemyName;

	#region Tooltip
	[Tooltip("The prefab for the enemy")]
	#endregion Tooltip
	public GameObject enemyPrefab;

	#region Tooltip
	[Tooltip("Distance to the player before enemy starts chasing")]
	#endregion Tooltip
	public float chaseDistance = 50f;

	#region Header ENEMY MATERIAL
	[Space(10)]
	[Header("ENEMY MATERIAL")]
	#endregion Header ENEMY MATERIAL
	#region Tooltip
	[Tooltip("This is the standard lit shader material for the enemy (used after the enemy materializes)")]
	#endregion Tooltip
	public Material enemyStandardMaterial;

	#region Header ENEMY MATERIALIZE SETTINGS
	[Space(10)]
	[Header("ENEMY MATERIALIZE SETTINGS")]
	#endregion Header ENEMY MATERIALIZE SETTINGS
	#region Tooltip
	[Tooltip("The time in seconds that it takes the enemy to materialize")]
	#endregion Tooltip
	public float enemyMaterializeTime;
	#region Tooltip
	[Tooltip("The shader to be used when enemy materializes")]
	#endregion Tooltip
	public Shader enemyMaterializeShader;
	[ColorUsage(true, true)]
	#region Tooltip
	[Tooltip("The color to use when the enemy materializes. This is an HDR color so intensity can be set to cause glowing / bloom")]
	#endregion Tooltip
	public Color enemyMaterializeColor;

	#region Header ENEMY WEAPON SETTINGS
	[Space(10)]
	[Header("ENEMY WEAPON SETTINGS")]
	#endregion Header ENEMY WEAPON SETTINGS
	#region Tooltip
	[Tooltip("The weapon for the enemy - none if the enemy doesn't have a weapon")]
	#endregion Tooltip
	public WeaponDetailsSO enemyWeapon;
	#region Tooltip
	[Tooltip("The minimum time delay interval in seconds between bursts of enemy shooting. This value should be greater than zero. A random value will be selected between the min and max values")]
	#endregion Tooltip
	public float firingIntervalMin = 0.1f;
	#region Tooltip
	[Tooltip("The maximum time delay interval in seconds between bursts of enemy shooting. This value should be greater than zero. A random value will be selected between the min and max values")]
	#endregion Tooltip
	public float firingIntervalMax = 1f;
	#region Tooltip
	[Tooltip("The minimum firing duration that the enemy shoots for during a firing burst. This value should be greater than zero. A random value will be selected between the min and max values")]
	#endregion Tooltip
	public float firingDurationMin = 1f;
	#region Tooltip
	[Tooltip("The maximum firing duration that the enemy shoots for during a firing burst. This value should be greater than zero. A random value will be selected between the min and max values")]
	#endregion Tooltip
	public float firingDurationMax = 2f;
	#region Tooltip
	[Tooltip("Select this if the line of sight is required of the player before the enemy fires. If line of sight isn't selected the enemy will fire regardless of obstacles whenever the player is in range")]
	#endregion
	public bool firingLineOfSightRequired;

	#region Header ENEMY HEALTH
	[Space(10)]
	[Header("ENEMY HEALTH")]
	#endregion Header ENEMY HEALTH
	#region Tooltip
	[Tooltip("The health of the enemy for each level")]
	#endregion Tooltip
	public EnemyHealthDetails[] enemyHealthDetailsArray;
	#region Tooltip
	[Tooltip("Select if has immunity period immediately after being hit. If so, specify the immunity time in seconds in the other field")]
	#endregion Tooltip
	public bool isImmuneAfterHit = false;
	#region Tooltip
	[Tooltip("Immunity time in seconds after being hit")]
	#endregion Tooltip
	public float hitImmunityTime;
	#region Tooltip
	[Tooltip("Select to display health bar for the enemy")]
	#endregion Tooltip
	public bool isHealthBarDisplayed = false;

	#region Validation

#if UNITY_EDITOR

	private void OnValidate()
	{
		HelperUtilities.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);

		HelperUtilities.ValidateCheckNullValue(this, nameof(enemyPrefab), enemyPrefab);

		HelperUtilities.ValidateCheckPositiveValue(this, nameof(chaseDistance), chaseDistance, false);

		HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingIntervalMin), firingIntervalMin, nameof(firingIntervalMax), firingIntervalMax, false);

		HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingDurationMin), firingDurationMin, nameof(firingDurationMax), firingDurationMax, false);

		HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyHealthDetailsArray), enemyHealthDetailsArray);

		if (isImmuneAfterHit)
		{
			HelperUtilities.ValidateCheckPositiveValue(this, nameof(hitImmunityTime), hitImmunityTime, false);
		}
	}

#endif

	#endregion Validation
}
