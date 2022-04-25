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
