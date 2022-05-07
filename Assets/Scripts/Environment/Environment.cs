using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Environment : MonoBehaviour
{
	#region Header References
	[Header("References")]
	#endregion Header References
	#region Tooltip
	[Tooltip("Populate with the SpriteRenderer component on the prefab")]
	#endregion Tooltip

	public SpriteRenderer spriteRenderer;

	#region Validation

#if UNITY_EDITOR

	private void OnValidate()
	{
		HelperUtilities.ValidateCheckNullValue(this, nameof(spriteRenderer), spriteRenderer);
	}

#endif

	#endregion Validation
}
