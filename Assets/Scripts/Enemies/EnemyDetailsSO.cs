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

	#region Validation

#if UNITY_EDITOR

	private void OnValidate()
	{
		HelperUtilities.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);

		HelperUtilities.ValidateCheckNullValue(this, nameof(enemyPrefab), enemyPrefab);

		HelperUtilities.ValidateCheckPositiveValue(this, nameof(chaseDistance), chaseDistance, false);
	}

#endif

	#endregion Validation
}
