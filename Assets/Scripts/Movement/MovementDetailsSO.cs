using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/Movement Details")]
public class MovementDetailsSO : ScriptableObject
{
	#region Header MOVEMENT DETAILS
	[Space(10)]
	[Header("MOVEMENT DETAILS")]
	#endregion Header MOVEMENT DETAILS 

	#region Tooltip
	[Tooltip("The minimum move speed. The GetMoveSpeed method calculates a random value between the minimum and the maximum")]
	#endregion Tooltip
	public float minMoveSpeed = 8f;

	#region Tooltip
	[Tooltip("The maximum move speed. The GetMoveSpeed method calculates a random value between the minimum and the maximum")]
	#endregion Tooltip
	public float maxMoveSpeed = 8f;

	#region Tooltip
	[Tooltip("if there is a roll movement, this is the roll speed")]
	#endregion Tooltip
	public float rollSpeed;

	#region Tooltip
	[Tooltip("if there is a roll movement, this is the roll distance")]
	#endregion Tooltip
	public float rollDistance;

	#region Tooltip
	[Tooltip("if there is a roll movement, this is the cool down time in seconds between roll actions")]
	#endregion Tooltip
	public float rollCoolDownTime;

	public float GetMoveSpeed()
	{
		if (minMoveSpeed == maxMoveSpeed)
		{
			return minMoveSpeed;
		}
		else
		{
			return Random.Range(minMoveSpeed, maxMoveSpeed);
		}
	}

	#region Validation

#if UNITY_EDITOR

	private void OnValidate()
	{
		HelperUtilities.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed), maxMoveSpeed, false);

		if (rollDistance != 0f || rollSpeed != 0f || rollCoolDownTime != 0f)
		{
			HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollDistance), rollDistance, false);

			HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollSpeed), rollSpeed, false);

			HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollCoolDownTime), rollCoolDownTime, false);
		}
	}

#endif

	#endregion Validation
}
